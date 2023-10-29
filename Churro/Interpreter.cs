using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Churro
{
    internal class Interpreter : Expr.IVisitor<Object>, Stmt.IVisitor<Object>
    {
        public void Interpret(List<Stmt> statements)
        {
            try
            {
                foreach (Stmt stmt in statements)
                {
                    Execute(stmt);
                }
            }
            catch (RuntimeError ex)
            {
                Churro.runtimeError(ex);
            }
        }

        private void Execute(Stmt stmt)
        {
            stmt.Accept(this);
        }

        public object visitGroupingExpr(Expr.Grouping expr)
        {
            return Evaluate(expr.expression);
        }

        public object visitLiteralExpr(Expr.Literal expr)
        {
            return expr.value;
        }

        public object visitUnaryExpr(Expr.Unary expr)
        {
            Object right = Evaluate(expr.right);
            switch (expr.Operator.Type)
            {
                case Token.TokenType.MINUS:
                    {
                        checkNumberOperand(expr.Operator, right);
                        return -(double)right;
                    }
                case Token.TokenType.BANG:
                    {
                        return !IsTruthy(right);
                    }
            }
            //Unreachable
            return null;
        }

        public object visitBinaryExpr(Expr.Binary expr)
        {
            Object right = Evaluate(expr.right);
            Object left = Evaluate(expr.left);

            switch (expr.Operator.Type)
            {
                case Token.TokenType.MINUS:
                    checkNumberOperands(expr.Operator, left, right);
                    return (double)left - (double)right;

                case Token.TokenType.STAR:

                    checkNumberOperands(expr.Operator, left, right);
                    return (double)left * (double)right;

                case Token.TokenType.SLASH:
                    checkNumberOperands(expr.Operator, left, right);
                    return (double)left / (double)right;

                case Token.TokenType.PLUS:
                    if (left is Double && right is Double)
                    {
                        return (double)left + (double)right;
                    }; if (left is String && right is String)
                    {
                        return (String)left + (String)right;
                    };
                    throw new RuntimeError(expr.Operator, "Operands must be the same type");
                    break;

                case Token.TokenType.GREATER:

                    checkNumberOperands(expr.Operator, left, right);
                    return (double)left > (double)right;

                case Token.TokenType.GREATER_EQUAL:
                    checkNumberOperands(expr.Operator, left, right);
                    return (double)left >= (double)right;

                case Token.TokenType.LESS:
                    checkNumberOperands(expr.Operator, left, right);
                    return (double)left < (double)right;

                case Token.TokenType.LESS_EQUAL:
                    checkNumberOperands(expr.Operator, left, right);
                    return (double)left <= (double)right;

                case Token.TokenType.BANG_EQUAL: return !IsEqual(left, right);
                case Token.TokenType.EQUAL_EQUAL: return IsEqual(left, right);
            }
            return null;
        }

        public object visitExpressionStmt(Stmt.Expression stmt)
        {
            Evaluate(stmt.expression);
            return null;
        }

        public object visitPrintStmt(Stmt.Print stmt)
        {
            Object value = Evaluate(stmt.expression);
            Console.WriteLine(Stringify(value));
            return null;
        }

        #region "Utils"

        private string Stringify(object value)
        {
            if (value == null) return "null";
            if (value is Double)
            {
                String text = value.ToString();
                if (text.Last().Equals(".0"))
                {
                    text = text.Substring(0, text.Length - 2);
                }
                return text;
            }
            return value.ToString();
        }

        private bool IsTruthy(object right)
        {
            if (right == null)
            {
                return false;
            }
            if (right is Boolean) { return (bool)right; };
            return true;
        }

        private object Evaluate(Expr expression)
        {
            return expression.Accept(this);
        }

        private bool IsEqual(object left, object right)
        {
            if (right == null && left == null) return true;
            if (left == null) return false;
            return left.Equals(right);
        }

        private void checkNumberOperand(Token Operator, object operand)
        {
            if (operand is Double) return;
            throw new RuntimeError(Operator, "Operand must be a number");
        }

        private void checkNumberOperands(Token @operator, object left, object right)
        {
            if (left is Double && right is Double) return;

            throw new RuntimeError(@operator, "Operands must be a number");
        }

        #endregion "Utils"
    }
}