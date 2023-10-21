using Churro;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.Json;

internal class Scanner
{
    public List<Error> ErrorList;

    private string source;
    private List<Token> tokens;
    private int start = 1;
    private int current = 1;
    private int line = 1;

    public Scanner(string source)
    {
        this.source = source;
        this.tokens = new List<Token>();
        this.ErrorList = new List<Error>();
    }

    private bool IsAtEnd()
    {
        return current >= source.Length;
    }

    public List<Token> scanTokens()
    {
        while (!IsAtEnd())
        {
            start = current;
            ScanToken();
        }

        tokens.Add(new Token(Token.TokenType.EOF, "", null, line));
        return tokens;
    }

    private void ScanToken()
    {
        char c = Advance();
        switch (c)
        {
            case '(': AddToken(Token.TokenType.LEFT_PAREN); break;
            case ')': AddToken(Token.TokenType.RIGHT_PAREN); break;
            case '{': AddToken(Token.TokenType.LEFT_BRACE); break;
            case '}': AddToken(Token.TokenType.RIGHT_BRACE); break;

            case ',': AddToken(Token.TokenType.COMMA); break;
            case '.': AddToken(Token.TokenType.DOT); break;
            case ';': AddToken(Token.TokenType.SEMICOLON); break;

            case '+': AddToken(Token.TokenType.PLUS); break;
            case '-': AddToken(Token.TokenType.MINUS); break;
            case '*': AddToken(Token.TokenType.STAR); break;

            case '!': AddToken(Match('=') ? Token.TokenType.BANG_EQUAL : Token.TokenType.BANG); break;
            case '=': AddToken(Match('=') ? Token.TokenType.EQUAL_EQUAL : Token.TokenType.EQUAL); break;
            case '>': AddToken(Match('=') ? Token.TokenType.GREATER_EQUAL : Token.TokenType.GREATER); break;
            case '<': AddToken(Match('=') ? Token.TokenType.LESS_EQUAL : Token.TokenType.LESS); break;
            case '/':
                Comment()
                    ; break;
            case ' ':
            case '\r':
            case '\t':
                // Ignore whitespace.
                break;

            case '\n':
                line++;
                break;

            case '"':
                String()
                ; break;
            default:

                if (Char.IsNumber(c))
                {
                    Number();
                }
                else if (IsAlpha(c))
                {
                    Identifier();
                }
                else
                {
                    ErrorList.Add(new Error(line, $"UNEXPEDTED CHARACTER.{c}"));
                }
                break;
        }
    }

    private char Peek()
    {
        if (IsAtEnd()) { return '\0'; };
        return source.ElementAt(current);
    }

    private bool Match(char expected)
    {
        if (IsAtEnd()) return false;
        if (source.ElementAt(current) != expected) return false;
        current++;
        return true;
    }

    private void AddToken(Token.TokenType type)
    {
        AddToken(type, null);
    }

    private void AddToken(Token.TokenType type, Object? literal)
    {
        string text = source.Substring(start, current - start);
        tokens.Add(new Token(type, text, literal, line));
    }

    private char Advance()
    {
        return source.ElementAt(current++);
    }

    private void String()
    {
        while (Peek() != '"' && !IsAtEnd())
        {
            if (Peek() == '\n') line++;
            Advance();
        }
        if (IsAtEnd())
        {
            ErrorList.Add(new Error(line, $"UNDERTERMINATE STRING"));
        }

        Advance();
        string value = source.Substring(start + 1, current - start - 2);
        AddToken(Token.TokenType.STRING, value);
    }

    private void Number()
    {
        while (Char.IsNumber(Peek())) Advance();
        if (Peek() == '.' && char.IsNumber(PeekNext()))
        {
            Advance();

            while (Char.IsNumber(Peek())) Advance();
        }
        AddToken(Token.TokenType.NUMBER, double.Parse(source[start..current], CultureInfo.InvariantCulture));
    }

    private void Comment()
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
            AddToken(Token.TokenType.SLASH);
        }
    }

    private char PeekNext()
    {
        if (current + 1 >= source.Length) return '\0';
        return source.ElementAt(current + 1);
    }

    private void Identifier()
    {
        while (IsAlphaNumeric(Peek())) Advance();
        string text = source[start..current];
        Token.TokenType type;
        if (!Keywords.TryGetValue(text, out type)) { type = Token.TokenType.IDENTIFIER; };

        AddToken(type);
    }

    private bool IsAlpha(char c) => (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c == '_');

    private bool IsAlphaNumeric(char c) => IsAlpha(c) || isDigit(c);

    private bool isDigit(char c) => Char.IsDigit(c);

    private static readonly Dictionary<string, Token.TokenType> Keywords = new() {
        { "and",Token.TokenType.AND},
        { "class",Token.TokenType.CLASS},
        { "else",Token.TokenType.ELSE},
        { "false" , Token.TokenType.FALSE},
        { "for",Token.TokenType.FOR},
        { "fun",Token.TokenType.FUN},
        { "if",Token.TokenType.IF},
        { "null",Token.TokenType.NULL},
        { "or",Token.TokenType.OR},
        { "print",Token.TokenType.PRINT},
        { "return",Token.TokenType.RETURN},
        { "super",Token.TokenType.SUPER},
        { "this",Token.TokenType.THIS},
        { "true",Token.TokenType.TRUE},
        { "var",Token.TokenType.VAR},
        { "while",Token.TokenType.WHILE},
    };
}