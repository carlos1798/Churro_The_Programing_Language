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
        private Dictionary<string, Object> values = new();

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
            throw new RuntimeError(key, $"Undefined variable {key.Lexeme}");
        }

        internal void Assign(Token name, object value)
        {
            if (values.ContainsKey(name.Lexeme))
            {
                values[name.Lexeme] = value;
                return;
            }
            throw new RuntimeError(name, $"Undefined variable {name.Lexeme}");
        }
    }
}