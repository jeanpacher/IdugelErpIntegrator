using Inventor;
using InvUtils;

namespace WConnectorModels
{
    public static class MigrateOldProperties
    {
        public static void RunIlogicMigrateProperties()
        {
            InvRunILogicRule.RuniLogic(InvDocs.InvDoc(), iLogicRuleNames.iLogicMigrateProperties.Trim());
        }
    }
}