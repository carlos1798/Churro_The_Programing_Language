using System.Data.Common;

internal class Program
{
    public static void defineAst(String outputDir, String BaseName, List<String> types)
    {
        String savePath = outputDir + "/" + BaseName + ".cs";
        StreamWriter sw = new StreamWriter(savePath, false);
        sw.WriteLine("namespace Churro");
        sw.WriteLine("{");
        sw.WriteLine($"public abstract class {BaseName}");
        sw.WriteLine("{");

        sw.WriteLine("public abstract T  Accept<T>(IVisitor<T> visitor);");

        defineVisitor(sw, BaseName, types);

        foreach (String type in types)
        {
            string className = type.Split(':')[0].Trim();
            string fields = type.Split(":")[1].Trim();
            defineType(sw, BaseName, className, fields);
        }

        sw.WriteLine("}");
        sw.WriteLine("}");
        sw.WriteLine($"");
        //

        sw.Close();
    }

    private static void defineVisitor(StreamWriter sw, string baseName, List<string> types)
    {
        sw.WriteLine(" public interface IVisitor<T>{");
        foreach (String type in types)
        {
            string typeName = type.Split(":")[0].Trim();
            sw.WriteLine($"  T visit{typeName}{baseName}({typeName} {baseName.ToLower()});");
        }

        sw.WriteLine("}");
    }

    private static void defineType(StreamWriter sw, string baseName, string className, string fields)
    {
        sw.WriteLine($"public class {className} : {baseName}");
        sw.WriteLine("{");
        sw.WriteLine($"     public {className}({fields})");
        sw.WriteLine("{");

        string[] fieldArr = fields.Split(",");
        for (int i = 0; i < fieldArr.Length; i++)
        {
            string trimname = fieldArr[i].Trim();
            String name = trimname.Split(" ")[1];
            sw.WriteLine($"          this.{name} = {name};");
        }

        sw.WriteLine("}");
        for (int i = 0; i < fieldArr.Length; i++)
        {
            sw.WriteLine($"     public {fieldArr[i]};");
        }
        sw.WriteLine();
        //public override T Accept<T>(IVisitor<T> visitor)
        sw.WriteLine(" public override T Accept<T>(IVisitor<T> visitor){");
        sw.WriteLine($"return visitor.visit{className}{baseName}(this);");

        sw.WriteLine("}");
        sw.WriteLine("}");
        sw.WriteLine($"");
        //
    }

    private static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.WriteLine("Usage:Generate Ast <output_dir>");
            Environment.Exit(69);
        }
        String outputDir = args[0];
        List<String> types = new List<String>() {
                  "Binary   : Expr left, Token Operator, Expr right",
                  "Grouping : Expr expression",
                  "Literal  : Object value",
                  "Unary    : Token Operator, Expr right",
 "Variable : Token name",
   "Assign   : Token name, Expr value",
                };
        List<String> statements = new List<String>() {
             "Expression : Expr expression",
             "Print : Expr expression",
             "Var : Token name, Expr initializer",
             "Block : List<Stmt> statements",
             "If: Expr condition, Stmt thenBranch," +
                  " Stmt elseBranch",
        };
        defineAst(outputDir, "Stmt", statements);
    }
}