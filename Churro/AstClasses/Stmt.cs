namespace Churro.AstClasses
{
    public abstract class Stmt
    {
        public abstract T Accept<T>(IVisitor<T> visitor);

        public interface IVisitor<T>
        {
            T visitExpressionStmt(Expression stmt);

            T visitPrintStmt(Print stmt);
        }

        public class Expression : Stmt
        {
            public Expression(Expr expression)
            {
                this.expression = expression;
            }

            public Expr expression;

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.visitExpressionStmt(this);
            }
        }

        public class Print : Stmt
        {
            public Print(Expr expression)
            {
                this.expression = expression;
            }

            public Expr expression;

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.visitPrintStmt(this);
            }
        }
    }
}