using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventor;

using WUtils;

namespace InvUtils
{
    public class InvParameter
    {
        /// <summary>
        /// Retorna o valor do parâmetro da peça ativa
        /// </summary>
        /// <param name="paramName"></param>
        /// <returns></returns>
        public static string GetParameter(string paramName)
        {
            PartDocument partDoc = InvDocs.PartDoc();

            Parameters param = partDoc.ComponentDefinition.Parameters;
            string valorResult;
            try
            {
                var p = param[paramName];

                try
                {
                    valorResult = (p.ModelValue * 10).ToString();
                }
                catch (Exception)
                {
                    valorResult = p.Expression;
                }
            }
            catch (Exception)
            {
                InvMsg.Msg($"O parâmetro - {paramName} - não existe");
                return String.Empty;
            }

            return ParameterRound(valorResult);
        }

        /// <summary>
        /// Retorna o valor do parâmetro da peça informada
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static string GetParameter(string paramName, Document doc)
        {
            PartDocument partDoc = (PartDocument) doc;

            Parameters param = partDoc.ComponentDefinition.Parameters;
            string valorResult;
            try
            {
                var p = param[paramName];

                try
                {
                    valorResult = (p.ModelValue * 10).ToString();
                }
                catch (Exception)
                {
                    valorResult = p.Expression;
                }
            }
            catch (Exception)
            {
                InvMsg.Msg(string.Format("O parâmetro - {0} - não existe", paramName));
                return string.Empty;
            }

            return ParameterRound(valorResult);
        }

        /// <summary>
        /// Retorna o valor do parâmetro da peça informada
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static string GetParameterText(string paramName, Document doc)
        {
            PartDocument partDoc = (PartDocument) doc;

            Parameters param = partDoc.ComponentDefinition.Parameters;
            string valorResult;
            try
            {
                var p = param[paramName];
                try
                {
                    valorResult = p.Value.ToString();
                }
                catch (Exception)
                {
                    valorResult = p.Expression;
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }

            return valorResult.Replace("\"",string.Empty).Trim();
        }

        /// <summary>
        /// Arredonda um parâmetro
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string ParameterRound(string value)
        {
            var valueConverted = value.ToDouble();

            return Math.Round(valueConverted, 3).ToString(CultureInfo.InvariantCulture);
        }

    }
}
