using System;
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

        
    }

    public class FbkVarContentElement
    {
        public FbkVarContentElement(FbkVar fbkVar)
        {
            _fbkVar = fbkVar;
        }

        private FbkVar _fbkVar;
        public int Top;
        public int Left;
        public int Height => 60;
        public int Width => 200;

        public string ElementString =>
            $@"<Widget xsi:type=""widgets.brease.Label"" id=""Label_{_fbkVar.Name}"" top=""0"" left=""0"" width=""200"" height=""60"" zIndex=""0"" text=""{_fbkVar.Name}"" style=""VarLabelBoolFalse"" />";
    }
}
