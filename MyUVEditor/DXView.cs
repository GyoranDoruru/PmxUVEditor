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
            camera = new Camera(this);
        }
        static int swapCount = 0;
        //private bool primary;
        private Camera camera;
        private SwapChain swapChain;
        private Surface depthSurface;
        private Viewport Viewport
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
            c.SizeChanged += (o, args) => { this.Resize(); };
            c.ResizeEnd += (o, args) => { this.ResizeEnd(); };
            c.MouseWheel += (o, args) => { camera.CameraDolly(args.Delta); RequestRender(this, EventArgs.Empty); };
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
            device.SetTransform(TransformState.View, camera.View);
            device.SetTransform(TransformState.Projection, camera.Projection);
            //カリング無視

            device.SetRenderState(RenderState.CullMode, Cull.Counterclockwise);

            //アルファブレンド
            device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
            device.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);
            device.SetRenderState(RenderState.AlphaBlendEnable, true);

            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, pmx.MatManager.BGColor, 1.0f, 0);
            device.BeginScene();
            for (int i = 0; i < pmx.ExMaterialArray.Length; i++)
            {
                device.Material = pmx.GetMaterials()[i].MaterialD3D;
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
            camera.SetClientSize(this);
            RequestRender(this, EventArgs.Empty);
        }

        public void ResizeEnd()
        {
            Device device;
            DisposeResource(out device);
            InitResource(device);
            camera.SetClientSize(this);
            RequestRender(this, EventArgs.Empty);
        }

        ///// <summary>
        ///// サブウィンドウ用リソース初期化
        ///// </summary>
        ///// <param name="device"></param>
        ///// <param name="pp"></param>
        //public void InitResource(Device device, PresentParameters pp)
        //{
        //    if (pp == null)
        //    {
        //        pp = DeviceManager.GetPresentParameters(this);
        //    }
        //    swapChain = new SwapChain(device, pp);
        //    depthSurface = Surface.CreateDepthStencil(device,
        //        ClientSize.Width, ClientSize.Height,
        //        pp.AutoDepthStencilFormat, MultisampleType.None, 0, true);

        //}
        ///// <summary>
        ///// メインウィンドウ用リソース初期化
        ///// </summary>
        ///// <param name="swapChain"></param>
        //public void InitResource(SwapChain swapChain)
        //{
        //    this.swapChain = swapChain;
        //    depthSurface = Surface.CreateDepthStencil(swapChain.Device,
        //        ClientSize.Width, ClientSize.Height,
        //        swapChain.PresentParameters.AutoDepthStencilFormat,
        //        MultisampleType.None, 0, true);
        //}

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
    }
}
