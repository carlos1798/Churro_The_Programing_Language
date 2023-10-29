using Churro.Errors;

namespace Churro
{
    internal class Parser
    {
        private List<Token> tokens = new();
        private int current = 0;

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        private Token Advance()
        {
            if (!IsAtEnd()) current++;
            return Previous();
        }

        private bool Check(Token.TokenType tokenType)
        {
            if (IsAtEnd()) return false;
            return Peek().Type == tokenType;
        }

        private Token Peek()
        {
            return tokens[current];
        }

        private bool IsAtEnd()
        {
            return Peek().Type == Token.TokenType.EOF;
        }

        private bool Match(params Token.TokenType[] types)
        {
            for (int i = 0; i < types.Length; i++)
            {
                if (Check(types[i]))
                {
                    Advance();
                    return true;
                }
            }

            return false;
        }

        private Token Previous()
        {
            return tokens[current - 1];
        }

        private Token? Consume(Token.TokenType tokenType, string v)
        {
            if (Check(tokenType))
            {
                return Advance();
            }
            throw Error(Peek(), v);
        }

        public List<Stmt> Parse()
        {
            List<Stmt> statements = new List<Stmt>();
            while (!IsAtEnd())
            {
                statements.Add(Declaration());
            };
            return statements;
        }

        private Stmt Declaration()
        {
            try
            {
                if (Match(Token.TokenType.VAR)) { return VarDeclaration(); }
                return Statement();
            }
            catch (ParserError e)
            {
                Synchronize();
                return null;
            }
        }

        private Stmt VarDeclaration()
        {
            Token name = Consume(Token.TokenType.IDENTIFIER, "Expect Variable name");
            Expr initializer = null;
            if (Match(Token.TokenType.EQUAL))
            {
                initializer = Expression();
            }
            Consume(Token.TokenType.SEMICOLON, "Expect ; at the en of declaration");
            return new Stmt.Var(name, initializer);
        }

        private Stmt Statement()
        {
            if (Match(Token.TokenType.PRINT))
            {
                return PrintStatement();
            }
            return ExpressionStatement();
        }

        private Stmt ExpressionStatement()
        {
            Expr Value = Expression();
            Consume(Token.TokenType.SEMICOLON, "Expect ; at the end of statement");
            return new Stmt.Expression(Value);
        }

        private Stmt PrintStatement()
        {
            Expr Value = Expression();
            Consume(Token.TokenType.SEMICOLON, "Expect ; at the end of statement");
            return new Stmt.Print(Value);
        }

        private Expr Expression()
        { return Equality(); }

        private Expr Equality()
        {
            Expr expr = Comparison();
            while (Match(Token.TokenType.BANG_EQUAL, Token.TokenType.EQUAL_EQUAL))
            {
                Token Operator = Previous();
                Expr right = Comparison();
                expr = new Expr.Binary(expr, Operator, right);
            }
            return expr;
        }

        private Expr Comparison()
        {
            Expr expr = Term();

            while (Match(Token.TokenType.LESS, Token.TokenType.GREATER, Token.TokenType.GREATER_EQUAL, Token.TokenType.LESS_EQUAL))
            {
                Token token = Previous();
                Expr right = Term();
                expr = new Expr.Binary(expr, token, right);
            }
            return expr;
        }

        private Expr Term()
        {
            Expr expr = Factor();
            while (Match(Token.TokenType.MINUS, Token.TokenType.PLUS))
            {
                Token token = Previous();
                Expr right = Factor();
                expr = new Expr.Binary(expr, token, right);
            }
            return expr;
        }

        private Expr Factor()
        {
            Expr expr = Unary();
            while (Match(Token.TokenType.SLASH, Token.TokenType.STAR))
            {
                Token token = Previous();
                Expr right = Unary();
                expr = new Expr.Binary(expr, token, right);
            }
            return expr;
        }

        private Expr Unary()
        {
            if (Match(Token.TokenType.BANG, Token.TokenType.SLASH))
            {
                Token Operator = Previous();
                Expr right = Unary();
                return new Expr.Unary(Operator, right);
            }
            return Primary();
        }

        private Expr? Primary()
        {
            if (Match(Token.TokenType.NUMBER, Token.TokenType.STRING))
            { return new Expr.Literal(Previous().Literal); };

            if (Match(Token.TokenType.NULL)) { return new Expr.Literal(null); };
            if (Match(Token.TokenType.FALSE)) { return new Expr.Literal(false); };
            if (Match(Token.TokenType.TRUE)) { return new Expr.Literal(true); };

            if (Match(Token.TokenType.IDENTIFIER)) { return new Expr.Variable(Previous()); }
            if (Match(Token.TokenType.LEFT_PAREN))
            {
                Expr expr = Expression();
                Consume(Token.TokenType.RIGHT_PAREN, "Expect ')' after expression");
                return new Expr.Grouping(expr);
            }
            throw Error(Peek(), "Expect expression");
        }

        private Exception Error(Token token, string v)
        {
            ErrorHandling.Error(token, v);
            return new ParserError();
        }

        private void Synchronize()
        {
            Advance();
            while (!IsAtEnd())
            {
                if (Previous().Type == Token.TokenType.SEMICOLON) return;
                switch (Peek().Type)
                {
                    case Token.TokenType.CLASS:
                    case Token.TokenType.FUN:
                    case Token.TokenType.VAR:
                    case Token.TokenType.FOR:
                    case Token.TokenType.IF:
                    case Token.TokenType.WHILE:
                    case Token.TokenType.PRINT:
                    case Token.TokenType.RETURN:
                        return;
                }
                Advance();
            }
        }
    }
}