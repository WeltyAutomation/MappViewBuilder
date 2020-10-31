using System;
using System.IO;
using System.Linq;
using FunctionParser;
using MappViewContentGenerator;

namespace FunctionDiagramGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var output = "C://Temp/Content.txt";
            var path = string.Empty;

            while (string.IsNullOrWhiteSpace(path))
            {
                Console.WriteLine("Enter the path to a .fun file to turn into mappview content:");
                path = Console.ReadLine()?.Trim('"') ?? string.Empty;
            }

            var funContent = File.ReadAllText(path);

            var fbks = Fbk.ParseFunctionFile(funContent);

            var fbkContents = fbks.Select(f => new FbkContentElement(f));

            var lines = fbkContents.SelectMany(f => f.ToContent()).ToArray();

            File.WriteAllLines(output, lines); 
        }
    }
}
