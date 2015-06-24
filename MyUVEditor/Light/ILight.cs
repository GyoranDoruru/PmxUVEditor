using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace MyUVEditor.Light
{
    interface ILight
    {
        Vector3 Diffuse { get; }
        Vector3 Ambient { get; }
        Vector3 Specular { get; }
        Vector3 LightDirection { get; }
        Matrix LightWorldViewProjection { get; }

    }
}
