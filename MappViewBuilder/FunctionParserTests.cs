using System.IO;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace FunctionParser.Tests
{
    public class FunctionParserTests
    {

        [Fact]
        public void WeCanGetTheNameOfAFunctionBlock()
        {
            var funContent =
                @"{REDUND_ERROR} FUNCTION_BLOCK SampleFBK (*Transfer Conveyor State Manager*) (*$GROUP=User,$CAT=User,$GROUPICON=User.png,$CATICON=User.png*)
                    VAR_INPUT
                        InputVariable : BOOL;
                    END_VAR
                    VAR_OUTPUT
                        OutputVariable: { REDUND_UNREPLICABLE} BOOL;
                    END_VAR
                    VAR
                        InternalVariable: TransferConveyorStateEnum;
                    END_VAR
                END_FUNCTION_BLOCK";


            var fbkName = Fbk.GetFbkName(funContent);

            fbkName.Should().Be("SampleFBK");
        }

        [Fact]
        public void WeCanIgnoreFunctions()
        {
            var funContent =
                "FUNCTION_BLOCK SampleFBK END_FUNCTION_BLOCK,FUNCTION SampleFunc : BOOL END_FUNCTION,FUNCTION_BLOCK SampleFBK2 END_FUNCTION_BLOCK";

            var fbkTexts = Fbk.GetFunctionBlockTexts(funContent);
            fbkTexts[0].Should().Be("FUNCTION_BLOCK SampleFBK");
            fbkTexts[1].Should().Be("FUNCTION_BLOCK SampleFBK2");
        }

        [Fact]
        public void WeCanRemoveLineBreaksAndWhiteSpaceFromFunctionDefinition()
        {
            var funContent =
                @"FUNCTION_BLOCK SampleFBK
                    VAR_INPUT
                        InputVariable : BOOL;
                    END_VAR
                    VAR_OUTPUT
                        OutputVariable: { REDUND_UNREPLICABLE} BOOL;
                    END_VAR
                    VAR
                        InternalVariable: TransferConveyorStateEnum;
                    END_VAR
                END_FUNCTION_BLOCK";

            var flatContent = Fbk.FlattenDefinitionText(funContent);
            flatContent.Should()
                .Be("FUNCTION_BLOCK SampleFBK,VAR_INPUT,InputVariable : BOOL;,END_VAR,VAR_OUTPUT,OutputVariable: { REDUND_UNREPLICABLE} BOOL;,END_VAR,VAR,InternalVariable: TransferConveyorStateEnum;,END_VAR,END_FUNCTION_BLOCK");
        }

        [Fact]
        public void WeCanIsolateVariableGroupings()
        {
            var funContent =
                @"FUNCTION_BLOCK SampleFBK
                    VAR_INPUT
                        InputVariable : BOOL;
                    END_VAR
                    VAR_OUTPUT
                        OutputVariable : { REDUND_UNREPLICABLE} BOOL;
                    END_VAR
                    VAR
                        InternalVariable : TransferConveyorStateEnum;
                    END_VAR
                END_FUNCTION_BLOCK";

            var variableGroupTexts = Fbk.GetVariableGroupTexts(funContent);


            variableGroupTexts.Length.Should().Be(3);
            variableGroupTexts[0].Should().Be("VAR_INPUT,InputVariable : BOOL;");
            variableGroupTexts[1].Should().Be("VAR_OUTPUT,OutputVariable : { REDUND_UNREPLICABLE} BOOL;");
            variableGroupTexts[2].Should().Be("VAR,InternalVariable : TransferConveyorStateEnum;");
        }

        [Fact]
        public void WeCanDetermineVariableVatagoryFromGroupText()
        {
            var inputText = "VAR_INPUT,InputVariable : BOOL;";
            var outputText = "VAR_OUTPUT,OutputVariable : { REDUND_UNREPLICABLE} BOOL;";
            var localText = "VAR,InternalVariable : TransferConveyorStateEnum;";

            FbkVar.GetVariableCategory(inputText).Should().Be(FbkVarCategory.Input);
            FbkVar.GetVariableCategory(outputText).Should().Be(FbkVarCategory.Output);
            FbkVar.GetVariableCategory(localText).Should().Be(FbkVarCategory.Local);
        }

        [Fact]
        public void WeCanExtractVariableDefinitionsFromVariableGroupText()
        {
            var outputText = "VAR_OUTPUT,var0 : { REDUND_UNREPLICABLE} BOOL;,var1 : TestableTON;,var2 : TT_Mark_DKFV_InputConfigFBK;";

            var variableDefinitions = FbkVar.GetVariableDefinitions(outputText);

            variableDefinitions[0].Should().Be("var0 : { REDUND_UNREPLICABLE} BOOL;");
            variableDefinitions[1].Should().Be("var1 : TestableTON;");
            variableDefinitions[2].Should().Be("var2 : TT_Mark_DKFV_InputConfigFBK;");
        }

        [Fact]
        public void WeCanParseVariableTextIntoNameAndType()
        {
            var var0 = "var0 : { REDUND_UNREPLICABLE} BOOL;";
            var var1 = "var1 : TestableTON;";
            var var2 = "var2 : TT_Mark_DKFV_InputConfigFBK;";

            var var0Tuple = FbkVar.ParseVariable(var0);
            var var1Tuple = FbkVar.ParseVariable(var1);
            var var2Tuple = FbkVar.ParseVariable(var2);

            var0Tuple.name.Should().Be("var0");
            var0Tuple.type.Should().Be("BOOL");

            var1Tuple.name.Should().Be("var1");
            var1Tuple.type.Should().Be("TestableTON");

            var2Tuple.name.Should().Be("var2");
            var2Tuple.type.Should().Be("TT_Mark_DKFV_InputConfigFBK");
        }

        [Fact]
        public void WeCanParseFbkTextToObject()
        {
            var funContent =
                @"FUNCTION_BLOCK SampleFBK
                    VAR_INPUT
                        InputVariable : BOOL;
                    END_VAR
                    VAR_OUTPUT
                        OutputVariable : { REDUND_UNREPLICABLE} BOOL;
                    END_VAR
                    VAR
                        InternalVariable : TransferConveyorStateEnum;
                    END_VAR
                END_FUNCTION_BLOCK";

            var fbk = new Fbk(funContent);

            fbk.Name.Should().Be("SampleFBK");
            fbk.Variables.Count.Should().Be(3);
            fbk.Variables[0].Category.Should().Be(FbkVarCategory.Input);
            fbk.Variables[0].Name.Should().Be("InputVariable");
            fbk.Variables[0].Type.Should().Be("BOOL");

            fbk.Variables[1].Category.Should().Be(FbkVarCategory.Output);
            fbk.Variables[1].Name.Should().Be("OutputVariable");
            fbk.Variables[1].Type.Should().Be("BOOL");

            fbk.Variables[2].Category.Should().Be(FbkVarCategory.Local);
            fbk.Variables[2].Name.Should().Be("InternalVariable");
            fbk.Variables[2].Type.Should().Be("TransferConveyorStateEnum");
        }

        [Fact]
        public void WeCanParseAFullDotFunFileWithMultipleFbks()
        {
            var sampleFilePath = "SampleFile\\TTCtrlLIB.fun";
            var fileContent = File.ReadAllText(sampleFilePath);

            var fbks = Fbk.ParseFunctionFile(fileContent);

            fbks.Count.Should().Be(10);

            fbks[0].Name.Should().Be("TransferConveyorStateMgrFBK");
            fbks[0].Variables.Count.Should().Be(11);
            fbks[0].Variables.Count(v => v.Category.Equals(FbkVarCategory.Input)).Should().Be(5);
            fbks[0].Variables.Count(v => v.Category.Equals(FbkVarCategory.Output)).Should().Be(2);
            fbks[0].Variables.Count(v => v.Category.Equals(FbkVarCategory.Local)).Should().Be(4);

            fbks[1].Name.Should().Be("TT_Mark_DKFV_CtrlFBK");
            fbks[1].Variables.Count.Should().Be(25);
            fbks[1].Variables.Count(v => v.Category.Equals(FbkVarCategory.Input)).Should().Be(18);
            fbks[1].Variables.Count(v => v.Category.Equals(FbkVarCategory.Output)).Should().Be(4);
            fbks[1].Variables.Count(v => v.Category.Equals(FbkVarCategory.Local)).Should().Be(3);

            fbks[2].Name.Should().Be("TT_Mark_DKFV_InputConfigFBK");
            fbks[2].Variables.Count.Should().Be(19);
            fbks[2].Variables.Count(v => v.Category.Equals(FbkVarCategory.Input)).Should().Be(9);
            fbks[2].Variables.Count(v => v.Category.Equals(FbkVarCategory.Output)).Should().Be(4);
            fbks[2].Variables.Count(v => v.Category.Equals(FbkVarCategory.Local)).Should().Be(6);

            fbks[3].Name.Should().Be("TT_Mark_DKFV_MgrFBK");
            fbks[3].Variables.Count.Should().Be(39);
            fbks[3].Variables.Count(v => v.Category.Equals(FbkVarCategory.Input)).Should().Be(26);
            fbks[3].Variables.Count(v => v.Category.Equals(FbkVarCategory.Output)).Should().Be(8);
            fbks[3].Variables.Count(v => v.Category.Equals(FbkVarCategory.Local)).Should().Be(5);

            fbks[4].Name.Should().Be("TT_Mark_DKFV_OutputConfigFBK");
            fbks[4].Variables.Count.Should().Be(7);
            fbks[4].Variables.Count(v => v.Category.Equals(FbkVarCategory.Input)).Should().Be(4);
            fbks[4].Variables.Count(v => v.Category.Equals(FbkVarCategory.Output)).Should().Be(3);
            fbks[4].Variables.Count(v => v.Category.Equals(FbkVarCategory.Local)).Should().Be(0);

            fbks[5].Name.Should().Be("TT_Mark_DKFV_PartPassCtrlFBK");
            fbks[5].Variables.Count.Should().Be(9);
            fbks[5].Variables.Count(v => v.Category.Equals(FbkVarCategory.Input)).Should().Be(6);
            fbks[5].Variables.Count(v => v.Category.Equals(FbkVarCategory.Output)).Should().Be(3);
            fbks[5].Variables.Count(v => v.Category.Equals(FbkVarCategory.Local)).Should().Be(0);

            fbks[6].Name.Should().Be("TT_TDC_DKFV_CtrlFBK");
            fbks[6].Variables.Count.Should().Be(24);
            fbks[6].Variables.Count(v => v.Category.Equals(FbkVarCategory.Input)).Should().Be(17);
            fbks[6].Variables.Count(v => v.Category.Equals(FbkVarCategory.Output)).Should().Be(4);
            fbks[6].Variables.Count(v => v.Category.Equals(FbkVarCategory.Local)).Should().Be(3);

            fbks[7].Name.Should().Be("TT_TDC_DKFV_InputConfigFBK");
            fbks[7].Variables.Count.Should().Be(21);
            fbks[7].Variables.Count(v => v.Category.Equals(FbkVarCategory.Input)).Should().Be(9);
            fbks[7].Variables.Count(v => v.Category.Equals(FbkVarCategory.Output)).Should().Be(5);
            fbks[7].Variables.Count(v => v.Category.Equals(FbkVarCategory.Local)).Should().Be(7);

            fbks[8].Name.Should().Be("TT_TDC_DKFV_OutputConfigFBK");
            fbks[8].Variables.Count.Should().Be(7);
            fbks[8].Variables.Count(v => v.Category.Equals(FbkVarCategory.Input)).Should().Be(4);
            fbks[8].Variables.Count(v => v.Category.Equals(FbkVarCategory.Output)).Should().Be(3);
            fbks[8].Variables.Count(v => v.Category.Equals(FbkVarCategory.Local)).Should().Be(0);

            fbks[9].Name.Should().Be("TT_TDC_DKFV_PartPassCtrlFBK");
            fbks[9].Variables.Count.Should().Be(9);
            fbks[9].Variables.Count(v => v.Category.Equals(FbkVarCategory.Input)).Should().Be(6);
            fbks[9].Variables.Count(v => v.Category.Equals(FbkVarCategory.Output)).Should().Be(3);
            fbks[9].Variables.Count(v => v.Category.Equals(FbkVarCategory.Local)).Should().Be(0);
        }


    }
}
