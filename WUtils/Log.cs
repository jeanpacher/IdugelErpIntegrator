using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WUtils
{
    public static class Logger
    {
        private static readonly string logFilePath = @"C:\InventorCustomAddins\IdugelNewAgeIntegration\log.txt";

        public static void Log(string message)
        {
            try
            {
                // Adiciona timestamp à mensagem
                string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";

                // Escreve no arquivo de log
                File.AppendAllText(logFilePath, logMessage + Environment.NewLine);
            }
            catch (Exception ex)
            {
                // Se o log falhar, exibe no console (opcional)
                Console.WriteLine($"Erro ao escrever no log: {ex.Message}");
            }
        }
    }

}
