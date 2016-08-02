using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using KBS.CHANDRA.SSC.DATAMODEL;
using KBS.CHANDRA.SSC.FUNCTION;

namespace KBS.CHANDRA.SSC.GUI
{
    public partial class ProgressBarForm : Form
    {

        public ProgressBarForm()
        {

            InitializeComponent();
        }

        private void ProgressBarForm_Load(object sender, EventArgs e)
        {

        }

        public string Message
        {
            set { labelMessage.Text = value; }
            get { return labelMessage.Text; }     
        }

        public int ProgressValue
        {
            set { progressBar1.Value = value; }
            get { return progressBar1.Value; }    
        }

        public int ProgressBarMaxValue
        {
            set { progressBar1.Maximum = value; }
            get { return progressBar1.Maximum; }    
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }




        public event EventHandler<EventArgs> Canceled;

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            EventHandler<EventArgs> ea = Canceled;
            /* If there are no subscribers, ea will be null so we need to check
                * to avoid a NullReferenceException. */
            if (ea != null)
                ea(this, e);
        }
    }
}
