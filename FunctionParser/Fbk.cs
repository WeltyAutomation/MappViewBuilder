using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FunctionParser
{
    public class Fbk
    {
        public Fbk(string content)
        {
            Name = GetFbkName(content);
            Variables = GetVariables(content);
        }

        public string Name;
        public List<FbkVar> Variables;

        public static string GetFbkName(string funContent)
        {
            var match = Regex.Match(funContent, @"FUNCTION_BLOCK (\w+)");
            return match.Success ? match.Groups[1].Value : string.Empty;
        }

        public static List<FbkVar> GetVariables(string funContent)
        {
            var variables = new List<FbkVar>();
            foreach (var group in GetVariableGroupTexts(funContent))
            {
                var category = FbkVar.GetVariableCategory(group);
                if (category.Equals(FbkVarCategory.Other))
                {
                    Console.WriteLine("STOP");
                }
                variables.AddRange(FbkVar.GetVariableDefinitions(@group).Select(variableText => new FbkVar(variableText, category)));
            }

            return variables;
        }

        public static string FlattenDefinitionText(string content)
        {
            return string.Join(",", content.Split(Environment.NewLine).Select(s => s.Trim()));
        }

        public static string[] GetVariableGroupTexts(string content)
        {
            var flatContent = FlattenDefinitionText(content);
            var variableGroupRegex = new Regex(@"VAR.*END_VAR", RegexOptions.None);
            var match = variableGroupRegex.Match(flatContent);
            return match.Value.Split("END_VAR", StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim(',')).ToArray();
        }

        public static List<Fbk> ParseFunctionFile(string fileContent)
        {
            var flattenedText = Fbk.FlattenDefinitionText(fileContent);
            var functionTexts = GetFunctionBlockTexts(flattenedText);
            return functionTexts.Select(s => new Fbk(s)).ToList();
        }

        public static string[] GetFunctionBlockTexts(string content)
        {
            var variableGroupRegex = new Regex(@"FUNCTION_BLOCK.*$", RegexOptions.None);
            var texts = content.Split("END_FUNCTION_BLOCK").Select(v => v.Trim().Trim(','));
            var matches = texts.Select(v => variableGroupRegex.Match(v));

            return matches.Where(m => m.Success).Select(m => m.Value).ToArray();
        }
    }
}