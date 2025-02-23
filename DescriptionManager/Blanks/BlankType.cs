namespace DescriptionManager.Blanks
{
    public class BlankType
    {
        public const string SheetMetal = "SheetMetal";
        public const string Cylinder = "Cylinder";
        public const string Rectangle = "Rectangle";
        public const string Linear = "Linear";

        public string BlkType;

        public BlankType(string blankType)
        {
            BlkType = blankType;
        }
    }
}