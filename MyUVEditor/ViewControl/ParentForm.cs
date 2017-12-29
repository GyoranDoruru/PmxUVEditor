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
        public ParentForm(IPERunArgs peRunArgs,string version)
        {
            InitializeComponent();
            PERunArgs = peRunArgs;
            m_version = version;
        }

        public IPERunArgs PERunArgs { get; private set; }
        public BackgroundWorker Worker { get { return backgroundWorker1; } }

        private string m_version;
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            using (DirectX11.DeviceManager11 deviceManager = new DirectX11.DeviceManager11())
            {
            Control[] controls = new Control[2];
                UVEditorMainForm mainForm = new UVEditorMainForm(m_version);
                controls[0] = mainForm.ViewPanel;
                Form subForm = new Form();
                controls[1] = subForm;
                mainForm.Show();
                subForm.Show();

                deviceManager.SetCommonContents(new DirectX11.MyCommonContents(),PERunArgs);
                deviceManager.Run(controls, backgroundWorker1);
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            label1.Text = e.UserState.ToString();
            this.TopMost = false;
            progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            label1.Text = "UVエディタ終了";
            this.Close();
        }
    }
}
