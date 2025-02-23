using System;
using System.Linq;
using System.Windows.Forms;
using ConnectorDataBase.Json;

namespace ConnectorDataBase
{
    public static class DwgCutFileDataAccess
    {

        public static void Insert(DWG_CUT_FILE_MANAGER dwgCut)
        {
            var db = new ConnectorDataClassesDataContext(AppConfig.GetSqlConn());
            DWG_CUT_FILE_MANAGER dwg =
                Enumerable.SingleOrDefault(from d in db.DWG_CUT_FILE_MANAGERs
                                           where d.DWG_CUT_FILENAME == dwgCut.DWG_CUT_FILENAME
                                           select d);

            if (dwg == null)
            {
                dwgCut.CUT_FILE_VERSION = 0;
                db.DWG_CUT_FILE_MANAGERs.InsertOnSubmit(dwgCut);
            }
            else
            {
                dwg.UPDATE_DATE = DateTime.Now;
                dwg.UPDATE_BY = SystemInformation.UserName;
                dwg.DWG_CUT_BLANK_DIM = dwgCut.DWG_CUT_BLANK_DIM;
                dwg.DWG_CUT_MP_COD = dwgCut.DWG_CUT_MP_COD;
                dwg.DWG_CUT_MP_DESC_INVENTOR = dwgCut.DWG_CUT_MP_DESC_INVENTOR;
                dwg.DWG_CUT_FILE_UPDATED = dwg.DWG_CUT_FILE_UPDATED;
                // Versionamento
                dwg.CUT_FILE_VERSION++;


            }

            db.SubmitChanges();
            db.Dispose();

        }

        public static void Update(DWG_CUT_FILE_MANAGER pDwgCut)
        {
            using (ConnectorDataClassesDataContext db = new ConnectorDataClassesDataContext(AppConfig.GetSqlConn()))
            {
                // busca o item relacionado
                DWG_CUT_FILE_MANAGER dwgCut =
                    Enumerable.SingleOrDefault((from m in db.DWG_CUT_FILE_MANAGERs where m.DWG_CUT_FILENAME == pDwgCut.DWG_CUT_FILENAME select m));

                // atualiza os campos
                if (dwgCut != null)
                {
                    dwgCut.DWG_CUT_FILENAME = pDwgCut.DWG_CUT_FILENAME;
                    dwgCut.DWG_CUT_BLANK_DIM = pDwgCut.DWG_CUT_BLANK_DIM;
                    dwgCut.DWG_CUT_FILE_UPDATED = pDwgCut.DWG_CUT_FILE_UPDATED;
                    dwgCut.DWG_CUT_MP_COD = pDwgCut.DWG_CUT_MP_COD;
                    dwgCut.DWG_CUT_MP_DESC_INVENTOR = pDwgCut.DWG_CUT_MP_DESC_INVENTOR;
                    dwgCut.UPDATE_DATE = pDwgCut.UPDATE_DATE;
                    dwgCut.CREATE_BY = pDwgCut.CREATE_BY;
                    dwgCut.CREATE_DATE = pDwgCut.CREATE_DATE;

                }
                else return;

                // Submete as mudanças
                db.SubmitChanges();
            }
        }

        public static void UpdateStatus(DWG_CUT_FILE_MANAGER dwgCut)
        {

        }
       
    }
}
