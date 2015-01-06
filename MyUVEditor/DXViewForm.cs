using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SlimDX;
using SlimDX.Direct3D9;
namespace MyUVEditor
{
    public partial class DXViewForm : Form
    {
        public DXView ViewPort { get { return this.dxView1; } }
        public DXViewForm()
        {
            InitializeComponent();
            ViewPort.SetRequest(this);
        }
        
    }
}
