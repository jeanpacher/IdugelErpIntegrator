using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using InvApp;
using Application = Inventor.Application;

// ReSharper disable UnusedParameter.Local
#pragma warning disable 169

namespace WUtils
{
    /// <summary>
    ///     Controla a apresentação e abertura dos forms
    /// </summary>
    public class ShowFormControl
    {
        /// <summary>
        ///     Get no Inventor Aplication
        /// </summary>
        public ShowFormControl()
        {
            InvApp.StandardAddInServer.m_InvApp = (Application)Marshal.GetActiveObject("Inventor.Application");
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
                    (IntPtr)InvApp.StandardAddInServer.m_InvApp.MainFrameHWND);
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
                WindowsWrapperForForm window = new WindowsWrapperForForm((IntPtr)InvApp.StandardAddInServer.m_InvApp.MainFrameHWND);
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