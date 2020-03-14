using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LangComparison.Cs
{
    public class Term
    {
        public int Coef { get; set; }
        public int Power { get; set; }
    }

    public class Poly
    {
        public List<Term> Terms { get; set; }
    }

    public class Reader
    {
        public (Poly First, Poly Second) Read(string filename)
        {
            using var reader = new StreamReader(filename);
            var jr = new JsonTextReader(reader);
            return new JsonSerializer().Deserialize<(Poly, Poly)>(jr);
        }
    }

    public class Writer
    {
        private string Format(Dictionary<int, int> result)
        {
            if (result.Count == 0 || result.All(e => e.Value == 0))
            {
                return "0";
            }
            var builder = new StringBuilder();
            foreach (var (power, coef) in result.OrderByDescending(r => r.Key))
            {
                if (coef == 0)
                {
                    continue;
                }
                if (coef > 0)
                {
                    builder.Append("+");
                }
                if (coef != 1)
                {
                    builder.Append(coef);
                }
                if (power == 0)
                {
                    continue;
                }
                builder.Append("x");
                if (power != 1)
                {
                    builder.Append("^");
                    if (power < 0)
                    {
                        builder.Append("(");
                    }
                    builder.Append(power);
                    if (power < 0)
                    {
                        builder.Append(")");
                    }
                }
            }
            return builder.ToString().TrimStart('+');
        }

        public void Write(Dictionary<int, int> result) => Console.WriteLine($"Result = {Format(result)}\n");
    }

    public class CsImplementation
    {
        public Dictionary<int, int> Run(Poly p1, Poly p2)
        {
            if (p1.Terms.Count == 0 || p2.Terms.Count == 0)
            {
                return new Dictionary<int, int>();
            }
            var max = Math.Max(p1.Terms.Max(t => t.Power), p2.Terms.Max(t => t.Power));
            var result = new Dictionary<int, int>();
            foreach (var term1 in p1.Terms)
            {
                foreach (var term2 in p2.Terms)
                {
                    var power = term1.Power + term2.Power;
                    var coef = term1.Coef * term2.Coef;
                    if (result.ContainsKey(power))
                    {
                        result[power] += coef;
                    }
                    else
                    {
                        result.Add(power, coef);
                    }
                }
            }
            return result;
        }
    }

    public class Menu
    {
        public string SelectFilename()
        {
            var files = Directory.GetFiles(@".\", "*.json", SearchOption.AllDirectories);
            Console.WriteLine("Select test file:\n");
            for (var i = 0; i < files.Length; i++)
            {
                var name = files[i]
                    .TrimStart('.', '\\')
                    .Replace(@"\", " -> ");
                Console.WriteLine($"\t[{i + 1}] {name}");
            }
            Console.WriteLine();
            var n = int.Parse(Console.ReadLine());
            return files[n - 1];
        }
    }

    internal static class Program
    {
        private static void Main()
        {
            var menu = new Menu();
            var reader = new Reader();
            var writer = new Writer();
            Directory.SetCurrentDirectory(@"..\..\..\..\tests");
            while (true)
            {
                var file = menu.SelectFilename();
                var (p1, p2) = reader.Read(file);
                var result = new CsImplementation().Run(p1, p2);
                writer.Write(result);
            }
        }
    }
}
