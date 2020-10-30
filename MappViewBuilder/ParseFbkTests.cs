using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FluentAssertions;
using Xunit;

namespace MappViewBuilder
{
    public class ParseFbkTests
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


            var fbkName = GetFbkName(funContent);

            fbkName.Should().Be("SampleFBK");
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

            var flatContent = FlattenDefinitionText(funContent);
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

            var variableGroupTexts = GetVariableGroupTexts(funContent);


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

            GetVariableCategory(inputText).Should().Be("Input");
            GetVariableCategory(outputText).Should().Be("Output");
            GetVariableCategory(localText).Should().Be("Local");
        }

        [Fact]
        public void WeCanExtractVariableDefinitionsFromVariableGroupText()
        {
            var outputText = "VAR_OUTPUT,var0 : { REDUND_UNREPLICABLE} BOOL;,var1 : TestableTON;,var2 : TT_Mark_DKFV_InputConfigFBK;";

            var variableDefinitions = GetVariableDefinitions(outputText);

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

            var var0Tuple = ParseVariable(var0);
            var var1Tuple = ParseVariable(var1);
            var var2Tuple = ParseVariable(var2);

            var0Tuple.name.Should().Be("var0");
            var0Tuple.type.Should().Be("BOOL");

            var1Tuple.name.Should().Be("var1");
            var1Tuple.type.Should().Be("TestableTON");

            var2Tuple.name.Should().Be("var2");
            var2Tuple.type.Should().Be("TT_Mark_DKFV_InputConfigFBK");
        }

        private string GetFbkName(string funContent)
        {
            var match = Regex.Match(funContent, @"FUNCTION_BLOCK (\w+)");
            return match.Success ? match.Groups[1].Value : string.Empty;
        }

        private string FlattenDefinitionText(string content)
        {
            return string.Join(",", content.Split(Environment.NewLine).Select(s => s.Trim()));
        }

        private string[] GetVariableGroupTexts(string content)
        {
            var flatContent = FlattenDefinitionText(content);
            var variableGroupRegex = new Regex(@"VAR.*END_VAR", RegexOptions.None);
            var match = variableGroupRegex.Match(flatContent);
            return match.Value.Split("END_VAR", StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim(',')).ToArray();
        }

        private string GetVariableCategory(string variableGroupText)
        {
            var header = variableGroupText.Split(',').FirstOrDefault();

            return string.IsNullOrWhiteSpace(header) ? string.Empty :
                header.Equals("VAR_INPUT") ? "Input" :
                header.Equals("VAR_OUTPUT") ? "Output" :
                header.Equals("VAR") ? "Local" : string.Empty;
        }

        private string[] GetVariableDefinitions(string variableGroupText)
        {
            return variableGroupText.Split(',').Where(s => !s.StartsWith("VAR")).ToArray();
        }

        private (string name, string type) ParseVariable(string variableText)
        {
            var match = Regex.Match(variableText, @"^(\w+).*\W(\w+);$");
            return match.Success ? (match.Groups[1].Value, match.Groups[2].Value) : (string.Empty, string.Empty);
        }

    }
}
