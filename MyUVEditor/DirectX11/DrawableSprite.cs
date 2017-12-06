using System.Collections.Generic;
using SlimDX;
using SlimDX.Direct3D11;

namespace MyUVEditor.DirectX11
{
    class DrawableSprite:IDrawable
    {
        private float m_ratio;  // width/height
        public Matrix World
        {
            get { return Matrix.Scaling(m_ratio, -1, 1); }
        }
        public bool Visible { get; set; }
        protected Device Device { get; private set; }
        protected Buffer VertexBuffer { get; private set; }
        protected InputLayout VertexLayout { get; set; }
        private bool m_IsCommonEffectManager;
        public EffectManager11 EffectManager { get; private set; }
        public ShaderResourceView Texture { get; set; }

        public DrawableSprite()
        {
        }

        private void initVertexBuffer(Device device)
        {
            if (VertexBuffer != null && !VertexBuffer.Disposed)
                VertexBuffer.Dispose();

            var vertices = new Vector2[]{
                Vector2.Zero,
                Vector2.UnitX,
                new Vector2(1,1),
                Vector2.UnitY
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
        }
        protected void initVertexLayout(Device device, Effect effect)
        {
            if (VertexLayout != null && !VertexLayout.Disposed)
                VertexLayout.Dispose();

            var elements = new InputElement[]{
                new InputElement{
                SemanticName = "SV_Position",
                Format = SlimDX.DXGI.Format.R32G32_Float},
            };

            VertexLayout = new InputLayout(
                device,
                EffectManager11.Signature(effect),
                elements);
        }
        public void SetEffectManager(EffectManager11 effectManager, bool isCommon)
        {
            EffectManager = effectManager;
            m_IsCommonEffectManager = isCommon;
        }
        public void SetDrawableSprite(ICommonContents common)
        {
            Device = common.Effect.Device;
            SetEffectManager(new EffectManager11(common.Effect), false);
            initVertexLayout(Device, common.Effect);
            initVertexBuffer(Device);
        }

        public void ResetForDraw()
        {
            Device.ImmediateContext.InputAssembler.InputLayout
                = VertexLayout;
            Device.ImmediateContext.InputAssembler.SetVertexBuffers(
                0, new VertexBufferBinding(VertexBuffer, PmxVertexStruct.SizeInBytes, 0));
            Device.ImmediateContext.InputAssembler.PrimitiveTopology
                 = PrimitiveTopology.TriangleStrip;
            EffectManager.SetWorld(World);
        }

        private void SetEffect()
        {
            EffectManager.SetObjectTexture(Texture);
            EffectManager.SetTechAndPass(2, 0);
        }


        public void Draw()
        {
            if (!Visible)
                return;
            Device.ImmediateContext.Draw(4, 0);
        }


        public void Dispose()
        {
            if (!VertexLayout.Disposed)
                VertexLayout.Dispose();
            if (!VertexBuffer.Disposed)
                VertexBuffer.Dispose();
            if (m_IsCommonEffectManager)
                EffectManager = null;
            else
                EffectManager.Dispose();

            Device = null;
            Texture = null;
        }

    }
}
