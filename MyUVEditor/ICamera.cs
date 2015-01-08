using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SlimDX;
using SlimDX.Direct3D9;
using System.Drawing;

namespace MyUVEditor
{
    interface ICamera:IDisposable
    {
        Control Client { get; }
        Matrix WorldViewProjection { get; }
        Vector3 ScreenToWorld(Point p, float tgZ);
        void ResetCamera();
    }
}
