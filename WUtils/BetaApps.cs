using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventor;
using iApp = InvApp.StandardAddInServer;
using System.Diagnostics;
namespace WUtils
{
    public class BetaApps
    {

        /// <summary>
        /// Cria uma Janela _ Mas ainda não funcionou
        /// </summary>
        public static void DockableWindow()
        {
            UserInterfaceManager uiManager = iApp.m_InvApp.UserInterfaceManager;

            DockableWindow oWindow = uiManager.DockableWindows.Add("Text01", "Text02", "Text03");

            long hwnd = iApp.m_InvApp.MainFrameHWND;

            oWindow.AddChild(hwnd);

            oWindow.DisabledDockingStates = DockingStateEnum.kDockTop;
            oWindow.DisabledDockingStates = DockingStateEnum.kDockBottom;  //kDockTop + kDockBottom;

            oWindow.Visible = true;

        }


        public static void Select_A()
        {
            PartDocument oDoc = (PartDocument)iApp.m_InvApp.ActiveEditDocument;

            Face oFace1 = oDoc.ComponentDefinitions[1].SurfaceBodies[1].Faces[1]; // (Face) iApp.InventorApp.CommandManager.Pick(SelectionFilterEnum.kPartFaceFilter, "Selecione uma Face");
            Face oFace2 = oDoc.ComponentDefinitions[1].SurfaceBodies[1].Faces[3]; // (Face) iApp.InventorApp.CommandManager.Pick(SelectionFilterEnum.kPartFaceFilter, "Selecione uma Face");

            HighlightSet oSet1 = oDoc.CreateHighlightSet();
            HighlightSet oSet2 = oDoc.CreateHighlightSet();

            oSet1.Color = iApp.m_InvApp.TransientObjects.CreateColor(0, 200, 8);
            oSet2.Color = iApp.m_InvApp.TransientObjects.CreateColor(150, 200, 8);

            oSet1.AddItem(oFace1);
            oSet2.AddItem(oFace2);

            oFace1.AttributeSets.Add("Valeu");

        }



        public static void Select_B()
        {
            PartDocument oDoc = (PartDocument)iApp.m_InvApp.ActiveEditDocument;

            Face oFace1 = (Face)iApp.m_InvApp.CommandManager.Pick(SelectionFilterEnum.kPartFaceFilter, "Selecione uma Face");
            HighlightSet oSet1 = oDoc.CreateHighlightSet();
            int num = 0;

            foreach (EdgeLoop eLoop in oFace1.EdgeLoops)
            {
                Debug.WriteLine($"{++num} -------");
                Debug.WriteLine(eLoop.Type.ToString());
                Debug.WriteLine(eLoop.IsOuterEdgeLoop.ToString());
                Debug.WriteLine(eLoop.Edges.Count.ToString());
                Debug.WriteLine(eLoop.Edges.Type.ToString());
                Debug.WriteLine("###-------");


                oSet1.Color = iApp.m_InvApp.TransientObjects.CreateColor(0, 200, 8);
                oSet1.AddItem(eLoop);


                var result = System.Windows.Forms.MessageBox.Show("continuar?", "", System.Windows.Forms.MessageBoxButtons.YesNo);

                if (result == System.Windows.Forms.DialogResult.No) break;
            }



        }


        public static void GetHoles()
        {
            PartDocument oDoc = (PartDocument)iApp.m_InvApp.ActiveEditDocument;


            foreach (SurfaceBodies item in oDoc.ComponentDefinition.SurfaceBodies)
            {

            }

            //var face = oDoc.ComponentDefinition.SurfaceBodies[1];

            

        }




    }
}
