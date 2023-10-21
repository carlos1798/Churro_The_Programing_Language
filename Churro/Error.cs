using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Churro
{
    public class Error
    {
        public Error(int line, string? message)
        {
            Line = line;
            Message = message;
        }

        public int Line { get; set; }
        public string? Message { get; set; }

        public void report(string where)
        {
            Console.WriteLine($"[line {this.Line}] Error {where} : {Message}");
        }
    }
}