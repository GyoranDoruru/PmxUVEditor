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
            Camera = new Camera(this);
        }
        static int swapCount = 0;
        public Camera Camera { get; protected set; }
        protected SwapChain swapChain;
        protected Surface depthSurface;
        protected Viewport Viewport
        {
            get
            {
                return new Viewport
                {
                    Width = ClientSize.Width,
                    Height = ClientSize.Height,
                    MaxZ = 1.0f,
                };
            }
        }
        public event EventHandler RequestRender;

        public void SetRequest(Form c)
        {
            this.SizeChanged += (o, args) => { this.Resize(); };
            c.ResizeEnd += (o, args) => { this.ResizeEnd(); };
            this.MouseWheel += (o, args) => { RequestRender(this, EventArgs.Empty); };
            this.MouseMove += (o, args) => { DXView_MouseMove(this, args); };
        }

        public Result Render(PMXMesh pmx)
        {
            Device device = swapChain.Device;
            device.SetRenderTarget(0, swapChain.GetBackBuffer(0));

            device.SetRenderState(RenderState.ZEnable, true);
            device.DepthStencilSurface = depthSurface;
            device.Viewport = Viewport;
            device.SetRenderState(RenderState.Lighting, true);
            device.SetLight(0, pmx.MatManager.Light);
            device.EnableLight(0, true);
            device.SetTransform(TransformState.View, Camera.View);
            device.SetTransform(TransformState.Projection, Camera.Projection);
            //カリング無視

            device.SetRenderState(RenderState.CullMode, Cull.Counterclockwise);

            //アルファブレンド
            device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
            device.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);
            device.SetRenderState(RenderState.AlphaBlendEnable, true);

            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, pmx.MatManager.BGColor, 1.0f, 0);
            device.BeginScene();
            for (int i = 0; i < pmx.Pmx.Material.Count; i++)
            {
                ExtendedMaterial m = pmx.GetMaterials()[i];
                device.Material = m.MaterialD3D;
                device.SetTexture(0, pmx.MatManager.DicObjTex[m.TextureFileName]);
                pmx.DrawSubset(i);
            }
            device.EndScene();
            return swapChain.Present(Present.None);
        }

        public new void Resize()
        {
            Device device = swapChain.Device;
            swapChain.PresentParameters.BackBufferHeight = ClientSize.Height;
            swapChain.PresentParameters.BackBufferWidth = ClientSize.Width;
            Camera.SetClientSize(this);
            RequestRender(this, EventArgs.Empty);
        }

        public void ResizeEnd()
        {
            Device device;
            DisposeResource(out device);
            InitResource(device);
            Camera.SetClientSize(this);
            RequestRender(this, EventArgs.Empty);
        }

        public void InitResource(Device device)
        {
            if (swapCount == 0)
            {
                swapChain = device.GetSwapChain(0);
                depthSurface = device.DepthStencilSurface;
            }
            else
            {
                PresentParameters pp = DeviceManager.GetPresentParameters(this);
                swapChain = new SwapChain(device, pp);
                depthSurface = Surface.CreateDepthStencil(
                    device, ClientSize.Width, ClientSize.Height,
                    swapChain.PresentParameters.AutoDepthStencilFormat
                    , DeviceManager.tmpSample, DeviceManager.tmpQuality - 1, true);
            }
            swapCount++;
        }
        public void DisposeResource(out Device device)
        {

            device = swapChain.Device;
            for (int i = 0; i < swapChain.PresentParameters.BackBufferCount; i++)
                swapChain.GetBackBuffer(i).Dispose();

            depthSurface.Dispose();
            swapChain.Dispose();
        }

        public new void Dispose()
        {
            Device device;
            DisposeResource(out device);
            base.Dispose();
        }

        public Point Prev_Point { get; protected set; }
        public Point L_Down { get; protected set; }
        public Point M_Down { get; protected set; }
        public Point R_Down { get; protected set; }
        public bool isL_Down { get; protected set; }
        public bool isM_Down { get; protected set; }
        public bool isR_Down { get; protected set; }
        protected void DXView_MouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    L_Down = e.Location;
                    isL_Down = true;
                    break;
                case MouseButtons.Middle:
                    M_Down = e.Location;
                    isM_Down = true;
                    break;
                case MouseButtons.Right:
                    R_Down = e.Location;
                    isR_Down = true;
                    break;
            }
        }

        protected void DXView_MouseMove(object sender, MouseEventArgs e)
        {
            if (Prev_Point == e.Location) return;
            Prev_Point = e.Location;
            RequestRender(this, EventArgs.Empty);
        }

        protected void DXView_MouseUp(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    isL_Down = false;
                    break;
                case MouseButtons.Middle:
                    isM_Down = false;
                    break;
                case MouseButtons.Right:
                    isR_Down = false;
                    break;
            }
        }

    }
}
