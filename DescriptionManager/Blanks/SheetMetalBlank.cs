using System;
using Inventor;
using InvUtils;

namespace DescriptionManager.Blanks
{
    public class SheetMetalBlank : IBlank
    {
        public void Calculation()
        {
            string resultCalcDescription = InvSheetMetal.GetFlatPatternExtends(InvDocs.PartDoc()); //SheetMetalCalculationDescription();
            // Gravando diretamente nesta propriedade
            if (string.IsNullOrEmpty(resultCalcDescription))
            {
                InvMsg.Msg("Verifique se o documento é do tipo SheetMetal e se a planificação está sendo gerada corretamente.");
                return;
            }

            InvProps.SetInvIProperties(Resources.GeneralConfigurations.Default.SetBlankFieldValues,
                resultCalcDescription, InvPropetiesGroup.CustomFields);
        }

        public void Calculation(Document oDoc, bool message = true)
        {
            if (oDoc.DocumentType != DocumentTypeEnum.kPartDocumentObject) return;

            var partDoc = (PartDocument) oDoc;
            var resultCalcDescription = InvSheetMetal.GetFlatPatternExtends(partDoc);

            // Gravando diretamente nesta propriedade
            if (string.IsNullOrEmpty(resultCalcDescription))
            {
                if (message)
                {
                    InvMsg.Msg("Verifique se o documento é do tipo SheetMetal e se a planificação está sendo gerada corretamente.");
                }
                return;
            }

            InvProps.SetInvIProperties(Resources.GeneralConfigurations.Default.SetBlankFieldValues,
                resultCalcDescription, InvPropetiesGroup.CustomFields, oDoc);

        }

    }
}