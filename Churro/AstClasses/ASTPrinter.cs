﻿using Churro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AST_Class_Generator
{
    internal class ASTPrinter : Expr.IVisitor<string>
    {
        public string print(Expr expr)
        {
            return expr.Accept(this);
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

        private string parenthesize(String name, params Expr[] exprs)
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