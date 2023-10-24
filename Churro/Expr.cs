namespace Churro
{
    public abstract class Expr
    {
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
        }

        public class Grouping : Expr
        {
            public Grouping(Expr expression)
            {
                this.expression = expression;
            }

            public Expr expression;
        }

        public class Literal : Expr
        {
            public Literal(Object value)
            {
                this.value = value;
            }

            public Object value;
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
        }
    }
}