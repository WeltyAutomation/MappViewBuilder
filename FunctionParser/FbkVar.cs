using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace FunctionParser.Tests
{
    public class FbkVar
    {
        public FbkVar(string variableText, FbkVarCategory category)
        {
            Console.WriteLine($"{category}, {variableText}");
            var (name, type) = ParseVariable(variableText);
            Name = name;
            Type = type;
            Category = category;
        }

        public string Name;
        public string Type;
        public FbkVarCategory Category;

        public static (string name, string type) ParseVariable(string variableText)
        {
            var match = Regex.Match(variableText, @"^(\w+).*\W(\w+);$");
            return match.Success ? (match.Groups[1].Value, match.Groups[2].Value) : (string.Empty, string.Empty);
        }

        public static FbkVarCategory GetVariableCategory(string variableGroupText)
        {
            var header = variableGroupText.Split(',').FirstOrDefault();

            return string.IsNullOrWhiteSpace(header) ? FbkVarCategory.Other :
                header.Equals("VAR_INPUT") ? FbkVarCategory.Input :
                header.Equals("VAR_OUTPUT") ? FbkVarCategory.Output :
                header.Equals("VAR") ? FbkVarCategory.Local : FbkVarCategory.Other;
        }

        public static string[] GetVariableDefinitions(string variableGroupText)
        {
            return variableGroupText.Split(',').Where(s => !s.StartsWith("VAR")).ToArray();
        }
    }
}