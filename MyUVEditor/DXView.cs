﻿using System;
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
            camera = new Camera();
        }
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

        public Result Render(MaterialManager mm)
        {
            Device device = swapChain.Device;
            using (Surface surface = swapChain.GetBackBuffer(0))
            {
                device.SetRenderTarget(0, surface);
                device.DepthStencilSurface = depthSurface;
                device.Viewport = Viewport;
            }
            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, mm.BGColor, 1.0f, 0);
            device.BeginScene();
            device.SetTransform(TransformState.View, camera.TransformView());
            device.SetTransform(TransformState.Projection, camera.TransformProjection(device, 1));
            device.EndScene();
            return swapChain.Present(Present.None);
        }

        public void ResizeEnd()
        {
            Device device;
            PresentParameters pp;
            DisposeResource(out device, out pp);
            pp.BackBufferWidth = ClientSize.Width;
            pp.BackBufferHeight = ClientSize.Height;
            InitResource(device, pp);
        }

        public void InitResource(Device device, PresentParameters pp)
        {
            if (pp == null)
            {
                pp = DeviceManager.GetPresentParameters(this);
            }
            swapChain = new SwapChain(device, pp);
            depthSurface = Surface.CreateDepthStencil(device,
                ClientSize.Width, ClientSize.Height,
                pp.AutoDepthStencilFormat, MultisampleType.None, 0, true);

        }

        public void InitResource(SwapChain swapChain)
        {
            this.swapChain = swapChain;
            depthSurface = Surface.CreateDepthStencil(swapChain.Device,
                ClientSize.Width, ClientSize.Height,
                swapChain.PresentParameters.AutoDepthStencilFormat,
                MultisampleType.None, 0, true);
        }

        public void DisposeResource(out Device device, out PresentParameters pp)
        {
            if (depthSurface != null)
                depthSurface.Dispose();

            if (swapChain == null)
            {
                device = null;
                pp = null;
                return;
            }
            device = swapChain.Device;
            pp = swapChain.PresentParameters;
            for (int i = 0; i < swapChain.PresentParameters.BackBufferCount; i++)
                swapChain.GetBackBuffer(i).Dispose();

            swapChain.Dispose();
        }
        public new void Dispose()
        {
            Device device;
            PresentParameters pp;
            DisposeResource(out device, out pp);
            base.Dispose();
        }
    }
}