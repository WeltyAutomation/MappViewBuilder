using System;
using FluentAssertions;
using FunctionParser;
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

        [Fact]
        public void FbkContentElementCanOutputItselfAndItsVariablesAsAnArrayOfStrings()
        {
            var funContent =
                @"FUNCTION_BLOCK SampleFBK
                    VAR_INPUT
                        InputVariable0 : BOOL;
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

            fbkContentElement.ToContent().Should().BeEquivalentTo(new string[]
            {
                "<--*****************SampleFBK Function Block Diagram*****************-->",
                $@"<Widget xsi:type=""widgets.brease.Label"" id=""Label_SampleFBK"" top=""0"" left=""0"" width=""240"" height=""60"" zIndex=""0"" text=""SampleFBK"" style=""FbkLabel"" />",
                $@"<Widget xsi:type=""widgets.brease.Label"" id=""Label_InputVariable0"" top=""0"" left=""0"" width=""200"" height=""60"" zIndex=""0"" text=""InputVariable0"" style=""VarLabelBoolFalse"" />",
                $@"<Widget xsi:type=""widgets.brease.Label"" id=""Label_OutputVariable"" top=""0"" left=""440"" width=""200"" height=""60"" zIndex=""0"" text=""OutputVariable"" style=""VarLabelBoolFalse"" />",
                $@"<Widget xsi:type=""widgets.brease.Label"" id=""Label_InternalVariable"" top=""60"" left=""220"" width=""200"" height=""60"" zIndex=""0"" text=""InternalVariable"" style=""VarLabelBoolFalse"" />",
                "<--******************************************************************-->"
            });
        }

    }
}
