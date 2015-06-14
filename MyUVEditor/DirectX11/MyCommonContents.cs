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
using PEPlugin.Pmx;
namespace MyUVEditor.DirectX11
{
    class MyCommonContents:ICommonContents
    {
        public IPXPmx Pmx { get; private set; }
        public Effect Effect { get; private set; }
        public InputLayout VertexLayout { get; private set; }
        public SlimDX.Direct3D11.Buffer VertexBuffer { get; private set; }

        private IPERunArgs Args { get; set; }
        private Dictionary<string, ShaderResourceView> m_Textures;

        private void initEffect(SlimDX.Direct3D11.Device device)
        {
            Effect = EffectManager11.InitEffect(device);
        }

        private void initVertexLayout(SlimDX.Direct3D11.Device device, Effect effect)
        {
            VertexLayout = new InputLayout(
                device,
                EffectManager11.Signature(effect),
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
                VertexBuffer = new SlimDX.Direct3D11.Buffer(
                    device,
                    vertexStream,
                    new BufferDescription
                    {
                        SizeInBytes = (int)vertexStream.Length,
                        BindFlags = BindFlags.VertexBuffer,
                    });
            }
        }

        private void initTexture(SlimDX.Direct3D11.Device device)
        {
            m_Textures.Add("", GetWhiteTex(device));
        }

        public void Load(SlimDX.Direct3D11.Device device)
        {
            initEffect(device);
            initVertexLayout(device, Effect);
            initVertexBuffer(device);
            initTexture(device);
        }

        public void Unload() {
            VertexLayout.Dispose();
            VertexBuffer.Dispose();
            foreach (var t in m_Textures)
            {
                t.Value.Dispose();
            }
            m_Textures.Clear();
            Effect.Dispose();
        }

        public MyCommonContents()
        {
            m_Textures = new Dictionary<string, ShaderResourceView>();
        }


        public void SetRunArgs(PEPlugin.IPERunArgs args)
        {
            Args = args;
        }

        static private Texture2D BitmapToTexture(
            SlimDX.Direct3D11.Device device, System.Drawing.Bitmap bitmap)
        {
            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Png);
                int len = (int)stream.Position;
                stream.Position = 0;
                return Texture2D.FromStream(device, stream, (int)stream.Length);
            }

        }

        static private ShaderResourceView GetWhiteTex(SlimDX.Direct3D11.Device device)
        {
            using (var texture = BitmapToTexture(device, Properties.Resources.WhiteTex))
            {
                return new ShaderResourceView(device, texture);
            }

        }



        public ShaderResourceView GetTexture(string key)
        {
            ShaderResourceView value = null;
            m_Textures.TryGetValue(key, out value);
            return value;
        }
    }
}
