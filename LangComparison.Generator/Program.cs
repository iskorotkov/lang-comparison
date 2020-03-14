using LangComparison.Cs;
using Newtonsoft.Json;
using System;

namespace LangComparison.Generator
{
    public class PolyGenerator
    {
        public (Poly First, Poly Second) Generate(int length,
            (int Min, int Max) coefRange, (int Min, int Max) powerRange)
        {
            var random = new Random();
            var p1 = new Poly();
            var p2 = new Poly();
            for (var i = 0; i < length; i++)
            {
                p1.Terms.Add(new Term
                {
                    Coef = random.Next(coefRange.Min, coefRange.Max),
                    Power = random.Next(powerRange.Min, powerRange.Max)
                });
                p2.Terms.Add(new Term
                {
                    Coef = random.Next(coefRange.Min, coefRange.Max),
                    Power = random.Next(powerRange.Min, powerRange.Max)
                });
            }
            return (p1, p2);
        }
    }

    internal static class Program
    {
        private static void Main()
        {
            var generator = new PolyGenerator();
            var result = generator.Generate(5000, (-10000, 10000), (-10000, 10000));
            var json = JsonConvert.SerializeObject(result);
            Console.WriteLine(json);
        }
    }
}
