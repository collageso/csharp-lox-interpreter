namespace LoxInterpreter.Scanning;

using LoxInterpreter.Reporting;
using LoxInterpreter.Abstractions;

public class Lexer
{
    private readonly string _source;
    private readonly List<Token> _tokens = new();
    private readonly DiagnosticManager _diagnosticManager;

    private int _start = 0;
    private int _current = 0;
    private int _currentLine = 1;
    private int _startColumn = 1;
    private int _currentColumn = 1;


    public Lexer(string source, DiagnosticManager diagnosticManager)
    {
        _source = source;
        _diagnosticManager = diagnosticManager;
    }

    public List<Token> ScanTokens()
    {
        while (!IsAtEnd())
        {
            _start = _current;
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

            case ' ':
            case '\r':
            case '\t':
                break;

            case '\n':
                _currentLine++;
                _currentColumn = 1;
                break;

            default:
                _diagnosticManager.ReportBadCharacter(_currentLine, _startColumn, ch);
                break;
        }
    }

    private bool IsAtEnd() => _current >= _source.Length;

    private char Advance()
    {
        _currentColumn++;
        return _source[_current++];
    }

    private void AddToken(TokenType type) => AddToken(type, null);

    private void AddToken(TokenType type, object? literal)
    {
        string lexeme = type == TokenType.EOF ? "" : _source.Substring(_start, _current - _start);
        int column = type == TokenType.EOF ? _currentColumn : _startColumn;

        _tokens.Add(new Token(type, lexeme, literal, _currentLine, column));
    }
}
