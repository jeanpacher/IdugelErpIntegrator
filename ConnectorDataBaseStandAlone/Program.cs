using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

using ConnectorDataBase;
using ConnectorDataBase.Views;

namespace DescriptionManagerStandAlone
{

    internal static class Program
    {

        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ViewDatabaseConfig());
        }

    }

}