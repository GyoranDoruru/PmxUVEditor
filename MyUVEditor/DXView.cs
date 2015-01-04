using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SlimDX;
using SlimDX.Direct3D9;
namespace MyUVEditor
{
    public partial class DXView : UserControl
    {
        public DXView()
        {
            InitializeComponent();
        }
        private Camera camera;
        private SwapChain swapChain;
        private void Render(MaterialManager mm)
        {
            Device device = swapChain.Device;
            using (Surface surface = swapChain.GetBackBuffer(0))
            {
                device.SetRenderTarget(0, surface);
            }
            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, mm.BGColor, 1.0f, 0);
            device.BeginScene();
            device.SetTransform(TransformState.View,camera.TransformView());
            device.SetTransform(TransformState.Projection, camera.TransformProjection(device, 1));
            device.EndScene();
        }
    }
}
