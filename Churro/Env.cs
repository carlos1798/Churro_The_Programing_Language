using Churro.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Churro
{
    internal class Env
    {
        private Env enclosing;
        private Dictionary<string, Object> values = new();

        public Env()
        {
            enclosing = null;
        }

        public Env(Env enclosing)
        {
            this.enclosing = enclosing;
        }

        public void Define(string key, object value)
        {
            values.Add(key, value);
        }

        public Object Get(Token key)
        {
            if (values.ContainsKey(key.Lexeme))
            {
                return values[key.Lexeme];
            }
            if (enclosing != null)
            {
                return enclosing.Get(key);
            }
            throw new RuntimeError(key, $"Undefined variable {key.Lexeme}");
        }

        internal void Assign(Token name, object value)
        {
            if (values.ContainsKey(name.Lexeme))
            {
                values[name.Lexeme] = value;
                return;
            }
            if (enclosing != null)
            {
                enclosing.Assign(name, value);
            }
            throw new RuntimeError(name, $"Undefined variable {name.Lexeme}");
        }
    }
}