//using InvApp;
using Inventor;

namespace InvUtils
{
    public static class InvDocs
    {
        /// <summary>
        /// Pega um Documento Ativo
        /// </summary>
        /// <returns></returns>
        public static Document InvDoc()
        {
            var invDocument = InvApp.StandardAddInServer.m_InvApp.ActiveEditDocument;

            return invDocument;
        }

        /// <summary>
        /// Pega um documento do Tipo PartDocument
        /// </summary>
        /// <returns></returns>
        public static PartDocument PartDoc()
        {
            var partDocument = (PartDocument)InvApp.StandardAddInServer.m_InvApp.ActiveEditDocument;

            return partDocument;
        }

        /// <summary>
        /// Pega um Documento do Tipo AssemblyDocument
        /// </summary>
        /// <returns></returns>
        public static AssemblyDocument AssyDoc()
        {
            var assyDocument = (AssemblyDocument) InvApp.StandardAddInServer.m_InvApp.ActiveEditDocument;

            return assyDocument;
        }

        /// <summary>
        /// Retorna o tipo do arquivo ativo em String - Vazia considerar nulo.
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static string GetInventorDocumentType(Document doc)
        {
            if (doc.SubType == InvDocType.InvPartDoc) return KInvDocs.PartDocument.ToString();
            if (doc.SubType == InvDocType.InvSheetDoc) return KInvDocs.SheetDocument.ToString();
            if (doc.SubType == InvDocType.InvAssyDoc) return KInvDocs.AssemblyDocument.ToString();
            if (doc.SubType == InvDocType.InvWeldDoc) return KInvDocs.WeldDocument.ToString();
            if (doc.SubType == InvDocType.InvDrawDoc) return KInvDocs.DrawingDocument.ToString();
            if (doc.SubType == InvDocType.InvPresentDoc) return KInvDocs.PresentationDocument.ToString();
            
            return string.Empty;
        }

        /// <summary>
        /// Abre um documento
        /// </summary>
        /// <param name="fullFilePath"></param>
        public static void OpenDocument(string fullFilePath)
        {
            InvApp.StandardAddInServer.m_InvApp.Documents.Open(fullFilePath);
        }

    }

}