using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PEPlugin.Pmx;

namespace MyUVEditor
{
    public partial class ParentForm : Form
    {
        public ParentForm(IPXPmx pmx)
        {
            InitializeComponent();
            ModelViewer = new DXViewForm();
            UVEditor = new DXViewForm();
            DXViewers = new DXViewForm[] { UVEditor, ModelViewer };
            DeviceM = new DeviceManager(DXViewers);
            PMX = PMXMesh.GetPMXMesh(DeviceM.Device,pmx);
            DeviceM.Render(DXViewers, PMX);

            foreach (var v in DXViewers)
            {
                v.Resize += (o, args) => { DeviceM.Render(DXViewers, PMX); };
                v.ResizeEnd += (o, args) => { DeviceM.Render(DXViewers, PMX); };
            }
        }
        public DeviceManager DeviceM { get; private set; }
        public DXViewForm[] DXViewers { get; private set; }
        private DXViewForm UVEditor { get; set; }
        private DXViewForm ModelViewer { get;  set; }
        public PMXMesh PMX { get; private set; }
    }
}
