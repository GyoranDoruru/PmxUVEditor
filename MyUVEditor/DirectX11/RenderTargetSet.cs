using System;
using System.Windows.Forms;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
namespace MyUVEditor.DirectX11
{
    class RenderTargetSet : IDisposable
    {
        public SwapChain SwapChain { get; private set; }
        public RenderTargetView RenderTarget { get; private set; }
        public DepthStencilView DepthStencil { get; private set; }
        protected SlimDX.Direct3D11.Device m_device;
        protected Control m_client;
        protected SlimDX.Color4 m_backgroundColor = new SlimDX.Color4(1.0f, 0.39f, 0.58f, 0.93f);
        public RenderTargetSet(SlimDX.Direct3D11.Device device, SwapChain swapChain, Control client)
        {
            m_device = device;
            SwapChain = swapChain;
            m_client = client;
            LoadContent();
        }

        public void Draw(ICommonContents commonContents)
        {
            SetTargets();
            DrawContents(commonContents);
            SwapChain.Present(1, PresentFlags.None);
        }

        public void Dispose()
        {
            UnloadContent();
            RenderTarget.Dispose();
            DepthStencil.Dispose();
            SwapChain.Dispose();
            m_device = null;
            m_client = null;
        }

        private void SetTargets()
        {
            m_device.ImmediateContext.OutputMerger.SetTargets(DepthStencil, RenderTarget);
            m_device.ImmediateContext.Rasterizer.SetViewports(
                new Viewport
                {
                    Width = m_client.ClientSize.Width,
                    Height = m_client.ClientSize.Height,
                    MaxZ = 1
                });

            m_device.ImmediateContext.ClearRenderTargetView(RenderTarget, m_backgroundColor);
            m_device.ImmediateContext.ClearDepthStencilView(DepthStencil, DepthStencilClearFlags.Depth, 1, 0);
        }

        protected virtual void DrawContents(ICommonContents commonContents) { }
        protected virtual void LoadContent() { }
        protected virtual void UnloadContent() { }

    }
    interface ICommonContents
    {
        void Load();
        void Unload();
    }
}