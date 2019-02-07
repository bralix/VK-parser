using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace VKParser
{
    public partial class outputForm : Form
    {
        private Task _outputLoopTask;

        public outputForm()
        {
            InitializeComponent();
            _outputLoopTask = new Task(outputLoopTask);
        }

        private void outputLoopTask()
        {
            while (true)
            {
                if (Parser.isReady)
                {
                    Action action = new Action(() =>
                    {
                        webBrowser1.DocumentText =
                           File.ReadAllText(Parser.jsonFolder + "template.html").Replace("{content}", Parser.strToLoad);
                    });

                    webBrowser1.Invoke(action);
                    break;
                }
            }
        }

        private void output_Load(object sender, EventArgs e)
        {
            _outputLoopTask.Start();
        }

        private void output_FormClosing(object sender, FormClosingEventArgs e)
        {
            _outputLoopTask.Wait();
        }
    }
}
