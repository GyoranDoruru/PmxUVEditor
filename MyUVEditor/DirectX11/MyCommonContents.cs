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
        public SlimDX.Direct3D11.Buffer IndexBuffer { get; private set; }

        private IPERunArgs Args { get; set; }
        private Dictionary<string, ShaderResourceView> m_Textures;
        private Dictionary<IPXVertex, int> m_VertexIndexDic;

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
            m_VertexIndexDic.Clear();

            int index = 0;
            var vertices = new PmxVertexStruct[Pmx.Vertex.Count];
            foreach (var v in Pmx.Vertex)
            {
                vertices[index] = new PmxVertexStruct(v);
                m_VertexIndexDic.Add(v, index);
                index++;
            }

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

        private void initIndexBuffer(SlimDX.Direct3D11.Device device)
        {
            int faceLength = 0;
            foreach (var m in Pmx.Material)
                faceLength += m.Faces.Count;
            int[] vertexIndices = new int[faceLength*3];
            int index = 0;
            foreach (var m in Pmx.Material)
            {
                foreach (var f in m.Faces)
                {
                    vertexIndices[index] = m_VertexIndexDic[f.Vertex1];
                    vertexIndices[index+1] = m_VertexIndexDic[f.Vertex2];
                    vertexIndices[index+2] = m_VertexIndexDic[f.Vertex3];
                    index += 3;
                }
            }
            using (SlimDX.DataStream indexStream
                = new SlimDX.DataStream(vertexIndices, true, true))
            {
                IndexBuffer = new SlimDX.Direct3D11.Buffer(
                    device,
                    indexStream,
                    new BufferDescription
                    {
                        SizeInBytes = (int)indexStream.Length,
                        BindFlags = BindFlags.IndexBuffer,
                    });
            }

        }

        private void initTexture(SlimDX.Direct3D11.Device device)
        {
            m_Textures.Add("", GetWhiteTex(device));
        }

        private void SetPmx()
        {
            Pmx = Args.Host.Connector.Pmx.GetCurrentState();
        }

        public void Load(SlimDX.Direct3D11.Device device)
        {
            initEffect(device);
            initVertexLayout(device, Effect);
            initVertexBuffer(device);
            initIndexBuffer(device);
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
            m_VertexIndexDic = new Dictionary<IPXVertex, int>();
        }


        public void SetRunArgsAndPmx(PEPlugin.IPERunArgs args)
        {
            Args = args;
            Pmx = args.Host.Connector.Pmx.GetCurrentState();
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
