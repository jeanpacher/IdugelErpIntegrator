// ReSharper disable SpecifyACultureInStringConversionExplicitly

using System;
using Inventor;

namespace InvUtils
{
    /// <summary>
    /// Helper para peças em Chapa
    /// </summary>
    public static class InvSheetMetal
    {

        private const string SheetMetalDocType = "{9C464203-9BAE-11D3-8BAD-0060B0CE6BB4}";

        /// <summary>
        /// Planifica a chapa
        /// </summary>
        /// <returns></returns>
        public static void SheetMetalUnfold(PartDocument oSheetDoc)
        {
            //if (IsSheetMetalPart()) return;
            var oSheetDef = (SheetMetalComponentDefinition) oSheetDoc.ComponentDefinition;

            if (!oSheetDef.HasFlatPattern)
            {
                oSheetDef.Unfold();
                oSheetDef.FlatPattern.ExitEdit();

                InvApp.StandardAddInServer.m_InvApp.CommandManager.ControlDefinitions["AppIsometricViewCmd"].Execute();
            }
        }

        /// <summary>
        /// Pega os valores da planificação como String concatenada x X y
        /// </summary>
        /// <param name="oSheetDoc"></param>
        /// <returns></returns>
        public static string GetFlatPatternExtends(PartDocument oSheetDoc)
        {
            string blankDesc = null;
            if (IsSheetMetalPart(oSheetDoc))
            {
                var oSheetDef = (SheetMetalComponentDefinition)oSheetDoc.ComponentDefinition;

                if (!oSheetDef.HasFlatPattern)
                {
                    SheetMetalUnfold(oSheetDoc);
                }

                var oFlatPattern = oSheetDef.FlatPattern;
                var sheetMetalLength = Math.Round(oFlatPattern.Length * 10, 0).ToString();
                var sheetMetalWidth = Math.Round(oFlatPattern.Width * 10, 0).ToString();

                blankDesc = $"{sheetMetalWidth} X {sheetMetalLength}";
            }

            return blankDesc;
        }

        /// <summary>
        /// Seta o valor da espessura da chapa
        /// </summary>
        /// <param name="thickness"></param>
        public static void SetThickness(double thickness)
        {
            var oSheetDef = (SheetMetalComponentDefinition)InvDocs.PartDoc().ComponentDefinition;

            oSheetDef.UseSheetMetalStyleThickness = false;

            oSheetDef.Thickness.Value = thickness / 10;
        }

        /// <summary>
        /// Seta o valor da espessura da chapa solicitada
        /// </summary>
        /// <param name="thickness"></param>
        public static void SetThickness(double thickness, PartDocument oDoc)
        {
            var oSheetDef = (SheetMetalComponentDefinition) oDoc.ComponentDefinition;

            oSheetDef.UseSheetMetalStyleThickness = false;

            oSheetDef.Thickness.Value = thickness / 10;
        }

        /// <summary>
        /// Pega a espessura da peça em chapa ativa
        /// </summary>
        /// <returns></returns>
        public static double GetThickness()
        {
            var oSheetDef = (SheetMetalComponentDefinition)InvDocs.PartDoc().ComponentDefinition;

            Parameter thickness = oSheetDef.Thickness;

            return thickness.ModelValue * 10;
        }

        /// <summary>
        /// Pega a espessura da peça em chapa passada por parâmetro
        /// </summary>
        /// <param name="oSheetDoc"></param>
        /// <returns></returns>
        public static double GetThickness(PartDocument oSheetDoc)
        {
            if (IsSheetMetalPart(oSheetDoc))
            {
                var oSheetDef = (SheetMetalComponentDefinition)oSheetDoc.ComponentDefinition;

                Parameter thickness = oSheetDef.Thickness;

                return thickness.ModelValue * 10;
            }

            return double.NaN;
        }

        /// <summary>
        /// Verifica se o documento ativo é SheetMetal
        /// </summary>
        /// <returns></returns>
        public static bool IsSheetMetalPart()
        {
            return InvDocs.InvDoc().SubType.Equals(SheetMetalDocType);
        }

        /// <summary>
        /// Verifica se o documento solicitado é SheetMetal
        /// </summary>
        /// <param name="oSheetDoc"></param>
        /// <returns></returns>
        public static bool IsSheetMetalPart(PartDocument oSheetDoc)
        {
            return oSheetDoc.SubType.Equals(SheetMetalDocType);
        }
    }
}
