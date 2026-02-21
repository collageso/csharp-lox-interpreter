namespace LoxInterpreter.Scanning;

using LoxInterpreter.Reporting;
using LoxInterpreter.Abstractions;

public class Lexer
{
    private readonly string _source;
    private readonly List<Token> _tokens = new();
    private readonly DiagnosticManager _diagnosticManager;

    private int _startIndex = 0;
    private int _currentIndex = 0;
    private int _currentLine = 1;
    private int _startColumn = 1;
    private int _currentColumn = 1;

    private static readonly Dictionary<string, TokenType> Keywords = new()
    {
        {"and", TokenType.AND},
        {"class", TokenType.CLASS},
        {"else", TokenType.ELSE},
        {"false", TokenType.FALSE},
        {"for", TokenType.FOR},
        {"fun", TokenType.FUN},
        {"if", TokenType.IF},
        {"nil", TokenType.NIL},
        {"or", TokenType.OR},
        {"print", TokenType.PRINT},
        {"return", TokenType.RETURN},
        {"super", TokenType.SUPER},
        {"this", TokenType.THIS},
        {"true", TokenType.TRUE},
        {"var", TokenType.VAR},
        {"while", TokenType.WHILE}
    };


    public Lexer(string source, DiagnosticManager diagnosticManager)
    {
        _source = source;
        _diagnosticManager = diagnosticManager;
    }

    public List<Token> ScanTokens()
    {
        while (!IsAtEnd())
        {
            _startIndex = _currentIndex;
            _startColumn = _currentColumn;
            ScanToken();
        }

        AddToken(TokenType.EOF);
        return _tokens;
    }

    private void ScanToken()
    {
        char ch = Advance();

        switch (ch)
        {
            case '(': AddToken(TokenType.LEFT_PAREN); break;
            case ')': AddToken(TokenType.RIGHT_PAREN); break;
            case '{': AddToken(TokenType.LEFT_BRACE); break;
            case '}': AddToken(TokenType.RIGHT_BRACE); break;
            case ',': AddToken(TokenType.COMMA); break;
            case '.': AddToken(TokenType.DOT); break;
            case '-': AddToken(TokenType.MINUS); break;
            case '+': AddToken(TokenType.PLUS); break;
            case ';': AddToken(TokenType.SEMICOLON); break;
            case '*': AddToken(TokenType.STAR); break;
            case '!': AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG); break;
            case '=': AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL); break;
            case '<': AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS); break;
            case '>': AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER); break;
            case '/': HandleSlash(); break;
            case '"': HandleString(); break;

            case ' ':
            case '\r':
            case '\t':
                break;

            case '\n':
                _currentLine++;
                _currentColumn = 1;
                break;

            default:
                if (IsDigit(ch))
                {
                    HandleNumber();
                }
                else if (IsAlpha(ch))
                {
                    HandleIdentifier();
                }
                else
                {
                    _diagnosticManager.ReportBadCharacter(_currentLine, _startColumn, ch);
                }

                break;
        }
    }

    private bool IsAtEnd() => _currentIndex >= _source.Length;

    private char Advance()
    {
        _currentColumn++;
        return _source[_currentIndex++];
    }

    private void AddToken(TokenType type) => AddToken(type, null);

    private void AddToken(TokenType type, object? literal)
    {
        string lexeme = type == TokenType.EOF ? "" : _source.Substring(_startIndex, _currentIndex - _startIndex);
        int column = type == TokenType.EOF ? _currentColumn : _startColumn;

        _tokens.Add(new Token(type, lexeme, literal, _currentLine, column));
    }

    private bool Match(char expected)
    {
        if (IsAtEnd())
        {
            return false;
        }

        if (_source[_currentIndex] != expected)
        {
            return false;
        }

        _currentIndex++;
        return true;
    }

    private char Peek() => IsAtEnd() ? '\0' : _source[_currentIndex];

    private char PeekNext() => _currentIndex + 1 >= _source.Length ? '\0' : _source[_currentIndex + 1];

    private bool IsDigit(char ch) => ch >= '0' && ch <= '9';

    private bool IsAlpha(char ch) => (ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || ch == '_';

    private bool IsAlphaNumeric(char ch) => IsAlpha(ch) || IsDigit(ch);

    private void HandleSlash()
    {
        if (Match('/'))
        {
            while (Peek() != '\n' && !IsAtEnd())
            {
                Advance();
            }
        }
        else
        {
            AddToken(TokenType.SLASH);
        }
    }

    private void HandleString()
    {
        while (Peek() != '"' && !IsAtEnd())
        {
            if (Peek() == '\n')
            {
                _currentLine++;
            }
            Advance();
        }

        if (IsAtEnd())
        {
            _diagnosticManager.ReportUnterminatedString(_currentLine, _startColumn);
        }

        Advance();

        string value = _source.Substring(_startIndex + 1, _currentIndex - _startIndex - 2);
        AddToken(TokenType.STRING, value);
    }

    private void HandleNumber()
    {
        while (IsDigit(Peek()))
        {
            Advance();
        }

        if (Peek() == '.' && IsDigit(PeekNext()))
        {
            Advance();

            while (IsDigit(Peek()))
            {
                Advance();
            }
        }

        double value = double.Parse(_source.Substring(_startIndex, _currentIndex - _startIndex), System.Globalization.CultureInfo.InvariantCulture);
        AddToken(TokenType.NUMBER, value);
    }

    private void HandleIdentifier()
    {
        while (IsAlphaNumeric(Peek()))
        {
            Advance();
        }

        string text = _source.Substring(_startIndex, _currentIndex - _startIndex);
        TokenType type = Keywords.GetValueOrDefault(text, TokenType.IDENTIFIER);

        AddToken(type);
    }
}
