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
        public static int FbkWidth = 240;
        public FbkContentElement(Fbk fbk)
        {
            Name = fbk.Name;
            
            Height = CalculateHeight(fbk.Variables);
            Width = FbkWidth;
            Style = "FbkLabel";
        }

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
