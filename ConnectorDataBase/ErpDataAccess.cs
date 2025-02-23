using System.Linq;
using ConnectorDataBase.Json;


namespace ConnectorDataBase
{
    public static class ErpDataAccess
    {
        public static IQueryable GetAllErpMaterialData()
        {
            var db = new ConnectorDataClassesDataContext(AppConfig.GetSqlConn());

            IQueryable result = from d in db.NEWAGEs select d;

            return result;
        }

        public static NEWAGE GetAllNewAgeMaterials()
        {
            var db = new ConnectorDataClassesDataContext(AppConfig.GetSqlConn());

            NEWAGE result = Enumerable.SingleOrDefault(from d in db.NEWAGEs select d);

            return result;
        }


        /// <summary>
        /// Atualiza os materiais no Banco
        /// </summary>
        /// <param name="pNewage"></param>
        public static void UpdateMaterial(NEWAGE pNewage)
        {
            // Conecta database
            using (ConnectorDataClassesDataContext db = new ConnectorDataClassesDataContext(AppConfig.GetSqlConn()))
            {
                // busca o item relacionado
                NEWAGE newAge =
                    Enumerable.SingleOrDefault((from m in db.NEWAGEs where m.ID == pNewage.ID select m));

                // atualiza os campos
                if (newAge != null)
                {
                    newAge.DESCRICAO_INVENTOR = pNewage.DESCRICAO_INVENTOR ?? string.Empty;
                    newAge.FATOR = pNewage.FATOR ?? string.Empty;
                    newAge.UNIDADE = pNewage.UNIDADE ?? string.Empty;
                    newAge.ESPESSURA = pNewage.ESPESSURA ?? string.Empty;
                }
                else return;

                // Submete as mudanças
                db.SubmitChanges();
            }
        }

        /// <summary>
        /// Busca um código específico
        /// </summary>
        /// <param name="codMateriaPrima"></param>
        /// <returns></returns>
        public static NEWAGE GetMaterialByCod(string codMateriaPrima)
        {
            // Conecta database
            using (ConnectorDataClassesDataContext db = new ConnectorDataClassesDataContext(AppConfig.GetSqlConn()))
            {
                // busca o item relacionado
                NEWAGE newAge = Enumerable.SingleOrDefault((from m in db.NEWAGEs where m.CODIGO == codMateriaPrima select m));

                return newAge;
            }
        }



    }
}