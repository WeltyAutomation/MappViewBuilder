using FunctionParser;

namespace MappViewContentGenerator
{
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