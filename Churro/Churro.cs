using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Churro
{
    internal class Churro
    {
        private static bool hadError = false;

        private static void Main(string[] args)
        {
            if (args.Count() > 1)
            {
                Console.WriteLine($"Usage: churro [script]");
                Environment.Exit(69);
            }
            else if (args.Count() == 1)
            {
                runFile(args[0]);
            }
            else
            {
                runPrompt();
            }
        }

        private static void runPrompt()
        {
            while (true)
            {
                Console.Write(">");
                try
                {
                    string source = Console.ReadLine();
                    if (!String.IsNullOrEmpty(source))
                    {
                        run(source);
                        hadError = false;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        private static void runFile(string v)
        {
            byte[] bytes = File.ReadAllBytes(Path.GetFullPath(v));
            string file = System.Text.Encoding.Default.GetString(bytes);
            run(file);
            if (hadError)
            {
                Environment.Exit(65);
            }
        }

        private static void run(string source)
        {
            Scanner scanner = new(source);
            List<Token> list = scanner.scanTokens();
            foreach (Token token in list)
            {
                Console.WriteLine($"{token.ToString()}");
            }
            scanner.ErrorList.ForEach(l => l.report("run()"));
        }
    }
}