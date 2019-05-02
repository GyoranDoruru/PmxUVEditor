﻿using System;
using System.ComponentModel;
using System.Windows.Forms;
using PEPlugin;
using SlimDX.Direct3D11;
using SlimDX.DXGI;


namespace MyUVEditor.DirectX11
{
    class DeviceManager11 : IDisposable
    {
        private SlimDX.Direct3D11.Device m_Device;
        private RenderTargetContents[] m_RenderTargets;
        public ICommonContents CommonContents { get; private set; }

        public DeviceManager11()
        {
            CheckSamplingQuality();
        }

        private void CreateDeviceAndSwapChain(Control control, out SwapChain swapChain)
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
			//m_RenderTargets[0] = new RenderTargetContents(m_Device, tmp_Swap, controls[0]);
			m_RenderTargets[0] = new RenderTargetContentsUV(m_Device, tmp_Swap, controls[0]);
            Factory factory = m_Device.Factory;
            for (int i = 1; i < controls.Length; i++)
            {
                tmp_Swap
                    = new SwapChain(factory, m_Device, DefaultSwapChainDescription(controls[i]));
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
                    Count = SampleCount,
                    Quality = SampleQuality
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
        static public int SampleCount { get; private set; }
        static public int SampleQuality { get; private set; }
        static public void CheckSamplingQuality()
        {
            var device = new SlimDX.Direct3D11.Device(DriverType.Hardware, DeviceCreationFlags.None);
            for(SampleCount = 4; SampleCount >= 0; --SampleCount)
            {
                SampleQuality = device.CheckMultisampleQualityLevels(
                    SlimDX.DXGI.Format.R8G8B8A8_UNorm, SampleCount) - 1;
                if (SampleQuality >= 0)
                    break;
      
            }
            if (SampleQuality > 16)
                SampleQuality = 16;
            device.Dispose();

        }



        private void LoadCommonContent(BackgroundWorker worker) { CommonContents.Load(m_Device, worker); }
        private void UnloadCommonContent() { CommonContents.Unload(); }

        public void Run(Control[] clients, BackgroundWorker worker)
        {
            CreateDeviceAndMultiSwapChain(clients);
            LoadCommonContent(worker);
            foreach (var rt in m_RenderTargets)
            {
                rt.LoadContent(CommonContents);
            }
            m_parentWorker = worker;
            SlimDX.Windows.MessagePump.Run(RenderTargetContents.GetParentForm(clients[0]), Draw);
            m_parentWorker = null;
            Dispose();
        }

        public void SetCommonContents(ICommonContents commonContents, IPERunArgs args)
        {
            CommonContents = commonContents;
            CommonContents.SetRunArgsAndPmx(args);
        }
        private BackgroundWorker m_parentWorker;
        private int m_PrevTime = 0;
        private int m_FrameCount = 0;
        private const int m_Interval = 3000;
        private bool RedrawRequested { get; set; }
        public void RedrawEventHandler(object sendor, EventArgs e)
        {
            this.RedrawRequested = true;
        }

        private void Draw()
        {
            bool isEnd = true;
            bool isFocused = false;
            foreach (var rt in m_RenderTargets)
            {
                isFocused = isFocused || rt.IsFocused();
            }

            if (!isFocused&&!RedrawRequested)
                return;
            RedrawRequested = false;
            foreach (var rt in m_RenderTargets)
            {
                isEnd &= rt.ClientIsDisposed;
                rt.Draw();
            }
            m_FrameCount++;
            int tmpTime = Environment.TickCount;
            if (tmpTime - m_PrevTime > m_Interval)
            {
                int fps = m_FrameCount * 1000 / (tmpTime - m_PrevTime);
                m_parentWorker.ReportProgress(fps * 100 / 60, fps.ToString() + "fps");
                m_FrameCount = 0;
                m_PrevTime = tmpTime;
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
