using Inventor;

namespace DescriptionManager.Blanks
{
    /// <summary>
    ///     Blank Description Construtor
    /// </summary>
    public class BlankDescription
    {
        /// <summary>
        ///     Calcula o blank da peça
        /// </summary>
        /// <param name="blankType"></param>
        public void CalculateBlank(BlankType blankType)
        {
            switch (blankType.BlkType)
            {
                case BlankType.Cylinder:

                    // run cylinder method
                    IBlank cylinderBlank = new CylinderBlank();
                    cylinderBlank.Calculation();

                    break;
                case BlankType.Rectangle:

                    // run rectangle method
                    IBlank rectangleBlank = new RectangleBlank();
                    rectangleBlank.Calculation();

                    break;
                case BlankType.SheetMetal:

                    IBlank sheetMetalBlank = new SheetMetalBlank();
                    sheetMetalBlank.Calculation();

                    break;
                case BlankType.Linear:

                    IBlank linearBlank = new LinearBlank();
                    linearBlank.Calculation();

                    break;
            }
        }

        public void CalculateBlank(BlankType blankType,Document oDoc)
        {
            switch (blankType.BlkType)
            {
                case BlankType.Cylinder:

                    // run cylinder method
                    IBlank cylinderBlank = new CylinderBlank();
                    cylinderBlank.Calculation(oDoc);

                    break;
                case BlankType.Rectangle:

                    // run rectangle method
                    IBlank rectangleBlank = new RectangleBlank();
                    rectangleBlank.Calculation(oDoc);

                    break;
                case BlankType.SheetMetal:

                    IBlank sheetMetalBlank = new SheetMetalBlank();
                    sheetMetalBlank.Calculation(oDoc);

                    break;
                case BlankType.Linear:

                    IBlank linearBlank = new LinearBlank();
                    linearBlank.Calculation(oDoc);

                    break;
            }
        }
    }
}