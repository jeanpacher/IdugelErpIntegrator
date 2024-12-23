using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

//using BOM;
using InvApp;

using Inventor;

using Environment = System.Environment;
using File = System.IO.File;
using Path = System.IO.Path;

namespace WUtils
{
    public class CoreLog
    {
        private static void OldLog(string s)
        {
            string docName = string.Empty;
            //Get Inventor Doc Name
            try
            {
                docName = InvApp.StandardAddInServer.m_InvApp.ActiveEditDocument.DisplayName;
            }
            catch (Exception e)
            {
                docName = "Nenhum arquivo aberto";
            }

            var fullFileUpdate = Path.Combine(@"C:\InventorCustomAddins\IdugelNewAgeIntegration\Log\", @"LOG_BomCore.txt");
            if (!File.Exists(fullFileUpdate))
            {
                File.Create(Path.Combine(fullFileUpdate));
                File.AppendAllText(fullFileUpdate, $"LOG: {docName} -> " + s + Environment.NewLine);
            }
            else
            {
                File.AppendAllText(fullFileUpdate, $"LOG: {docName} -> " + s + Environment.NewLine);
                
            }
            
        }

        /// <summary>
        /// Escreve um log na temp do sistema
        /// </summary>
        /// <param name="s"></param>
        public static void WriteLog(string s)
        {
            try
            {
                var app = InvApp.StandardAddInServer.m_InvApp;
                var isreadonly = false;
                
                string docName;
                try
                {
                    var doc = app.ActiveDocument;
                    isreadonly = doc.IsModifiable;
                    docName = doc.FileSaveCounter != 0 ? doc.FullDocumentName : "Arquivo novo, ainda não salvo.";
                }
                catch (Exception e)
                {
                    docName = "Nenhum arquivo aberto";
                }

                var time = DateTime.Now;

                var fullFileUpdate = Path.Combine($"{Path.GetTempPath()}", @"TCAD_ERP_Connector.tclog");
                if (!File.Exists(fullFileUpdate))
                {
                    var logFile = File.Create(Path.Combine(fullFileUpdate));
                    logFile.Close();
                    logFile.Dispose();
                }

                string logString = $"<---------\n" +
                                   $"> Log: {time.ToString()}\n" +
                                   $"> Cad Version: {app.SoftwareVersion.DisplayVersion}\n" +
                                   $"> Product Edition: {app.SoftwareVersion.ProductEdition}\n" +
                                   $"> DocName: {docName}\n" +
                                   $"> ReadOnly: {isreadonly}\n" +
                                   $"> Mensagem: {s}\n" +
                                   $"<---------\n\n";
                app.StatusBarText = s;
                
                File.AppendAllText(fullFileUpdate, logString);
            }
            catch (Exception e)
            {
               // MessageBox.Show($"Erro ao Gravar o Log de erros.\n {e}");
            }
        }

        public static string GetMethodInfo(MethodBase mb)
        {
            try
            {
                var str = string.Empty;
                str = $"\tFunction: \n" +
                      $"\tName: {mb.Name}\n" +
                      $"\tFull Name: {mb.DeclaringType}";

                return str;
            }
            catch (Exception e)
            {
                return $"\n\tNão foi possível capturar o MethodBase.";
            }
        }

        //public static void WriteJsonLog(PartList partList, string desc)
        //{
        //    string fullPath = Assembly.GetExecutingAssembly().Location;
        //    string appfolderName = Path.GetDirectoryName(fullPath);

        //    string logFolderName = Path.Combine(appfolderName, $"Listas");

        //    var fullFileUpdate = Path.Combine(appfolderName, $"Listas\\{partList.AssemblyCod}_LogLista.tclog");

        //    if (!Directory.Exists(logFolderName))
        //    {
        //        Directory.CreateDirectory(logFolderName);
        //    }

        //    if (!File.Exists(fullFileUpdate))
        //    {
        //        File.Create(Path.Combine(fullFileUpdate));
        //    }

        //    File.AppendAllText(fullFileUpdate, $"\n\nLOG: {desc} -> \n" + partList.ToJson() + Environment.NewLine);
            
        //}
    }

}