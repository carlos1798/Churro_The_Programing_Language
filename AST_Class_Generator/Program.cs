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
          "Unary    : Token Operator, Expr right"
        };
        defineAst(outputDir, "Expr", types);
    }
}