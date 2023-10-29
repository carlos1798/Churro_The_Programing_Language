namespace Churro
{
    public abstract class Stmt
    {
        public abstract T Accept<T>(IVisitor<T> visitor);

        public interface IVisitor<T>
        {
            T visitExpressionStmt(Expression stmt);

            T visitPrintStmt(Print stmt);

            T visitVarStmt(Var stmt);

            T visitBlockStmt(Block stmt);

            T visitIfStmt(If stmt);

            T visitWhileStmt(While stmt);
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

        public class Var : Stmt
        {
            public Var(Token name, Expr initializer)
            {
                this.name = name;
                this.initializer = initializer;
            }

            public Token name;
            public Expr initializer;

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.visitVarStmt(this);
            }
        }

        public class Block : Stmt
        {
            public Block(List<Stmt> statements)
            {
                this.statements = statements;
            }

            public List<Stmt> statements;

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.visitBlockStmt(this);
            }
        }

        public class If : Stmt
        {
            public If(Expr condition, Stmt thenBranch, Stmt elseBranch)
            {
                this.condition = condition;
                this.thenBranch = thenBranch;
                this.elseBranch = elseBranch;
            }

            public Expr condition;
            public Stmt thenBranch;
            public Stmt elseBranch;

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.visitIfStmt(this);
            }
        }

        public class While : Stmt
        {
            public While(Expr condition, Stmt body)
            {
                this.condition = condition;
                this.body = body;
            }

            public Expr condition;
            public Stmt body;

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.visitWhileStmt(this);
            }
        }
    }
}