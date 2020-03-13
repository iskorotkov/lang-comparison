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
        public string Filename { get; set; }
        public (Poly First, Poly Second) Read()
        {
            using var reader = new StreamReader(Filename);
            var jr = new JsonTextReader(reader);
            return new JsonSerializer().Deserialize<(Poly, Poly)>(jr);
        }
    }

    public class Writer
    {
        public void Write(Dictionary<int, int> result)
        {
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
                    builder.Append(power);
                }
            }
            Console.WriteLine(builder.ToString().TrimStart('+'));
        }
    }

    public class CsImplementation
    {
        public void Run(Reader reader, Writer writer)
        {
            var (p1, p2) = reader?.Read() ?? throw new InvalidDataException();
            p1.Terms = p1.Terms.OrderBy(t => t.Power).ToList();
            var max = Math.Max(p1.Terms.Last().Power, p2.Terms.Max(t => t.Power));
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
            writer?.Write(result);
        }
    }

    public class Menu
    {
        public string SelectFilename()
        {
            Directory.SetCurrentDirectory(@"..\..\..\..\tests");
            var files = Directory.GetFiles(@".\", "*.json", SearchOption.AllDirectories);
            Console.WriteLine("Select test file:\n");
            for (var i = 0; i < files.Length; i++)
            {
                Console.WriteLine($"\t[{i + 1}] {files[i].TrimStart('.', '\\')}");
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
            var file = new Menu().SelectFilename();
            new CsImplementation().Run(new Reader { Filename = file }, new Writer());
        }
    }
}
