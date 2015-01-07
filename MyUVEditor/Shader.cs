using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using SlimDX.Direct3D9;

namespace MyUVEditor
{
    class Shader
    {
        public void Compile(Device device)
        {
            //string path = @"F:\Visual Studio 2010\Projects\MyUVEditor\MyUVEditor\test.txt";
            string path = "test.fx";
            ShaderBytecode vsb = ShaderBytecode.CompileFromFile(path, "VertexShader_Main", device.VertexShaderProfile,ShaderFlags.None);
            ShaderBytecode psb = ShaderBytecode.CompileFromFile(path, "PixelShader_Main", device.PixelShaderProfile, ShaderFlags.None);
            device.VertexShader = new VertexShader(device, vsb);
            device.PixelShader = new PixelShader(device, psb);
        }
    }
}
