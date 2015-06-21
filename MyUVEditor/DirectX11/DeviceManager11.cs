using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using PEPlugin;



namespace MyUVEditor.DirectX11
{
    class DeviceManager11:IDisposable
    {
        private SlimDX.Direct3D11.Device m_Device;
        private RenderTargetContents[] m_RenderTargets;
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

        private void CreateDeviceAndMultiSwapChain(Control[] controls)
        {
            m_RenderTargets = new RenderTargetContents[controls.Length];
            SwapChain tmp_Swap;
            CreateDeviceAndSwapChain(controls[0], out tmp_Swap);
            m_RenderTargets[0] = new RenderTargetContents(m_Device,tmp_Swap, controls[0]);
            Factory factory = m_Device.Factory;
            for (int i = 1; i < controls.Length; i++)
            {
                tmp_Swap
                    = new SwapChain(factory,m_Device,DefaultSwapChainDescription(controls[i]));
                m_RenderTargets[i] = new RenderTargetContents(m_Device, tmp_Swap, controls[i]);
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
                    Count = 4,
                    Quality = 16
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
            foreach (var rt in m_RenderTargets)
            {
                rt.LoadContent(m_CommonContents);
            }
            SlimDX.Windows.MessagePump.Run(RenderTargetContents.GetParentForm(clients[0]), Draw);
            Dispose();
        }

        public void SetCommonContents(ICommonContents commonContents, IPERunArgs args)
        {
            m_CommonContents = commonContents;
            m_CommonContents.SetRunArgsAndPmx(args);
        }

        private void Draw()
        {
            bool isEnd = true;
            bool isFocused = false;
            foreach (var rt in m_RenderTargets)
            {
                isFocused = rt.IsFocused();
                if (isFocused)
                    break;
            }

            if (!isFocused)
                return;
            foreach (var rt in m_RenderTargets)
            {
                isEnd &= rt.ClientIsDisposed;
                rt.Draw();
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
