using System.ComponentModel;
using SlimDX;
using SlimDX.Direct3D11;

namespace MyUVEditor.DirectX11
{
    class DrawableTriangle : IDrawable
    {
        protected EffectManager11 EffectManager { get; private set; }
        protected Matrix World { get; set; }
        protected InputLayout VertexLayout { get; set; }
        protected Buffer VertexBuffer { get; set; }
        protected int VertexSizeInBytes { get; private set; }
        protected ShaderResourceView Texture { get; private set; }
        virtual protected Device Device { get { return Texture.Device; } }
        protected Effect Effect { get { return EffectManager.Effect; } }
        virtual public bool Visible { get; set; }
        private bool m_IsCommonEffectManager;
        private bool m_IsCommonVertexLayout;
        private bool m_IsCommonVertexBuffer;
        private bool m_IsCommonTexture;

        public DrawableTriangle()
        {
            World = Matrix.Identity;
            Visible = true;
        }

        virtual protected void initVertexLayout(Device device, Effect effect)
        {
            VertexLayout = new InputLayout(
                device,
                EffectManager11.Signature(effect),
                PmxVertexStruct.VertexElements);
        }

        virtual protected void initVertexBuffer(Device device, BackgroundWorker worker)
        {
            worker.ReportProgress(0, "頂点読み込み中");
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
            using (DataStream vertexStream
                = new DataStream(vertices, true, true))
            {
                VertexBuffer = new Buffer(
                    device,
                    vertexStream,
                    new BufferDescription
                    {
                        SizeInBytes = (int)vertexStream.Length,
                        BindFlags = BindFlags.VertexBuffer,
                    });
            }
            worker.ReportProgress(100, "頂点読み込み中");
        }

        virtual public void ResetForDraw()
        {
            Device.ImmediateContext.InputAssembler.InputLayout
                = VertexLayout;
            Device.ImmediateContext.InputAssembler.SetVertexBuffers(
                0, new VertexBufferBinding(VertexBuffer, VertexSizeInBytes, 0));
        }

        virtual public void Draw()
        {
            if (!Visible)
                return;
            EffectManager.SetWorld(World);
            EffectManager.SetTexture(Texture);
            EffectManager.SetTechAndPass(0, 0);
            Device.ImmediateContext.Draw(3, 0);

        }

        public void SetEffectManager(EffectManager11 effectManager, bool isCommon)
        {
            EffectManager = effectManager;
            m_IsCommonEffectManager = isCommon;
        }
        public void setVertexLayout(InputLayout vertexLayout, bool isCommon, int vertexSizeInBytes)
        {
            if (isCommon && vertexLayout != null)
                VertexLayout = vertexLayout;
            else
                initVertexLayout(Device, Effect);

            m_IsCommonVertexLayout = isCommon;
            VertexSizeInBytes = vertexSizeInBytes;
        }
        public void SetVertexBuffer(Buffer vertexBuffer, bool isCommon, BackgroundWorker worker)
        {
            if (isCommon && vertexBuffer != null)
                VertexBuffer = vertexBuffer;
            else
                initVertexBuffer(Device,worker);

            m_IsCommonVertexBuffer = isCommon;
        }
        public void SetTexture(ShaderResourceView texture, bool isCommon)
        {
            Texture = texture;
            m_IsCommonTexture = isCommon;
        }

        virtual public void Dispose()
        {
            if (m_IsCommonTexture) Texture = null;
            else Texture.Dispose();

            if (m_IsCommonVertexBuffer) VertexBuffer = null;
            else VertexBuffer.Dispose();

            if (m_IsCommonVertexLayout) VertexLayout = null;
            else VertexLayout.Dispose();

            if (m_IsCommonEffectManager) EffectManager = null;
            else EffectManager.Dispose();

        }
    }
}
