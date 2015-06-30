using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PEPlugin.View;
using SlimDX;

namespace MyUVEditor.Light
{
    class ParallelLight:ILight
    {
        private Vector3 m_LightColor;
        public Vector3 Diffuse { get { return Vector3.Zero; } }
        public Vector3 Ambient { get { return m_LightColor; } }
        public Vector3 Specular { get { return m_LightColor; } }

        public Vector3 LightDirection { get; private set; }
        public Matrix LightWorldViewProjection { get; private set; }

        private void SetLightDirection(Vector3 lightDirection)
        {
            LightDirection = lightDirection;

            Vector3 upDir = Vector3.Cross(LightDirection,Vector3.UnitX);
            if(upDir.LengthSquared()==0)
                upDir = Vector3.UnitY;
            Matrix v = Matrix.LookAtLH(-LightDirection, Vector3.Zero, upDir);
            Matrix p = Matrix.OrthoLH(1024, 1024, 0.1f, 1000);
            LightWorldViewProjection = v * p;
        }

        private void SetLight(IPEViewSettingConnector viewSetting)
        {
            var dir = viewSetting.LightDirection;
            SetLightDirection(new Vector3(dir.X, dir.Y, dir.Z));
            var srcColor = viewSetting.LightColor;
            m_LightColor = new Vector3(srcColor.R, srcColor.G, srcColor.B);
        }
    }
}
