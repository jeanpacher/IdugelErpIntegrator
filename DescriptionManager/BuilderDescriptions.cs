using System;
using System.Globalization;

using DescriptionManager.Blanks;
using DescriptionManager.Resources;

using Inventor;

using InvUtils;

using WUtils;

namespace DescriptionManager
{

    public class BuilderDescriptions
    {

        private BlankDescription blankDescription;

        public void StartUpdateDoc()
        {
            BlankType blkt =
                new BlankType(
                    SqlDescription.GetDescMode(
                        GetIproperties(
                            GeneralConfigurations.Default
                                .GetValuesFromColumn)));

            blankDescription = new BlankDescription();
            blankDescription.CalculateBlank(blkt);
        }

        public void StartUpdateDoc(Document oDoc)
        {
            BlankType blkt =
                new BlankType(
                    SqlDescription.GetDescMode(
                        GetIproperties(GeneralConfigurations.Default.GetValuesFromColumn,oDoc)));

            blankDescription = new BlankDescription();
            blankDescription.CalculateBlank(blkt,oDoc);
        }

        public static string GetIproperties(string ipropName)
        {
            string valorResult = InvProps.GetInventorCustomProperties(ipropName);

            return valorResult;
        }

        public static string GetIproperties(string ipropName,Document oDoc)
        {
            string valorResult = InvProps.GetInventorCustomProperties(ipropName,oDoc);

            return valorResult;
        }

        public static string GetText(string textValue)
        {
            return textValue;
        }

    }

}