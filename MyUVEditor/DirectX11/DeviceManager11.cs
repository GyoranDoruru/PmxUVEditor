using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using SlimDX.Direct3D11;
using SlimDX.DXGI;



namespace MyUVEditor.DirectX11
{
    class DeviceManager11:IDisposable
    {
        private SlimDX.Direct3D11.Device m_Device;
        private RenderTargetSet[] m_RenderTargets;
        private ICommonContents m_CommonContents;

        private void CreateDeviceAndSwapChain( Control control, out SwapChain swapChain)
        {
            SlimDX.Direct3D11.Device.CreateWithSwapChain(
                DriverType.Hardware,
                DeviceCreationFlags.None,
                DefaultSwapChainDescription(control),
                out m_Device,
                out swapChain
                );
        }

        private SlimDX.Direct3D11.Buffer CreateVertexBuffer(Array vertices)
        {
            using (SlimDX.DataStream vertexStream
                = new SlimDX.DataStream(vertices, true, true))
            {
                return new SlimDX.Direct3D11.Buffer(
                    m_Device,
                    vertexStream,
                    new BufferDescription
                    {
                        SizeInBytes = (int)vertexStream.Length,
                        BindFlags = BindFlags.VertexBuffer,
                    }
                    );
            }
        }

        private void CreateDeviceAndMultiSwapChain(Control[] controls)
        {
            m_RenderTargets = new RenderTargetSet[controls.Length];
            SwapChain tmp_Swap;
            CreateDeviceAndSwapChain(controls[0], out tmp_Swap);
            m_RenderTargets[0] = new RenderTargetSet(m_Device,tmp_Swap, controls[0]);
            Factory factory = m_Device.Factory;
            for (int i = 1; i < controls.Length; i++)
            {
                tmp_Swap
                    = new SwapChain(factory,m_Device,DefaultSwapChainDescription(controls[i]));
                m_RenderTargets[i] = new RenderTargetSet(m_Device, tmp_Swap, controls[i]);
            }
        }

        static private SwapChainDescription DefaultSwapChainDescription(Control control)
        {
            return new SwapChainDescription
            {
                BufferCount = 1,
                OutputHandle = control.Handle,
                IsWindowed = true,
                SampleDescription = new SampleDescription
                {
                    Count = 1,
                    Quality = 0
                },
                ModeDescription = new ModeDescription
                {
                    Width = control.ClientSize.Width,
                    Height = control.ClientSize.Height,
                    RefreshRate = new SlimDX.Rational(60, 1),
                    Format = Format.R8G8B8A8_UNorm
                },
                Usage = Usage.RenderTargetOutput
            };
        }

        private void LoadCommonContent() { m_CommonContents.Load(m_Device); }
        private void UnloadCommonContent() { m_CommonContents.Unload(); }

        public void Run(Control[] clients)
        {
            CreateDeviceAndMultiSwapChain(clients);
            LoadCommonContent();
            SlimDX.Windows.MessagePump.Run(RenderTargetSet.GetParentForm(clients[0]), Draw);
            Dispose();
        }

        public void SetCommonContents(ICommonContents commonContents, IPERunArgs args)
        {
            m_CommonContents = commonContents;
            m_CommonContents.Set(args);
        }

        private void Draw()
        {
            bool isEnd = true;
            foreach (var rt in m_RenderTargets)
            {
                isEnd &= rt.ClientIsDisposed;
                rt.Draw(m_CommonContents);
            }
        }

        public void Dispose()
        {
            foreach (var rt in m_RenderTargets)
                rt.Dispose();
            UnloadCommonContent();
            m_Device.Dispose();
        }
    }

}
