using Inventor;

using InvUtils;

namespace DescriptionManager
{

    public class CodeDescription
    {
        public string FileName { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }

        public void GetCodeDescription()
        {
            FileName = InvDocs.InvDoc().DisplayName;
           
            CodeDescriptionBuilder.GetCodeDescription(FileName, this);
        }

        public void GetCodeDescription(Document oDoc)
        {
            FileName = oDoc.DisplayName;

            CodeDescriptionBuilder.GetCodeDescription(FileName, this, oDoc);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oDoc"></param>
        /// <returns></returns>
        public CodeDescription GetCodeDescriptionAudit(Document oDoc)
        {
            FileName = oDoc.DisplayName;

            return CodeDescriptionBuilder.GetCodeDescriptionAudit(FileName, this, oDoc);
        }
    }

}