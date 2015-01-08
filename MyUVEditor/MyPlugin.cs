using System;
using System.Windows.Forms;
using PEPlugin;

namespace MyUVEditor
{
    public class MyPlugin : PEPluginClass
    {
        //MyGame game;
        const string ver = "0_139_10_2";
        //Thread _thread;
        public MyPlugin()
            : base()
        {
            // 起動オプション
            // boot時実行(true/false), プラグインメニューへの登録(true/false), メニュー登録名("")
            m_option = new PEPluginOption(false, true, "UVエディタ" + ver);
        }
        // エントリポイント
        public override void Run(IPERunArgs args)
        {
            try
            {
                //game = new MyGame(args.Host,ver);
                //game.Run();
                ParentForm PF = new ParentForm(args.Host.Connector.Pmx.GetCurrentState());
                //_thread = new Thread(new ThreadStart(game.Run));
                //_thread.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //game.Dispose();
            }

        }
    }
}
