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
                if (!IsFairPmx(args))
                    return;
                Connector.Args = args;
                ParentForm PF = new ParentForm(args);
                PF.Show();
                PF.Worker.RunWorkerAsync();
                PF.TopMost = true;
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

        private bool IsFairPmx(IPERunArgs args)
        {
            var pmx = args.Host.Connector.Pmx.GetCurrentState();
            if (pmx.Vertex.Count == 0 || pmx.Material.Count == 0)
            {
                MessageBox.Show("モデルが読み込まれてないかも？");
                return false;
            }

            foreach (var m in pmx.Material)
            {
                if (m.Faces.Count == 0)
                {
                    MessageBox.Show("材質: " + m.Name + "の面の数が0なので中断します");
                    return false;
                }
            }
            return true;
        }

    }
}
