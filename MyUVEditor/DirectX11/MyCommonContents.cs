using System;
using System.IO;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using SlimDX.D3DCompiler;
using PEPlugin;
namespace MyUVEditor.DirectX11
{
    class MyCommonContents:ICommonContents
    {

        InputLayout m_VertexLayout;
        SlimDX.Direct3D11.Buffer m_VertexBuffer;
        ShaderResourceView[] m_Textures;
        Effect m_effect;

        private void initEffect(SlimDX.Direct3D11.Device device)
        {
            using (ShaderBytecode shaderByteCode = ShaderBytecode.Compile(
                Properties.Resources.myeffect, "fx_5_0", ShaderFlags.None, EffectFlags.None))
            {
                m_effect = new Effect(device, shaderByteCode);
            }
        }

        private void initVertexLayout(SlimDX.Direct3D11.Device device)
        {
            var t = m_effect.GetTechniqueByIndex(0);
            var p = t.GetPassByIndex(0);
            var d = p.Description;
            var s = d.Signature;
            var ve = PmxVertexStruct.VertexElements;

            m_VertexLayout = new InputLayout(
                device,
                m_effect.GetTechniqueByIndex(0).GetPassByIndex(0).Description.Signature,
                PmxVertexStruct.VertexElements);
        }

        private void initVertexBuffer(SlimDX.Direct3D11.Device device)
        {
            var vertices = new[] {
                new PmxVertexStruct {
                    Position = new Vector3(0, 0.5f, 0),
                    Tex = new Vector2(0.5f, 0)
                },
                new PmxVertexStruct{
                    Position = new Vector3(0.5f, 0, 0),
                    Tex = new Vector2(1, 1)
                },
                new PmxVertexStruct{
                    Position = new Vector3(-0.5f, 0, 0),
                    Tex = new Vector2(0, 1)
                },
            };
            using (SlimDX.DataStream vertexStream
                = new SlimDX.DataStream(vertices, true, true))
            {
                m_VertexBuffer = new SlimDX.Direct3D11.Buffer(
                    device,
                    vertexStream,
                    new BufferDescription
                    {
                        SizeInBytes = (int)vertexStream.Length,
                        BindFlags = BindFlags.VertexBuffer,
                    });
            }
        }

        private Texture2D BitmapToTexture(
            SlimDX.Direct3D11.Device device,System.Drawing.Bitmap bitmap)
        {
            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Png);
                int len = (int)stream.Position;
                stream.Position = 0;
                return Texture2D.FromStream(device, stream, (int)stream.Length);
            }

        }

        private ShaderResourceView GetWhiteTex(SlimDX.Direct3D11.Device device)
        {
            using (var texture = BitmapToTexture(device,Properties.Resources.WhiteTex))
            {
                return new ShaderResourceView(device, texture);
            }

        }
        private void initTexture(SlimDX.Direct3D11.Device device)
        {
            m_Textures = new ShaderResourceView[] { GetWhiteTex(device) };
        }

        public void Load(SlimDX.Direct3D11.Device device)
        {
            initEffect(device);
            initVertexLayout(device);
            initVertexBuffer(device);
            initTexture(device);
        }

        public void Unload() {
            m_effect.Dispose();
            m_VertexLayout.Dispose();
            m_VertexBuffer.Dispose();
            foreach (var t in m_Textures)
            {
                t.Dispose();
            }
        }

        public MyCommonContents() { }


        private IPERunArgs Args { get; set; }
        public void Set(PEPlugin.IPERunArgs args)
        {
            Args = args;
        }
    }
}
