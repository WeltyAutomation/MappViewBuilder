using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FunctionParser.Tests;
using Xunit;

namespace MappViewContentGenerator.Tests
{
    public class ContentGeneratorTests
    {
        [Fact]
        public void WeCanTurnAFbkVarIntoAContentElement()
        {
            var fbkVar = new FbkVar("VariableName : VariableType;", FbkVarCategory.Input);
            var contentElement = new FbkVarContentElement(fbkVar);

            contentElement.ElementString.Should()
                .Be($@"<Widget xsi:type=""widgets.brease.Label"" id=""Label_VariableName"" top=""0"" left=""0"" width=""200"" height=""60"" zIndex=""0"" text=""VariableName"" style=""VarLabelBoolFalse"" />");
        }

        [Fact]
        public void WeCanTurnAFbkIntoAContentElement()
        {
            var funContent =
                @"FUNCTION_BLOCK SampleFBK
                    VAR_INPUT
                        InputVariable0 : BOOL;
                        InputVariable1 : BOOL;
                        InputVariable2 : BOOL;
                    END_VAR
                    VAR_OUTPUT
                        OutputVariable: BOOL;
                    END_VAR
                    VAR
                        InternalVariable: TransferConveyorStateEnum;
                    END_VAR
                END_FUNCTION_BLOCK";

            var fbk = new Fbk(Fbk.FlattenDefinitionText(funContent));

            var fbkContentElement = new FbkContentElement(fbk);

            fbkContentElement.ElementString.Should()
                .Be(
                    $@"<Widget xsi:type=""widgets.brease.Label"" id=""Label_SampleFBK"" top=""0"" left=""0"" width=""240"" height=""180"" zIndex=""0"" text=""SampleFBK"" style=""FbkLabel"" />");
        }

        [Fact]
        public void FbkContentElementHasVariableElementsWithCorrectPlacement()
        {
            var funContent =
                @"FUNCTION_BLOCK SampleFBK
                    VAR_INPUT
                        InputVariable0 : BOOL;
                        InputVariable1 : BOOL;
                        InputVariable2 : BOOL;
                    END_VAR
                    VAR_OUTPUT
                        OutputVariable: BOOL;
                    END_VAR
                    VAR
                        InternalVariable: TransferConveyorStateEnum;
                    END_VAR
                END_FUNCTION_BLOCK";

            var fbk = new Fbk(Fbk.FlattenDefinitionText(funContent));

            var fbkContentElement = new FbkContentElement(fbk);

            //inputs
            fbkContentElement.VarContentElements[0].Name.Should().Be("InputVariable0");
            fbkContentElement.VarContentElements[0].Top.Should().Be(0);
            fbkContentElement.VarContentElements[0].Left.Should().Be(0);

            fbkContentElement.VarContentElements[1].Name.Should().Be("InputVariable1");
            fbkContentElement.VarContentElements[1].Top.Should().Be(FbkVarContentElement.VarLabelHeight);
            fbkContentElement.VarContentElements[1].Left.Should().Be(0);

            fbkContentElement.VarContentElements[2].Name.Should().Be("InputVariable2");
            fbkContentElement.VarContentElements[2].Top.Should().Be(FbkVarContentElement.VarLabelHeight * 2);
            fbkContentElement.VarContentElements[2].Left.Should().Be(0);

            //local
            fbkContentElement.VarContentElements[3].Name.Should().Be("OutputVariable");
            fbkContentElement.VarContentElements[3].Top.Should().Be(0);
            fbkContentElement.VarContentElements[3].Left.Should().Be(FbkVarContentElement.VarLabelWidth + FbkContentElement.FbkWidth);

            //outputs
            fbkContentElement.VarContentElements[4].Name.Should().Be("InternalVariable");
            fbkContentElement.VarContentElements[4].Top.Should().Be(fbkContentElement.Top + FbkVarContentElement.VarLabelHeight);
            fbkContentElement.VarContentElements[4].Left.Should().Be(FbkVarContentElement.VarLabelWidth + FbkContentElement.Padding);
        }

    }

    public class ContentElement
    {
        public string Name;
        public int Top;
        public int Left;
        public int Height;
        public int Width;
        public string Style;
        public string ElementString =>
            $@"<Widget xsi:type=""widgets.brease.Label"" id=""Label_{Name}"" top=""{Top}"" left=""{Left}"" width=""{Width}"" height=""{Height}"" zIndex=""0"" text=""{Name}"" style=""{Style}"" />";
    }
    public class FbkContentElement : ContentElement
    {
        public static int Padding = 20;
        public static int FbkWidth = FbkVarContentElement.VarLabelWidth + Padding * 2;
        public FbkContentElement(Fbk fbk)
        {
            Name = fbk.Name;
            VarContentElements = GenerateContentElements(fbk.Variables);
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
    }
    public class FbkVarContentElement : ContentElement
    {
        public static int VarLabelHeight = 60;
        public static int VarLabelWidth = 200;
        public FbkVarContentElement(FbkVar fbkVar)
        {
            Name = fbkVar.Name;
            Height = VarLabelHeight;
            Width = VarLabelWidth;
            Style = "VarLabelBoolFalse";
        }
    }
}
