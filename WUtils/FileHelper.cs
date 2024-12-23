using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WUtils
{
    public static class FileHelper
    {
        /// <summary>
        /// Retorna o endereço/diretório de uma classe T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string GetAppPath<T>() where T : class
        {
           return System.IO.Path.GetDirectoryName(typeof(T).Assembly.Location);
        }

    }
}
