using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WUtils
{
    public static class ProgressBarControl
    {
        /// <summary>
        /// Star do Progress Bar
        /// </summary>
        /// <param name="descTask"></param>
        public static void Start(string descTask, ProgressBar prgBar)
        {
            prgBar.Visible = true;
            prgBar.Text = descTask;
            prgBar.Style = ProgressBarStyle.Marquee;
            prgBar.MarqueeAnimationSpeed = 30;
        }

        /// <summary>
        /// Star do Progress Bar
        /// </summary>
        /// <param name="descTask"></param>
        public static void Start(string descTask, ToolStripProgressBar prgBar)
        {
            prgBar.Visible = true;
            prgBar.Text = descTask;
            prgBar.Style = ProgressBarStyle.Marquee;
            prgBar.MarqueeAnimationSpeed = 30;
        }

        /// <summary>
        /// Star do Progress Bar
        /// </summary>
        /// <param name="descTask"></param>
        public static void Start(string descTask, ToolStripProgressBar prgBar, ToolStripLabel label)
        {
            label.Text = descTask;
            prgBar.Visible = true;
            prgBar.Text = descTask;
            prgBar.Style = ProgressBarStyle.Marquee;
            prgBar.MarqueeAnimationSpeed = 30;
        }


        /// <summary>
        /// Stop do Progress Bar
        /// </summary>
        /// <param name="descTask"></param>
        public static void Stop(string descTask, ProgressBar prgBar)
        {
            prgBar.Text = descTask;
            prgBar.Style = ProgressBarStyle.Blocks;
            prgBar.Value = 0;
            prgBar.MarqueeAnimationSpeed = 0;
            prgBar.Visible = false;
        }

        public static void Stop(string descTask, ToolStripProgressBar prgBar)
        {
            prgBar.Text = descTask;
            prgBar.Style = ProgressBarStyle.Blocks;
            prgBar.Value = 0;
            prgBar.MarqueeAnimationSpeed = 0;
            prgBar.Visible = false;
        }

        public static void Stop(string descTask, ToolStripProgressBar prgBar, ToolStripLabel label)
        {
            label.Text = descTask;
            prgBar.Text = descTask;
            prgBar.Style = ProgressBarStyle.Blocks;
            prgBar.Value = 0;
            prgBar.MarqueeAnimationSpeed = 0;
            prgBar.Visible = false;
        }

    }
}
