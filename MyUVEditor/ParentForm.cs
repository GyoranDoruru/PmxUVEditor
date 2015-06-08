using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PEPlugin;
using PEPlugin.Pmx;

namespace MyUVEditor
{
    public partial class ParentForm : Form
    {
        public ParentForm(IPERunArgs peRunArgs)
        {
            InitializeComponent();
            PERunArgs = peRunArgs;
        }

        public IPERunArgs PERunArgs { get; private set; }
        public BackgroundWorker Worker { get { return backgroundWorker1; } }

        private Form[] m_Forms = new Form[2];
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            using (DirectX11.DeviceManager11 deviceManager = new DirectX11.DeviceManager11())
            {

                for (int i = 0; i < m_Forms.Length; i++)
                {
                    m_Forms[i] = new Form();
                    m_Forms[i].Text = i.ToString();
                    m_Forms[i].Show();
                }
                deviceManager.SetCommonContents(new DirectX11.DummyCommonContents());
                deviceManager.Run(m_Forms);
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            label1.Text = e.UserState.ToString();
            progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            label1.Text = "UVエディタ終了";
            this.Close();
        }
    }
}
