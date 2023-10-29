using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Churro.AstClasses
{
    internal class ASTPrinter : Expr.IVisitor<string>
    {
        public string print(Expr expr)
        {
            return expr.Accept(this);
        }

        public string visitAssignExpr(Expr.Assign expr)
        {
            throw new NotImplementedException();
        }

        public string visitBinaryExpr(Expr.Binary expr)
        {
            return parenthesize(expr.Operator.Lexeme, expr.right, expr.left);
        }

        public string visitGroupingExpr(Expr.Grouping expr)
        {
            return parenthesize("group", expr.expression);
        }

        public string visitLiteralExpr(Expr.Literal expr)
        {
            if (expr.value == null) return "null";
            return expr.value.ToString();
        }

        public string visitUnaryExpr(Expr.Unary expr)
        {
            return parenthesize(expr.Operator.Lexeme, expr.right);
        }

        public string visitVariableExpr(Expr.Variable expr)
        {
            throw new NotImplementedException();
        }

        private string parenthesize(string name, params Expr[] exprs)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append('(').Append(name);
            foreach (Expr Expr in exprs)
            {
                builder.Append(" ");
                builder.Append(print(Expr));
            }
            builder.Append(")");
            return builder.ToString();
        }
    }
}