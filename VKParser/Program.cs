using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VKParser
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Task outputThread = new Task(() => { Application.Run(new parserForm()); });
            outputThread.Start();
            Application.Run(new outputForm());
        }
    }
}
