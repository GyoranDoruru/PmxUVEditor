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
    public class DeviceManager
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
        public DeviceManager(DXViewForm[] forms)
        {
            CreateDevice(forms[0]);
            forms[0].ViewPort.InitResource(Device.GetSwapChain(0));
            forms[0].Show();
            for (int i = 1; i < forms.Length; i++)
            {
                forms[i].ViewPort.InitResource(Device, null);
                forms[i].Show();
            }
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

        public void Render(DXViewForm[] forms,PMXMesh mesh)
        {
            Result hr;
            for (int i = 0; i < forms.Length; i++)
            {
                hr = forms[i].ViewPort.Render(mesh);
                LoopForResetDevice(forms, hr);
            }
        }
        public void Dispose(DXViewForm[] forms)
        {
            PresentParameters[] pps;
            DisposeResource(forms,out pps);
            if (Device != null && !Device.Disposed)
            {
                Device.Dispose();
            }
        }

        public void LoopForResetDevice(DXViewForm[] forms, Result hr)
        {
            if (hr.IsSuccess)
                return;
            if (hr != ResultCode.DeviceLost)
                throw new SlimDXException(hr);

            PresentParameters[] pps;
            DisposeResource(forms, out pps);
            while (hr == ResultCode.DeviceLost)
            {
                Thread.Sleep(100);
                hr = Device.Reset(pps[0]);
            }

            if (hr.IsFailure)
            {
                throw new SlimDXException(hr);
            }
            else
            {
                forms[0].ViewPort.InitResource(Device.GetSwapChain(0));
                for (int i = 0; i < forms.Length; i++)
                    forms[i].ViewPort.InitResource(Device, pps[i]);
            }
        }

        public void DisposeResource(DXViewForm[] forms, out PresentParameters[] pps)
        {
            Device device;
            pps = new PresentParameters[forms.Length];
            for (int i = 0; i < forms.Length;i++ )
            {
                forms[i].ViewPort.DisposeResource(out device, out pps[i]);
            }
        }

    }
}
