using System;
using ArcCross;
using System.Windows.Forms;
using CrossArc.GUI;

namespace CrossArc
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();

            Application.SetCompatibleTextRenderingDefault(false);
            using (var form = new MainForm())
            {
                Application.Run(form);
            }
        }
    }
}
