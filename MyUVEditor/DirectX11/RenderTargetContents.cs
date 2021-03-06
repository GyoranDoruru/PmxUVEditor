﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using MyUVEditor.Camera;
using MyUVEditor.Light;

namespace MyUVEditor.DirectX11
{
    class RenderTargetContents : IDisposable
    {
        private SwapChain SwapChain { get; set; }
        private RenderTargetView RenderTarget { get; set; }
        private DepthStencilView DepthStencil { get; set; }
        protected SlimDX.Direct3D11.Device GraphicsDevice { get; private set; }
        private EffectManager11 EffectManager { get; set; }
        protected Control Client { get; private set; }
        protected DrawablePmx DrawablePmx { get; private set; }
        protected ICamera Camera { get; set; }
        protected ILight Light { get; private set; }
        protected SlimDX.Color4 m_backgroundColor = new SlimDX.Color4(1.0f, 0.39f, 0.58f, 0.93f);
        public bool ClientIsDisposed { get { return Client.IsDisposed; } }

        public RenderTargetContents(SlimDX.Direct3D11.Device device, SwapChain swapChain, Control client)
        {
            GraphicsDevice = device;
            SwapChain = swapChain;
            Client = client;

            GetParentForm(Client).ResizeEnd += ClientResize;

            initRenderTarget();
            initDepthStencil();
        }

        virtual public void Draw()
        {
            try
            {
                SetTargets();
                EffectManager.SetCamera(Camera);
                EffectManager.SetLight(Light);
                DrawContents();
                SwapChain.Present(1, PresentFlags.None);
            }
            catch (SlimDX.SlimDXException e)
            {
                MessageBox.Show(e.Message);
            }
        }
        virtual public void Dispose()
        {
            UnloadContent();
            RenderTarget.Dispose();
            DepthStencil.Dispose();
            SwapChain.Dispose();
            if(Client!=null&&!Client.IsDisposed)
                GetParentForm(Client).ResizeEnd -= ClientResize;
            GraphicsDevice = null;
            Client = null;
        }

        private void SetTargets()
        {
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(DepthStencil, RenderTarget);
            GraphicsDevice.ImmediateContext.Rasterizer.SetViewports(
                new Viewport
                {
                    Width = Client.ClientSize.Width,
                    Height = Client.ClientSize.Height,
                    MaxZ = 1
                });

            GraphicsDevice.ImmediateContext.ClearRenderTargetView(RenderTarget, m_backgroundColor);
            GraphicsDevice.ImmediateContext.ClearDepthStencilView(DepthStencil, DepthStencilClearFlags.Depth, 1, 0);
        }
        private void initRenderTarget()
        {
            using (Texture2D backBuffer
                = SlimDX.Direct3D11.Resource.FromSwapChain<Texture2D>(SwapChain, 0)
                )
            {
                RenderTarget = new RenderTargetView(GraphicsDevice, backBuffer);
            }
        }
        private void initDepthStencil()
        {
            var swapChainSampleDesc = SwapChain.Description.SampleDescription;
            Texture2DDescription depthBufferDesc = new Texture2DDescription
            {
                ArraySize = 1,
                BindFlags = BindFlags.DepthStencil,
                Format = Format.D32_Float,
                Width = Client.ClientSize.Width,
                Height = Client.ClientSize.Height,
                MipLevels = 1,
                SampleDescription = new SampleDescription(swapChainSampleDesc.Count, swapChainSampleDesc.Quality)
            };

            using (Texture2D depthBuffer = new Texture2D(GraphicsDevice, depthBufferDesc))
            {
                DepthStencil = new DepthStencilView(GraphicsDevice, depthBuffer);
            }
        }

        private void ClientResize(object sender, EventArgs args)
        {
            RenderTarget.Dispose();
            DepthStencil.Dispose();
            System.Drawing.Size size = Client.ClientSize;
            SwapChain.ResizeBuffers(1, size.Width, size.Height, Format.R8G8B8A8_UNorm, SwapChainFlags.None);
            initRenderTarget();
            initDepthStencil();
        }

        protected virtual void DrawContents()
        {
            DrawablePmx.ResetForDraw();
            DrawablePmx.Draw();
        }

        public virtual void LoadContent(ICommonContents commonContents) {
            EffectManager = new EffectManager11(commonContents.Effect);
            InitCamera();
            InitLight();
            DrawablePmx = (DrawablePmx)commonContents.CommonDrawables[0];

        }
        public virtual void UnloadContent() {
            DrawablePmx.Dispose();
            EffectManager.Dispose();
        }

        protected virtual void InitCamera()
        {
            if (Camera != null)
                Camera.Dispose();
            Camera = new Camera3D(Client);
        }

        protected virtual void InitLight()
        {
            if (Light != null)
                Light.Dispose();
            Light = new ParallelLight();

        }

        static internal Form GetParentForm(Control control)
        {
            Control tmp_c = control;
            Form form = null;
            for (; form == null; tmp_c = tmp_c.Parent)
            {
                form = tmp_c as Form;
            }
            return form;
        }

        internal bool IsFocused()
        {
            for (Control tmp_c = Client; tmp_c != null; tmp_c = tmp_c.Parent)
            {
                if (tmp_c.Focused)
                    return true;
            }
            return false;
        }

    }
}