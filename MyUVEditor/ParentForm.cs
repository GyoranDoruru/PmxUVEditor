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
            DXViewers = new IDXViewForm[] { new UVEditorForm("test"), new DXViewForm() };
            DXViewers[0].Text = "main";
            DXViewers[1].Text = "sub";
            DeviceM = new DeviceManager(DXViewers);
            this.PERunArgs = peRunArgs;
            PMX = PMXMesh.GetPMXMesh(DeviceM.Device,peRunArgs.Host.Connector.Pmx.GetCurrentState());
            DeviceM.Render(DXViewers, PMX);

            foreach (var v in DXViewers)
            {
                v.ViewPort.RequestRender += (o, args) => { DeviceM.Render(DXViewers, PMX); };

                //v.Resize += (o, args) => { DeviceM.Render(DXViewers, PMX); };
                //v.ResizeEnd += (o, args) => { DeviceM.Render(DXViewers, PMX); };
            }
        }

        public IPERunArgs PERunArgs { get; private set; }
        public DeviceManager DeviceM { get; private set; }
        public IDXViewForm[] DXViewers { get; private set; }
        private IDXViewForm UVEditor { get { return DXViewers[0]; } }
        private IDXViewForm ModelViewer { get { return DXViewers[1]; } }
        public PMXMesh PMX { get; private set; }
    }
}
