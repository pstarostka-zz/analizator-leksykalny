using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AnalizatorLeksykalny
{
    class Program
    {
        static void Main(string[] args)
        {
            Analyzer analyzer = new Analyzer();
            analyzer.AnalyzeLexically(GetFileText());
            Console.ReadLine();
        }

        static string GetFileText()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetManifestResourceNames()
                .Single(str => str.EndsWith("example.txt"));

            string result = "";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                result += reader.ReadToEnd();
            }
            return result;
        }
    }
}
