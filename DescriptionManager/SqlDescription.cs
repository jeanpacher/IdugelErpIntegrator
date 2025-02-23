using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ConnectorDataBase.Json;

//using DataBaseConn;

namespace DescriptionManager
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class SqlDescription : ISqlDescription
    {
        private static string ConnectionString
            => AppConfig.GetSqlConn();

        private static void SqlCom(string cmdSql)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(cmdSql, connection))
                {
                    connection.Open();
                    command.ExecuteReader();
                }
            }
        }

        private static string SqlComScript(string cmdSql)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(cmdSql, connection))
                {
                    connection.Open();
                    SqlDataReader drResult = command.ExecuteReader();

                    string value = string.Empty;
                    while (drResult.Read())
                    {
                        value = drResult[0].ToString();
                    }
                    return value;
                }
            }
        }


        public static DataTable GetAllDescriptions()
        {
            return new DataTable();

        }

        public static void InsertDescription(FieldsDescription description)
        {
            string cmdSql =
                string.Format(
                              "INSERT INTO [IDUGEL_DB].[dbo].[DESCRIPTION_MANAGER] ([NAME_DESCRIPTION],[NAME_IPROP_FIELD],[FAMILY_TYPE],[SCRIPT_DESC],[COMMENT]) VALUES ('{0}','{1}','{2}','{3}','{4}')",
                    description.Name, description.IpropField, description.Family, description.Script,
                    description.BlankType);

            SqlCom(cmdSql);
        }

        public static void UpdateDescription(FieldsDescription description)
        {
            string cmdSql =
                string.Format(
                              "UPDATE {0} SET {1} = '{2}', {3} = '{4}', {5} = '{6}', {7} = '{8}' WHERE NAME_DESCRIPTION = '{9}'",
                    "", "NAME_IPROP_FIELD", description.IpropField, "FAMILY_TYPE", description.Family, "SCRIPT_DESC",
                    description.Script, "COMMENT", description.BlankType, description.Name);

            SqlCom(cmdSql);
        }

        public static void DeleteDescription(int id)
        {
            string cmdSql = string.Format("DELETE FROM {0} WHERE ID = {1}", "", id);

            SqlCom(cmdSql);
        }

        public static string GetScript(string familyName)
        {
            string scriptDesc =
                string.Format(
                              "SELECT [SCRIPT_DESC] FROM [IDUGEL_DB].[dbo].[DESCRIPTION_MANAGER] WHERE [FAMILY_TYPE] = '{0}'",
                    familyName);

            return SqlComScript(scriptDesc);
        }

        public static string GetDescMode(string familyName)
        {
            string comment =
                string.Format(
                              "SELECT [COMMENT] FROM [IDUGEL_DB].[dbo].[DESCRIPTION_MANAGER] WHERE [FAMILY_TYPE] = '{0}'",
                    familyName);

            return SqlComScript(comment);
        }
    }
}