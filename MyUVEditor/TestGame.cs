using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PEPlugin;

namespace MyUVEditor
{
    public partial class TestGame : Form
    {
        IPERunArgs args;
        public TestGame(IPERunArgs args)
        {
            InitializeComponent();
            this.args = args;
        }

        public void Run()
        {
            try
            {
                SlimDX.Windows.MessagePump.Run(this, Draw);
            }
            catch
            {

            }
        }
        int count = 0;
        protected void Draw()
        {
            System.Diagnostics.Debug.WriteLine(count);
            count++;
            if (count > 1000) count = 0;
        }
    }

}
