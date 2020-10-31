using System.Collections.Generic;
using System.Linq;
using FunctionParser;

namespace MappViewContentGenerator
{
    public class FbkContentElement : ContentElement
    {
        public static int Padding = 20;
        public static int FbkWidth = FbkVarContentElement.VarLabelWidth + Padding * 2;
        public FbkContentElement(Fbk fbk)
        {
            Name = fbk.Name;
            VarContentElements = GenerateContentElements(fbk.Variables);
            Left = FbkVarContentElement.VarLabelWidth;
            Height = CalculateHeight(fbk.Variables);
            Width = FbkWidth;
            Style = "FbkLabel";
        }

        public List<FbkVarContentElement> VarContentElements;

        private int CalculateHeight(List<FbkVar> fbkVars)
        {
            var counts = new[]
            {
                fbkVars.Count(v => v.Category.Equals(FbkVarCategory.Input)),
                fbkVars.Count(v => v.Category.Equals(FbkVarCategory.Output)),
                fbkVars.Count(v => v.Category.Equals(FbkVarCategory.Local))
            };
            return counts.Max() * FbkVarContentElement.VarLabelHeight;
        }

        private List<FbkVarContentElement> GenerateContentElements(List<FbkVar> vars)
        {
            var elements = new List<FbkVarContentElement>();

            var topOffsets = new Dictionary<FbkVarCategory, int>
            {
                {FbkVarCategory.Input, 0},
                {FbkVarCategory.Output, 0},
                {FbkVarCategory.Local, FbkVarContentElement.VarLabelHeight},
            };

            var leftOffsets = new Dictionary<FbkVarCategory, int>
            {
                {FbkVarCategory.Input, 0},
                {FbkVarCategory.Output, FbkVarContentElement.VarLabelWidth + FbkWidth},
                {FbkVarCategory.Local, FbkVarContentElement.VarLabelWidth + Padding},
            };


            foreach (var fbkVar in vars)
            {
                var element = new FbkVarContentElement(fbkVar)
                {
                    Top = topOffsets[fbkVar.Category], 
                    Left = leftOffsets[fbkVar.Category]
                };

                topOffsets[fbkVar.Category] = topOffsets[fbkVar.Category] + FbkVarContentElement.VarLabelHeight;
                elements.Add(element);
            }

            return elements;
        }

        public string[] ToContent()
        {
            var result = new List<string>();
            result.Add($"<--*****************{Name} Function Block Diagram*****************-->");
            result.Add(ElementString);
            result.AddRange(VarContentElements.Select(v => v.ElementString));
            result.Add("<--******************************************************************-->");
            return result.ToArray();
        }
    }
}