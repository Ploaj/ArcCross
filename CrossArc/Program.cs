using CrossArc.GUI;
using System;
using System.Windows.Forms;

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
