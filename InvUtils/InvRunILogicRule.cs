using Autodesk.iLogic.Automation;

using InvApp;
using Inventor;

namespace InvUtils
{
    public class InvRunILogicRule
    {
        private static readonly string _iLogicAddinGuid = Resources.InvResource.iLogicAddin;

        /// <summary>
        ///     Roda uma regra externa do iLogic no documeto informado
        /// </summary>
        /// <param name="document"></param>
        /// <param name="nomeIlogicRule"></param>
        public static void RuniLogic(Document document, string nomeIlogicRule)
        {
            ApplicationAddIn _addin = null;
            try
            {
                // try para pegar o Addin
                _addin = InvApp.StandardAddInServer.m_InvApp.ApplicationAddIns.ItemById[_iLogicAddinGuid];
            }
            catch
            {
                // Erros
                InvMsg.Msg("Não foi possível pegar o iLogic Addin");
            }

            if (_addin != null)
            {
                // Ativa o Addin
                if (!_addin.Activated)
                    _addin.Activate();

                // Entrada do iLogic
                iLogicAutomation iLogic = (iLogicAutomation)_addin.Automation;

                Document partDoc = document;

                iLogic.RunExternalRule(partDoc, nomeIlogicRule);
            }
        }
    }
}