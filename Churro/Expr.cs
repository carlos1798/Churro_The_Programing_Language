namespace Churro
{
    public abstract class Expr
    {
        public abstract T Accept<T>(IVisitor<T> visitor);

        public interface IVisitor<T>
        {
            T visitBinaryExpr(Binary expr);

            T visitGroupingExpr(Grouping expr);

            T visitLiteralExpr(Literal expr);

            T visitUnaryExpr(Unary expr);

            T visitVariableExpr(Variable expr);
        }

        public class Binary : Expr
        {
            public Binary(Expr left, Token Operator, Expr right)
            {
                this.left = left;
                this.Operator = Operator;
                this.right = right;
            }

            public Expr left;
            public Token Operator;
            public Expr right;

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.visitBinaryExpr(this);
            }
        }

        public class Grouping : Expr
        {
            public Grouping(Expr expression)
            {
                this.expression = expression;
            }

            public Expr expression;

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.visitGroupingExpr(this);
            }
        }

        public class Literal : Expr
        {
            public Literal(Object value)
            {
                this.value = value;
            }

            public Object value;

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.visitLiteralExpr(this);
            }
        }

        public class Unary : Expr
        {
            public Unary(Token Operator, Expr right)
            {
                this.Operator = Operator;
                this.right = right;
            }

            public Token Operator;
            public Expr right;

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.visitUnaryExpr(this);
            }
        }

        public class Variable : Expr
        {
            public Variable(Token name)
            {
                this.name = name;
            }

            public Token name;

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.visitVariableExpr(this);
            }
        }
    }
}