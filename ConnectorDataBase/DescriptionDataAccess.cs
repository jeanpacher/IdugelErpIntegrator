using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConnectorDataBase.Json;


namespace ConnectorDataBase
{
    public class DescriptionDataAccess
    {
        public static void Insert(DESCRIPTION_MANAGER dm)
        {
            var db = new ConnectorDataClassesDataContext(AppConfig.GetSqlConn());
            db.DESCRIPTION_MANAGERs.InsertOnSubmit(dm);
            db.SubmitChanges();
            db.Dispose();
            //return true;
        }

        public static void Delete(string name)
        {
            ConnectorDataClassesDataContext db = new ConnectorDataClassesDataContext(AppConfig.GetSqlConn());

            DESCRIPTION_MANAGER dm =
                Enumerable.SingleOrDefault((from d in db.DESCRIPTION_MANAGERs where d.NAME_DESCRIPTION == name select d));

            db.DESCRIPTION_MANAGERs.DeleteOnSubmit(dm);
            db.SubmitChanges();
            db.Dispose();
        }

        public static void Delete(int id)
        {
            ConnectorDataClassesDataContext db = new ConnectorDataClassesDataContext(AppConfig.GetSqlConn());

            DESCRIPTION_MANAGER dm =
                Enumerable.SingleOrDefault((from d in db.DESCRIPTION_MANAGERs where d.ID == id select d));

            db.DESCRIPTION_MANAGERs.DeleteOnSubmit(dm);
            db.SubmitChanges();
            db.Dispose();
        }

        public static bool Update(int id)
        {
            return true;
        }

        public static void Update(DESCRIPTION_MANAGER pDesc)
        {
            // Conecta database
            using (ConnectorDataClassesDataContext db = new ConnectorDataClassesDataContext(AppConfig.GetSqlConn()))
            {
                // busca o item relacionado
                DESCRIPTION_MANAGER oDesc =
                    Enumerable.SingleOrDefault(
                                               (from selectDm in db.DESCRIPTION_MANAGERs
                                                where selectDm.ID == pDesc.ID
                                                select selectDm));

                oDesc = pDesc;

                // atualizo os campos
                if (oDesc != null)
                {
                    oDesc.NAME_IPROP_FIELD = pDesc.NAME_IPROP_FIELD;
                    oDesc.FAMILY_TYPE = pDesc.FAMILY_TYPE;
                    oDesc.SCRIPT_DESC = pDesc.SCRIPT_DESC;
                    oDesc.COMMENT = pDesc.COMMENT;
                }
                else return;

                // Submeto as mudanças
                db.SubmitChanges();
            }
        }

        public static IQueryable GetAllDescManagerData()
        {
            var db = new ConnectorDataClassesDataContext(AppConfig.GetSqlConn());

            IQueryable result = from d in db.DESCRIPTION_MANAGERs select d;

            return result;
        }
    }
}