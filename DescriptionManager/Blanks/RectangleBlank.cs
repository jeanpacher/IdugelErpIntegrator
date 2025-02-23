using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DescriptionManager.Resources;
using Inventor;

using InvUtils;

namespace DescriptionManager.Blanks
{
    internal class RectangleBlank : IBlank
    {
        public void Calculation()
        {
            try
            {
                InvUtils.InvProps.SetInvIProperties(GeneralConfigurations.Default.SetBlankFieldValues,
                    InvParameter.GetParameter(GeneralConfigurations.Default.ParamComp),
                    InvUtils.InvPropetiesGroup.CustomFields);
            }
            catch
            {
                InvUtils.InvProps.SetInvIProperties(GeneralConfigurations.Default.SetBlankFieldValues,
                    InvParameter.GetParameter(string.Empty), InvUtils.InvPropetiesGroup.CustomFields);
            }
        }

        public void Calculation(Document oDoc, bool message = true)
        {
            try
            {
                InvUtils.InvProps.SetInvIProperties(GeneralConfigurations.Default.SetBlankFieldValues,InvParameter.GetParameter(GeneralConfigurations.Default.ParamComp),
                    InvUtils.InvPropetiesGroup.CustomFields,oDoc);
            }
            catch
            {
                InvUtils.InvProps.SetInvIProperties(GeneralConfigurations.Default.SetBlankFieldValues,
                    InvParameter.GetParameter(string.Empty), InvUtils.InvPropetiesGroup.CustomFields,oDoc);
            }
        }

    }
}