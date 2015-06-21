using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using SlimDX.Direct3D11;
using PEPlugin.Pmx;
namespace MyUVEditor.DirectX11
{
    class DrawableMaterial : DrawableTriangle
    {
        protected Buffer IndexBuffer { get; private set; }
        private bool m_IsCommonIndexBuffer;
        private IPXMaterial m_Material;
        private int m_IndexOffset;

        public DrawableMaterial()
        {
            World = Matrix.Identity;
            Visible = true;
        }

        public void ResetForDraw()
        {
            Device.ImmediateContext.InputAssembler.InputLayout
                = VertexLayout;
            Device.ImmediateContext.InputAssembler.SetVertexBuffers(
                0, new VertexBufferBinding(VertexBuffer, VertexSizeInBytes, 0));
            Device.ImmediateContext.InputAssembler.SetIndexBuffer(
                IndexBuffer, SlimDX.DXGI.Format.R32_UInt, 0);
            Device.ImmediateContext.InputAssembler.PrimitiveTopology
                = PrimitiveTopology.TriangleList;

        }

        override public void Draw()
        {
            if (!Visible)
                return;
            EffectManager.SetWorld(World);
            EffectManager.SetMaterial(m_Material);
            EffectManager.SetTexture(Texture);
            EffectManager.SetTechAndPass(0, 0);
            Device.ImmediateContext.DrawIndexed(m_Material.Faces.Count * 3, m_IndexOffset, 0);

        }
        public void SetMaterial(IPXMaterial material)
        {
            m_Material = material;
        }

        public void SetIndexBuffer(Buffer indexBuffer, bool isCommon, int indexOffset)
        {
            IndexBuffer = indexBuffer;
            m_IsCommonIndexBuffer = isCommon;
            m_IndexOffset = indexOffset;
        }

        override public void Dispose()
        {
            if (m_IsCommonIndexBuffer) IndexBuffer = null;
            else IndexBuffer.Dispose();
            base.Dispose();

        }
    }
}
