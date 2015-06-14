using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
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
        protected IList<IDrawable> DrawableList { get; private set; }
        protected SlimDX.Color4 m_backgroundColor = new SlimDX.Color4(1.0f, 0.39f, 0.58f, 0.93f);
        public bool ClientIsDisposed { get { return Client.IsDisposed; } }

        public RenderTargetContents(SlimDX.Direct3D11.Device device, SwapChain swapChain, Control client)
        {
            DrawableList = new List<IDrawable>();
            GraphicsDevice = device;
            SwapChain = swapChain;
            Client = client;

            GetParentForm(Client).ResizeEnd += ClientResize;
            initRenderTarget();
            initDepthStencil();
        }

        public void Draw()
        {
            SetTargets();
            DrawContents();
            SwapChain.Present(1, PresentFlags.None);
        }
        public void Dispose()
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
            Texture2DDescription depthBufferDesc = new Texture2DDescription
            {
                ArraySize = 1,
                BindFlags = BindFlags.DepthStencil,
                Format = Format.D32_Float,
                Width = Client.ClientSize.Width,
                Height = Client.ClientSize.Height,
                MipLevels = 1,
                SampleDescription = new SampleDescription(1, 0)
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
            foreach (var d in DrawableList)
                d.Draw();
        }
        public virtual void LoadContent(ICommonContents commonContents) {
            Drawable drawable = new Drawable(null);
            drawable.SetEffectManager(new EffectManager11(commonContents.Effect), false);
            drawable.setVertexLayout(commonContents.VertexLayout, true, PmxVertexStruct.SizeInBytes);
            drawable.SetVertexBuffer(commonContents.VertexBuffer, true);
            drawable.SetTexture(commonContents.GetTexture(""), true);

            DrawableList.Add(drawable);

        }
        public virtual void UnloadContent() { foreach (var d in DrawableList)d.Dispose(); }

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

    }
}