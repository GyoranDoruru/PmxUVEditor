using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using SlimDX.Direct3D9;
using System.Windows.Forms;
using System.Threading;

namespace MyUVEditor
{
    public class DeviceManager : IDisposable
    {
        static public PresentParameters GetPresentParameters(Control c)
        {
            return new PresentParameters
            {
                BackBufferFormat = Format.X8R8G8B8,
                BackBufferCount = 1,
                BackBufferWidth = c.ClientSize.Width,
                BackBufferHeight = c.ClientSize.Height,
                Multisample = MultisampleType.None,
                SwapEffect = SwapEffect.Discard,
                EnableAutoDepthStencil = true,
                AutoDepthStencilFormat = Format.D16,
                PresentFlags = PresentFlags.DiscardDepthStencil,
                PresentationInterval = PresentInterval.Default,
                Windowed = true,
                DeviceWindowHandle = c.Handle
            };

        }

        public Device Device { get; private set; }
        public string DriveState { get; private set; }
        public DXViewForm mainForm { get; private set; }
        public DXViewForm subForm { get; private set; }
        private MaterialManager mm = new MaterialManager();
        public DeviceManager()
        {
            mainForm = new DXViewForm();
            subForm = new DXViewForm();
            CreateDevice(mainForm);
            mainForm.ViewPort.InitResource(Device.GetSwapChain(0));
            subForm.ViewPort.InitResource(Device, null);
            mainForm.Show();
            subForm.Show();
        }
        private void CreateDevice(DXViewForm form)
        {
            Direct3D direct3D = new Direct3D();
            PresentParameters pp = GetPresentParameters(form.ViewPort);
            direct3D = new Direct3D();
            try
            {
                Device = new Device(direct3D, 0, DeviceType.Hardware, form.Handle, CreateFlags.HardwareVertexProcessing, pp);
                DriveState = "松";
            }
            catch (SlimDXException ex1)
            {
                try
                {
                    Device = new Device(direct3D, 0, DeviceType.Hardware, form.Handle, CreateFlags.SoftwareVertexProcessing, pp);
                    DriveState = "竹\r\n" + ex1;
                }
                catch (SlimDXException ex2)
                {
                    try
                    {
                        Device = new Device(direct3D, 0, DeviceType.Reference, form.Handle, CreateFlags.SoftwareVertexProcessing, pp);
                        DriveState = "梅\r\n" + ex2;
                    }
                    catch (SlimDXException ex3)
                    {
                        MessageBox.Show(ex3 + "\r\nデバイスの作成に失敗しました。", "DirectX Initialization failed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }
        }

        public void Render()
        {
            Result hr;
            hr = mainForm.ViewPort.Render(mm);
            LoopForResetDevice(hr);
            hr = subForm.ViewPort.Render(mm);
            LoopForResetDevice(hr);
        }
        public void Dispose()
        {
            PresentParameters pp1, pp2;
            DisposeResource(out pp1, out pp2);
            if (Device != null && !Device.Disposed)
            {
                Device.Dispose();
            }
        }

        public void LoopForResetDevice(Result hr)
        {
            if (hr.IsSuccess)
                return;
            if (hr != ResultCode.DeviceLost)
                throw new SlimDXException(hr);

            PresentParameters ppMain;
            PresentParameters ppSub;
            DisposeResource(out ppMain, out ppSub);
            while (hr == ResultCode.DeviceLost)
            {
                Thread.Sleep(100);
                hr = Device.Reset(ppMain);
            }

            if (hr.IsFailure)
            {
                throw new SlimDXException(hr);
            }
            else
            {
                mainForm.ViewPort.InitResource(Device.GetSwapChain(0));
                subForm.ViewPort.InitResource(Device, ppSub);
            }
        }

        public void DisposeResource(out PresentParameters mainPP, out PresentParameters subPP)
        {
            Device device;
            mainForm.ViewPort.DisposeResource(out device, out mainPP);
            subForm.ViewPort.DisposeResource(out device, out subPP);
        }
    }
}
