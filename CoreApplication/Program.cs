using Inventor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Application = System.Windows.Forms.Application;
using IdugelErpIntegrator;




namespace IdugelErpIntegrator
{
    internal static class Program
    {

       // public static Inventor.Application invApp { get; set; } = null;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (InvApp.StandardAddInServer.m_InvApp == null)
                InvApp.StandardAddInServer.m_InvApp = (Inventor.Application)System.Runtime.InteropServices.Marshal.GetActiveObject("Inventor.Application");
           //InvApp.StandardAddInServer.m_InvApp = invApp;
            Application.Run(new MainErpConnUi());


            //try
            //{
               
            //    //Application.Run(new Estudos());

            //}
            //    catch (Exception)
            //    {
            //        MessageBox.Show("Inventor não está aberto");
            //        return;
            //    }


        }
    }
}
