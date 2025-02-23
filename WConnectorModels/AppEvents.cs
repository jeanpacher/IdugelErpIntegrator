using System;
using System.Threading.Tasks;

using DescriptionManager;

using InvApp;
using InvUtils;
using Inventor;

namespace WConnectorModels
{
    public class AppEvents
    {
        private ApplicationEvents events;

        public ApplicationEvents ApplicationEvents => events;

        public AppEvents()
        {
            events = InvApp.StandardAddInServer.m_InvApp.ApplicationEvents;
           
        }

        public void StartEvents()
        {
            events.OnSaveDocument += GetCodeAndNameEvents_OnSaveDocument;
        }
        
        private void GetCodeAndNameEvents_OnSaveDocument(_Document DocumentObject, EventTimingEnum BeforeOrAfter, NameValueMap Context, out HandlingCodeEnum HandlingCode)
        {
            HandlingCode = HandlingCodeEnum.kEventNotHandled;

            // Verifica se o evento está sendo disparado antes ou depois de ser salvo
            if (BeforeOrAfter != EventTimingEnum.kBefore)
            {
                // MessageBox.Show($"O Arquivo será salvo. {DocumentObject.DisplayName}");
                // InvMsg.Msg("Nome do Arquivo: " + DocumentObject.DisplayName);
                return;
            }
            
            HandlingCode = HandlingCodeEnum.kEventHandled;
            if (BeforeOrAfter == EventTimingEnum.kBefore)
            {
                 //InvMsg.Msg(DocumentObject.DisplayName);

                //AddinGlobal.InventorApp.StatusBarText = $"O Arquivo foi salvo. {DocumentObject.DisplayName}" ;
                //
                //var codeDesc = new CodeDescription();
                //codeDesc.GetCodeDescription(DocumentObject);
                //
                //AddinGlobal.InventorApp.StatusBarText = $"Arquivo Salvo: {codeDesc.Code} -> {codeDesc.Description} ...";

            }

        }

        private void btn_OpenFile_Click(object sender, EventArgs e)

        {

            // Create a new FileDialog object.

            Inventor.FileDialog oFileDlg;

            InvApp.StandardAddInServer.m_InvApp.CreateFileDialog(out(oFileDlg));
            

            // Define the filter to select part

            // and assembly files or any file.

            oFileDlg.Filter =

                "Inventor Files (*.iam;*.ipt)|*.iam;*.ipt|All Files )|*.*";

 

            // Define the part and assembly files filter

            // to be the default filter.

            oFileDlg.FilterIndex = 1;

 

            // Set the title for the dialog.

            oFileDlg.DialogTitle = "Open File Test";

 

            // Set the initial directory that

            // will be displayed in the dialog.

            oFileDlg.InitialDirectory = @"C:\Vault_Idugel\ENGENHARIA";

 

            // Set the flag so an error will not be raised

            // if the user clicks the Cancel button.

            oFileDlg.CancelError = false;

 

            // Show the open dialog.  The same procedure

            // is also used for the Save dialog.

            // The commented code can be used for the Save dialog.

            oFileDlg.ShowOpen();

            // oFileDlg.ShowSave();

 

            System.Windows.Forms.MessageBox.Show(

                "File \n" + oFileDlg.FileName + "\nwas selected.",

                "Selected file");     

        }



    }
}
