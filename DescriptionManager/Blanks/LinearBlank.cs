using AppResources;
using Inventor;
using InvUtils;
using System;
using WUtils;

namespace DescriptionManager.Blanks
{
    class LinearBlank : IBlank
    {
        public void Calculation()
        {
            var resultCalcDescription = InvParameter.GetParameter(ParameterNames.COMP);
            // Gravando diretamente nesta propriedade
            if (string.IsNullOrEmpty(resultCalcDescription))
            {
                InvMsg.Msg($"{AppMessages.ParamCompNotFound01}\n{AppMessages.ParamCompNotFound02}");
                return;
            }

            InvProps.SetInvIProperties(Resources.GeneralConfigurations.Default.SetBlankFieldValues,
                resultCalcDescription, InvPropetiesGroup.CustomFields);
        }

        public void Calculation(Document oDoc, bool message = true)
        {
            var resultCalcDescription = Math.Round(InvParameter.GetParameter(ParameterNames.COMP,oDoc).ToDouble(),2);
            // Gravando diretamente nesta propriedade
            if (string.IsNullOrEmpty(resultCalcDescription.ToString()))
            {
                InvMsg.Msg($"{AppMessages.ParamCompNotFound01}\n{AppMessages.ParamCompNotFound02}");
                return;
            }

            InvProps.SetInvIProperties(Resources.GeneralConfigurations.Default.SetBlankFieldValues,
               resultCalcDescription.ToString(), InvPropetiesGroup.CustomFields,oDoc);
        }

    }
}
