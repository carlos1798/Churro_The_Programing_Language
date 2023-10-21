namespace Churro
{
    public class Token
    {
        private TokenType type;
        private string lexeme;
        private object? literal;
        private int line;

        public Token(TokenType type, string lexeme, Object? literal, int line)
        {
            Type = type;
            Lexeme = lexeme;
            Literal = literal;
            Line = line;
        }

        public int Line { get => line; set => line = value; }
        public object? Literal { get => literal; set => literal = value; }
        public string Lexeme { get => lexeme; set => lexeme = value; }
        public TokenType Type { get => type; set => type = value; }

        public override bool Equals(object? obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string? ToString()
        {
            return $"Type:{type} | Text:{lexeme} | Literal:{literal}";
        }

        public enum TokenType
        {
            //Single character tokens
            LEFT_PAREN,

            RIGHT_PAREN,
            LEFT_BRACE,
            RIGHT_BRACE,
            COMMA,
            DOT,
            MINUS,
            PLUS,
            SEMICOLON,
            SLASH,
            STAR,

            //One or two character tokens
            BANG,

            BANG_EQUAL,
            EQUAL,
            EQUAL_EQUAL,
            GREATER,
            GREATER_EQUAL,
            LESS,
            LESS_EQUAL,

            //Literals
            IDENTIFIER,

            STRING,
            NUMBER,

            //Keywords
            AND,

            OR,
            CLASS,
            ELSE,
            FALSE,
            FUN,
            FOR,
            IF,
            NULL,
            PRINT,
            RETURN,
            SUPER,
            THIS,
            TRUE,
            VAR,
            WHILE,

            EOF
        }
    }
}