using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using System.Drawing;

namespace MyUVEditor
{
    class CameraUV:Camera
    {
        public CameraUV(DXView client):base(client,-1)
        {
            Position = new Vector3(0, 18, -52);
            Target = new Vector3(0, 10, 0);
            UpDir = new Vector3(0, 1, 0);
            World = Matrix.Identity;
            Client = client;
            ResetWVPMatrix();
        }

        protected override void CameraRotate(Point prev, Point tmp)
        {
            CameraMove(prev, tmp);
        }


    }
}
