using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ConnectorDataBase;
using Newtonsoft.Json;

namespace ConnectorDataBase.Json
{
    public class AppConfig
    {
        public string AppPath { get; set; }
        public string AppSqlString { get; set; }
        public List<string> Names { get; set; }
        public string AppWebPortal { get; set; }

        public static string GetSqlConn()
        {
            var appConfig = JsonConvert.DeserializeObject<AppConfig>(File.ReadAllText(GetAppPath + @"\AppConfig.json"));

            return appConfig.AppSqlString;
        }

        public static string GetAppPath
        {
            get
            {
                var path = System.IO.Path.GetDirectoryName(typeof(ConnectorDataClassesDataContext).Assembly.Location);
                return path;
            }
        }

        public static AppConfig GetAppConfig()
        {
            return JsonConvert.DeserializeObject<AppConfig>(File.ReadAllText(GetAppPath + @"\AppConfig.json"));
        }

        public static List<string> GetNames()
        {
            //var appConfig = JsonConvert.DeserializeObject<AppConfig>(File.ReadAllText(GetAppPath + @"\AppConfig.json"));

            return null;
            //return appConfig.Names;
        }

        public static string GetWebPortalPath()
        {
            var webPath = JsonConvert.DeserializeObject<AppConfig>(File.ReadAllText(GetAppPath + @"\AppConfig.json"));

            return webPath.AppWebPortal;
        }
    }
}
