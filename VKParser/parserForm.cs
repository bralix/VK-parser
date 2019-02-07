using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VKParser
{
    public partial class parserForm : Form
    {
        private Parser parser = new Parser();

        public parserForm()
        {
            InitializeComponent();
        }


        private void backgroundWorker_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {

        }

        private void pars_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            backgroundWorker.RunWorkerAsync();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            Close();
        }

        private void progressBar1_Click_1(object sender, EventArgs e)
        {

        }

        private void backgroundWorker_DoWork_1(object sender, DoWorkEventArgs e)
        {
            parser.TaskVK(backgroundWorker);
        }

        private void backgroundWorker_ProgressChanged_1(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }
    }
}
