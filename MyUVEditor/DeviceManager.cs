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
        static MultisampleType maxSample;
        static public MultisampleType tmpSample { get; private set; }
        static int maxQuality;
        static public int tmpQuality { get; private set; }
        static public PresentParameters GetPresentParameters(Control c)
        {
            return new PresentParameters
            {
                BackBufferFormat = Format.X8R8G8B8,
                BackBufferCount = 1,
                BackBufferWidth = c.ClientSize.Width,
                BackBufferHeight = c.ClientSize.Height,
                Multisample = tmpSample,
                MultisampleQuality = tmpQuality-1,
                SwapEffect = SwapEffect.Discard,
                EnableAutoDepthStencil = true,
                AutoDepthStencilFormat = Format.D16,
                //PresentFlags = PresentFlags.DiscardDepthStencil,
                PresentFlags = PresentFlags.None,
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
            for (int i = 0; i < forms.Length; i++)
            {
                forms[i].ViewPort.InitResource(Device);
                forms[i].Show();
            }
        }

        private bool CheckDeviceType(Direct3D d3d, out DeviceType dType, out MultisampleType mType, out int mQuality)
        {
            dType = DeviceType.Hardware;
            mType = MultisampleType.EightSamples;
            mQuality = 0;
            //if (!d3d.CheckDeviceType(0, dType, Format.X8R8G8B8, Format.D16, true))
            //{
            //    dType = DeviceType.Reference;
            //    if (!d3d.CheckDeviceType(0, dType, Format.X8R8G8B8, Format.D16, true))
            //        return false;
            //}

            if (!d3d.CheckDeviceMultisampleType(0, dType, Format.X8R8G8B8, true, mType, out mQuality)
                || !d3d.CheckDeviceMultisampleType(0, dType, Format.D16, true, mType, out mQuality)
                || mQuality<1)
            {
                mType = MultisampleType.FourSamples;
                if (!d3d.CheckDeviceMultisampleType(0, dType, Format.X8R8G8B8, true, mType, out mQuality)
                    || !d3d.CheckDeviceMultisampleType(0, dType, Format.D16, true, mType, out mQuality)
                    || mQuality<1)
                {
                    mType = MultisampleType.TwoSamples;
                    if (!d3d.CheckDeviceMultisampleType(0, dType, Format.X8R8G8B8, true, mType, out mQuality)
                        || !d3d.CheckDeviceMultisampleType(0, dType, Format.D16, true, mType, out mQuality)
                        ||mQuality<1)
                        mType = MultisampleType.None;
                }
            }
            return true;
        }
        private void CreateDevice(DXViewForm form)
        {
            Direct3D direct3D = new Direct3D();
            DeviceType dType;
            CheckDeviceType(direct3D, out dType, out maxSample, out maxQuality);
            tmpSample = maxSample;
            tmpQuality = maxQuality;
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

        public void Render(DXViewForm[] forms, PMXMesh mesh)
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
            DisposeResource(forms);
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

            DisposeResource(forms);
            PresentParameters pp = DeviceManager.GetPresentParameters(forms[0]);
            while (hr == ResultCode.DeviceLost)
            {
                Thread.Sleep(100);
                hr = Device.Reset(pp);
            }

            if (hr.IsFailure)
            {
                throw new SlimDXException(hr);
            }
            else
            {
                for (int i = 0; i < forms.Length; i++)
                    forms[i].ViewPort.InitResource(Device);
            }
        }

        public void DisposeResource(DXViewForm[] forms)
        {
            Device device;
            for (int i = 0; i < forms.Length; i++)
            {
                forms[i].ViewPort.DisposeResource(out device);
            }
        }

    }
}
