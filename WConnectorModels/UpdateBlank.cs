using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DescriptionManager;

using Inventor;

namespace WConnectorModels
{
    public class UpdateBlank
    {
        /// <summary>
        /// Método para atualizar o Blank do arquivo atico
        /// </summary>
        public static string Execute(string family)
        {
            CoreDescription crd = new CoreDescription();

            var descResult = crd.CompileScript(crd.GetDescriptionScripts(family));

            return descResult;
        }

        /// <summary>
        /// Método para atualizar o Blank do arquivo solicitado
        /// </summary>
        public static string Execute(string family, Document oDoc)
        {
            CoreDescription crd = new CoreDescription();

            var descResult = crd.CompileScript(crd.GetDescriptionScripts(family),oDoc);

            return descResult;
        }

    }
}