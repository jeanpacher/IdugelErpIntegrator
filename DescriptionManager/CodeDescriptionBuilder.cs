using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Inventor;

using InvUtils;

namespace DescriptionManager
{

    public static class CodeDescriptionBuilder
    {
        public static void GetCodeDescription(string fileNameComplete, CodeDescription codeDescription)
        {
            if (InvDocs.InvDoc().FileSaveCounter > 0)
            {
                CodeDescriptionFactory(fileNameComplete, codeDescription);
            }
            else
            {
                InvMsg.Msg("O arquivo ainda não foi salvo.\n" +
                           "É necesário salvar a peça com o código e a descrição antes de usar este comando!");
            }
        }

        public static void GetCodeDescription(string fileNameComplete, CodeDescription codeDescription, Document oDoc)
        {
            if (oDoc.FileSaveCounter > 0)
            {
                CodeDescriptionFactory(fileNameComplete, codeDescription);
            }
            else
            {
                InvMsg.Msg("O arquivo ainda não foi salvo.\n" +
                    "É necesário salvar a peça com o código e a descrição antes de usar este comando!");
            }
        }

        public static CodeDescription GetCodeDescriptionAudit(string fileNameComplete, CodeDescription codeDescription, Document oDoc)
        {
            if (oDoc.FileSaveCounter > 0)
            {
                return CodeDescriptionFactoryAudit(fileNameComplete, codeDescription);
            }
            else
            {
                InvMsg.Msg("O arquivo ainda não foi salvo.\n" +
                           "É necesário salvar a peça com o código e a descrição antes de usar este comando!");
                return null;
            }
        }

        /// <summary>
        /// Factory da Descrição
        /// </summary>
        /// <param name="fileNameComplete"></param>
        /// <param name="codeDescription"></param>
        private static void CodeDescriptionFactory(string fileNameComplete, CodeDescription codeDescription)
        {
            codeDescription.Code = fileNameComplete.Trim().Substring(0, 6); // pega os 6 caracteres iniciais
            codeDescription.Description = DescritionBuilder(fileNameComplete.Trim().Substring(6).Trim()); // pega a string a partir da 6 posição
        }

        private static CodeDescription CodeDescriptionFactoryAudit(string fileNameComplete, CodeDescription codeDescription)
        {
            codeDescription.Code = fileNameComplete.Trim().Substring(0, 6); // pega os 6 caracteres iniciais
            codeDescription.Description = DescritionBuilder(fileNameComplete.Trim().Substring(6).Trim()); // pega a string a partir da 6 posição

            return codeDescription;
        }

        /// <summary>
        /// Ajusta a descrição conforme o array recebido
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [Obsolete("Versão obsoleta -> Utilize outro Overload")]
        private static string DescritionBuilder(string[] fileName)
        {
            string desc = string.Empty;
            foreach (string s in fileName)
            {
                if (s != fileName[0])
                {
                    desc += s + " ";
                }
            }

            string descrition;
            if (desc.Contains(".ipt") || desc.Contains(".iam"))
            {
                descrition = desc.Remove(desc.Length - 5);
            }
            else
            {
                descrition = desc;
            }
            return descrition;
        }

        /// <summary>
        /// Ajusta a descrição conforme o texto recebido
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        private static string DescritionBuilder(string description)
        {
            string descritionBuilder = string.Empty;
            
            if (description.Contains(".ipt") || description.Contains(".iam"))
            {
                descritionBuilder = description.Remove(description.Length - 4);
            }
            else
            {
                descritionBuilder = description;
            }
            return descritionBuilder;
        }

    }
}