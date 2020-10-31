namespace MappViewContentGenerator
{
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
}