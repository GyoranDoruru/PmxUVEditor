using System;
using System.Windows.Forms;
using PEPlugin;
using System.Threading;

namespace MyUVEditor
{
    public class MyPlugin : IPEPlugin
    {
        const string ver = "0_222c_12_0";
        public MyPlugin()
            : base()
        {
            m_option = new PEPluginOption(false, true, "UVエディタ_" + Version);
        }
        // エントリポイント
        public void Run(IPERunArgs args)
        {
            try
            {
                ParentForm PF = new ParentForm(args);
                PF.Show();
                PF.Worker.RunWorkerAsync();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //game.Dispose();
            }

        }

        public string Description
        {
            get { return "どるる式UVエディタ"; }
        }

        public string Name
        {
            get { return Option.RegisterMenuText; }
        }

        private PEPluginOption m_option;
        public IPEPluginOption Option
        {
            get { return m_option; }
        }

        public string Version
        {
            get { return "0_222c_12_0"; }
        }

        public void Dispose()
        {
        }

    }
}
