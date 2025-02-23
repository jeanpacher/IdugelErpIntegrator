using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

using Inventor;

using InvUtils;


namespace DescriptionManager
{
    public class CoreDescription : ICoreDescription
    {
        public string GetDescriptionScript(string familyName)
        {
            var validate = SqlDescription.GetScript(familyName);

            if (validate == string.Empty)
            {
                MessageBox.Show("Família não cadastrada no Gerenciador de Descrições.","Família não cadastrada",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }

            return validate;
        }

        public List<string> GetDescriptionScripts(string familyName)
        {
            string strScript = GetDescriptionScript(familyName);

            Regex r = new Regex(@"(?<=\<)[^\>]+?(?=\>)");
            MatchCollection m = r.Matches(strScript);

            return (from Match match in m select match.Value).ToList();
        }

        public string GetDescriptionType(string familyName)
        {
            return SqlDescription.GetDescMode(familyName);
        }

        public string CompileScript(List<string> listScripts)
        {
            string description = string.Empty;

            BuilderDescriptions builderDescriptions = new BuilderDescriptions(); // Executa a atualização do Documento

            builderDescriptions.StartUpdateDoc();

            foreach (string ls in listScripts)
            {
                string[] result = ls.Split('=');
                description += GenerateDescription(result);

                //MessageBox.Show(description);
            }

            return description;
        }

        public string CompileScript(List<string> listScripts,Document oDoc)
        {
            string description = string.Empty;

            BuilderDescriptions builderDescriptions = new BuilderDescriptions(); // Executa a atualização do Documento

            builderDescriptions.StartUpdateDoc(oDoc);

            foreach (string ls in listScripts)
            {
                string[] result = ls.Split('=');
                description += GenerateDescription(result,oDoc);

                //MessageBox.Show(description);
            }

            return description;
        }

        private string GenerateDescription(string[] value)
        {
            string descResult = string.Empty;
            string varName = value[0];
            string varValue = value[1];

            switch (varName)
            {
                case "PARAM":
                    descResult = InvParameter.GetParameter(varValue);
                    break;
                case "IPROP":
                    descResult = BuilderDescriptions.GetIproperties(varValue);
                    break;
                case "TEXT":
                    descResult = BuilderDescriptions.GetText(varValue);
                    break;
            }

            return descResult;
        }

        private string GenerateDescription(string[] value,Document oDoc)
        {
            string descResult = string.Empty;
            string varName = value[0];
            string varValue = value[1];

            switch (varName)
            {
                case "PARAM":
                    descResult = InvParameter.GetParameter(varValue,oDoc);
                    break;
                case "IPROP":
                    descResult = BuilderDescriptions.GetIproperties(varValue,oDoc);
                    break;
                case "TEXT":
                    descResult = BuilderDescriptions.GetText(varValue);
                    break;
            }

            return descResult;
        }
    }
}