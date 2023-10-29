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
    }
}