using Churro.Errors;
using Microsoft.VisualBasic;

namespace Churro
{
    internal class Parser
    {
        private List<Token> tokens = new();
        private int current = 0;

        #region "Utils"

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

        #endregion "Utils"

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
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
            else if (Match(Token.TokenType.LEFT_BRACE))
            {
                return new Stmt.Block(Block());
            }
            else if (Match(Token.TokenType.IF))
            {
                return IfStatement();
            }
            else if (Match(Token.TokenType.WHILE))
            {
                return WhileStatement();
            }
            return ExpressionStatement();
        }

        private Stmt WhileStatement()
        {
            Consume(Token.TokenType.LEFT_PAREN, "Expect ( after while");
            Expr condition = Expression();
            Consume(Token.TokenType.RIGHT_PAREN, "Expect ) after condition");
            Stmt body = Statement();
            return new Stmt.While(condition, body);
        }

        private Stmt IfStatement()
        {
            Consume(Token.TokenType.LEFT_PAREN, "Expect ( after if");
            Expr condition = Expression();
            Consume(Token.TokenType.RIGHT_PAREN, "Expect ) after condition");

            Stmt thenBranch = Statement();
            Stmt elseBranch = null;
            if (Match(Token.TokenType.ELSE))
            {
                elseBranch = Statement();
            }
            return new Stmt.If(condition, thenBranch, elseBranch);
        }

        private List<Stmt> Block()
        {
            List<Stmt> statements = new();
            while (!Check(Token.TokenType.RIGHT_BRACE) && !IsAtEnd())
            {
                statements.Add(Declaration());
            }
            Consume(Token.TokenType.RIGHT_BRACE, "Expect } after block");
            return statements;
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
        { return Assignment(); }

        private Expr Assignment()
        {
            Expr expr = Or();
            if (Match(Token.TokenType.EQUAL))
            {
                Token equals = Previous();
                Expr value = Assignment();
                if (expr is Expr.Variable)
                {
                    Token name = ((Expr.Variable)expr).name;
                    return new Expr.Assign(name, value);
                }
                Error(equals, "Invalid assignment target");
            }
            return expr;
        }

        private Expr Or()
        {
            Expr expr = And();
            while (Match(Token.TokenType.OR))
            {
                Token Operator = Previous();
                Expr right = And();
                expr = new Expr.Logical(expr, Operator, right);
            }
            return expr;
        }

        private Expr And()
        {
            Expr expr = Equality();
            while (Match(Token.TokenType.AND))
            {
                Token Operator = Previous();
                Expr right = Equality();
                expr = new Expr.Logical(expr, Operator, right);
            }
            return expr;
        }

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