using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using PluginDummyStarter;
using MyUVEditor;
using PEPlugin;
using PEPlugin.Pmx;

namespace Starter
{
    class Program
    {
        [STAThreadAttribute]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
        static void DoWork(object o)
        {
            object[] objects = (object[])o;
            IPEPlugin plugin = (IPEPlugin)objects[0];
            IPERunArgs arg = (IPERunArgs)objects[1];
            plugin.Run(arg);
        }
    }
}
