using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvApp;
using Inventor;
using AddinGlobal = InvApp.StandardAddInServer;

namespace InvUtils
{
    public class InvRibbons
    {
        public enum InvRibbonName
        {
            ZeroDoc,
            Part,
            Assembly,
            Drawing
        }
        
        public static void AddButtonToTab(InvRibbonName nomeRibbon)
        {
            Ribbon ribbon = AddinGlobal.m_InvApp.UserInterfaceManager.Ribbons[nomeRibbon];

            RibbonTab ribbonTab = ribbon.RibbonTabs.Add("NewAge", "KeepIntegration", Guid.NewGuid().ToString());


            //assyPanel.CommandControls.AddButton(btnNewAge.ButtonDef, true, true, "", btnNewAge.InsertBeforeTarget);
        }
    }
}