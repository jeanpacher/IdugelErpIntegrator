using Inventor;
using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;
using Inventor;


namespace InvApp
{
    /// <summary>
    /// This is the primary AddIn Server class that implements the ApplicationAddInServer interface
    /// that all Inventor AddIns are required to implement. The communication between Inventor and
    /// the AddIn is via the methods on this interface.
    /// </summary>
    [GuidAttribute("a38751c2-a17d-4285-8910-add752487a8f")]
    public class StandardAddInServer : Inventor.ApplicationAddInServer
    {

        // Inventor application object.
        public static Inventor.Application m_InvApp;
        public string inventorID;

        // InterfaceNames
        public string tabName = "NewAge";
        public string panelName = "Cadastro Produto";

        // ButtonDefinition
        ButtonDefinition ButtonNewAgeConnector;






        public StandardAddInServer()
        {
        }

        #region ApplicationAddInServer Members

        public void Activate(Inventor.ApplicationAddInSite addInSiteObject, bool firstTime)
        {
            // This method is called by Inventor when it loads the addin.
            // The AddInSiteObject provides access to the Inventor Application object.
            // The FirstTime flag indicates if the addin is loaded for the first time.

            // Initialize AddIn members.
            m_InvApp = addInSiteObject.Application;

            GuidAttribute guidAtt = (GuidAttribute)System.Attribute.GetCustomAttribute(typeof(StandardAddInServer), typeof(GuidAttribute));
            inventorID = "{" + guidAtt.Value + "}";

            CreateInterface();

            // TODO: Add ApplicationAddInServer.Activate implementation.
            // e.g. event initialization, command creation etc.


        }

        public void Deactivate()
        {
            // This method is called by Inventor when the AddIn is unloaded.
            // The AddIn will be unloaded either manually by the user or
            // when the Inventor session is terminated

            // TODO: Add ApplicationAddInServer.Deactivate implementation

            // Release objects.
            m_InvApp = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public void ExecuteCommand(int commandID)
        {
            // Note:this method is now obsolete, you should use the 
            // ControlDefinition functionality for implementing commands.
        }

        public object Automation
        {
            // This property is provided to allow the AddIn to expose an API 
            // of its own to other programs. Typically, this  would be done by
            // implementing the AddIn's API interface in a class and returning 
            // that class object through this property.

            get
            {
                // TODO: Add ApplicationAddInServer.Automation getter implementation
                return null;
            }
        }

        #endregion

        public void CreateInterface()
        {
            #region Criação da Interface (Aba e Painel)

            // PEGANDO OS CONTROLES DO INVENTOR
            ControlDefinitions controles = (ControlDefinitions)m_InvApp.CommandManager.ControlDefinitions;

            // PEGANDO OS CONTROLES DA INTERCACE
            UserInterfaceManager gerenciadorUI = (UserInterfaceManager)m_InvApp.UserInterfaceManager;

            //PEGANDO AS RIBBONS POR AMBIENTE
            Ribbon ribbonPart = gerenciadorUI.Ribbons["Part"];
            Ribbon ribbonAssembly = gerenciadorUI.Ribbons["Assembly"];
            Ribbon ribbonDrawing = gerenciadorUI.Ribbons["Drawing"];
            Ribbon ribbonZeroDoc = gerenciadorUI.Ribbons["ZeroDoc"]; // Acessa a ribbon quando não há documentos abertos

            //CRIANDO UMA NOVA ABA
            RibbonTab TabPart = ribbonPart.RibbonTabs.Add(tabName, "Autodesk.TabPart." + tabName, inventorID);
            RibbonTab TabAssembly = ribbonAssembly.RibbonTabs.Add(tabName, "Autodesk.TabAssembly." + tabName, inventorID);
            RibbonTab TabDrawing = ribbonDrawing.RibbonTabs.Add(tabName, "Autodesk.TabDrawing." + tabName, inventorID);
            RibbonTab TabZero = ribbonZeroDoc.RibbonTabs.Add(tabName, "Autodesk.TabZero." + tabName, inventorID);

            //CRIANDO UM NOVO PAINEL
            RibbonPanel painelPart = TabPart.RibbonPanels.Add(panelName, "Autodesk.PannelPart." + panelName, inventorID);
            RibbonPanel painelAssembly = TabAssembly.RibbonPanels.Add(panelName, "Autodesk.PannelAssembly." + panelName, inventorID);
            RibbonPanel painelDrawing = TabDrawing.RibbonPanels.Add(panelName, "Autodesk.PanelDrawing." + panelName, inventorID);
            RibbonPanel painelZero = TabZero.RibbonPanels.Add(panelName, "Autodesk.PanelZero." + panelName, inventorID);

            #endregion

            //Criação do botão CADASTRO DE PRODUTO
            IPictureDisp iconeBotaoImportar32x32 = (IPictureDisp)AxHostConverter.ImagemParaPictureDisp(global::InvApp.Properties.Resources.list);
            IPictureDisp iconeBotaoImportar16x16 = (IPictureDisp)AxHostConverter.ImagemParaPictureDisp(global::InvApp.Properties.Resources.list);


            ButtonNewAgeConnector = controles.AddButtonDefinition("Cadastro Produto", "Autodesk.KeepCAD.Inventor:BotãoCadastroProduto",
                CommandTypesEnum.kQueryOnlyCmdType, inventorID, "Comando para cadastrar dados do produto", "",
                iconeBotaoImportar32x32, iconeBotaoImportar16x16, ButtonDisplayEnum.kAlwaysDisplayText);

            painelPart.CommandControls.AddButton(ButtonNewAgeConnector, true);
            painelAssembly.CommandControls.AddButton(ButtonNewAgeConnector, true);
            //painelDrawing.CommandControls.AddButton(BotaoImportData, true);
            //painelZero.CommandControls.AddButton(ButtonNewAgeConnector, true);

            ButtonNewAgeConnector.OnExecute += new ButtonDefinitionSink_OnExecuteEventHandler(CarregarUI_NewAgeConnector);



        }

        void CarregarUI_NewAgeConnector(NameValueMap Content)
        {
            //IdugelErpIntegrator.MainErpConnUi mainErpConnUi = new IdugelErpIntegrator.MainErpConnUi();
            //ShowFormControl.ShowModelessForm(mainErpConnUi);
        }


   

    }

    public class AxHostConverter : AxHost
    {
        private AxHostConverter() : base("")
        {
        }

        static public stdole.IPictureDisp ImagemParaPictureDisp(Image image)
        {
            return (stdole.IPictureDisp)GetIPictureDispFromPicture(image);
        }

        static public Image PictureDispParaImagem(stdole.IPictureDisp pictureDisp)
        {
            return GetPictureFromIPicture(pictureDisp);
        }
    }
    public class ShowFormControl
    {
        /// <summary>
        ///     Get no Inventor Aplication
        /// </summary>
        public ShowFormControl()
        {
            //InvApp.cadApp = (Application)Marshal.GetActiveObject("Inventor.Application");
        }

        public static bool IsFormOpened(String formName)
        {
            foreach (Form objForm in System.Windows.Forms.Application.OpenForms)
            {
                if (objForm.GetType().Name == formName)
                {
                    objForm.WindowState = FormWindowState.Normal;

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Mostra o Form em modo Modal
        /// </summary>
        /// <param name="modalCmdDlg"></param>
        public static void ShowModalForm(Form modalCmdDlg)
        {
            WindowsWrapperForForm window =
                new WindowsWrapperForForm(
                    (IntPtr)StandardAddInServer.m_InvApp.MainFrameHWND);
            modalCmdDlg.Activate();
            modalCmdDlg.ShowInTaskbar = false;
            //ShowDialog is used..for Modal forms
            modalCmdDlg.ShowDialog(window);
        }

        /// <summary>
        ///     Mostra o Form em modo Modeless
        /// </summary>
        /// <param name="modelessCmdDlg"></param>
        public static void ShowModelessForm(Form modelessCmdDlg)
        {
            if (!(IsFormOpened(modelessCmdDlg.Name)))
            {
                WindowsWrapperForForm window = new WindowsWrapperForForm((IntPtr)StandardAddInServer.m_InvApp.MainFrameHWND);
                modelessCmdDlg.Activate();
                modelessCmdDlg.ShowInTaskbar = false;
                modelessCmdDlg.StartPosition = FormStartPosition.CenterScreen;
                modelessCmdDlg.Show(window);
            }
        }

        /// <summary>
        ///     Classe WindowsWrapper para enquadramento do form em modeless
        /// </summary>
        private class WindowsWrapperForForm : IWin32Window
        {
            private IntPtr _mHwnd;
            private int _p;

            public WindowsWrapperForForm(IntPtr handle)
            {
                _mHwnd = handle;
            }

            #region IWin32Window Members

            public IntPtr Handle
            {
                get { return _mHwnd; }
            }

            #endregion
        }
    }
}
