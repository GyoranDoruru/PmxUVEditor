﻿using System.Collections.Generic;
using SlimDX;
using SlimDX.Direct3D11;
using PEPlugin.Pmx;
namespace MyUVEditor.DirectX11
{
    using VIDictionary = Dictionary<IPXVertex, int>;
    class DrawableMaterial : DrawableTriangle
    {
        protected Buffer IndexBuffer { get; private set; }
        private int m_IndexOffset;
        protected bool m_IsCommonIndexBuffer;
        private IPXPmx m_Pmx;
        private IPXMaterial m_Material;
        private VIDictionary m_VertexIndexDic;
        public DrawableMaterial(IPXPmx pmx, IPXMaterial material)
        {
            World = Matrix.Identity;
            Visible = true;
            m_Pmx = pmx;
            m_Material = material;
            m_VertexIndexDic = new VIDictionary();
        }

        public DrawableMaterial(IPXPmx pmx, IPXMaterial material, VIDictionary vertexIndexDic)
        {
            World = Matrix.Identity;
            Visible = true;
            m_Pmx = pmx;
            m_Material = material;
            m_VertexIndexDic = vertexIndexDic;
        }

        override protected void initVertexBuffer(Device device)
        {
            int index = 0;
            var vertices = new PmxVertexStruct[m_Pmx.Vertex.Count];
            foreach (var v in m_Pmx.Vertex)
            {
                vertices[index] = new PmxVertexStruct(v);
                m_VertexIndexDic.Add(v, index);
                index++;
            }

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

        protected void initIndexBuffer(Device device)
        {
            int faceLength = 0;
            foreach (var m in m_Pmx.Material)
                faceLength += m.Faces.Count;
            int[] vertexIndices = new int[faceLength * 3];
            int index = 0;
            foreach (var m in m_Pmx.Material)
            {
                foreach (var f in m.Faces)
                {
                    vertexIndices[index] = m_VertexIndexDic[f.Vertex1];
                    vertexIndices[index + 1] = m_VertexIndexDic[f.Vertex2];
                    vertexIndices[index + 2] = m_VertexIndexDic[f.Vertex3];
                    index += 3;
                }
            }
            using (DataStream indexStream
                = new DataStream(vertexIndices, true, true))
            {
                IndexBuffer = new Buffer(
                    device,
                    indexStream,
                    new BufferDescription
                    {
                        SizeInBytes = (int)indexStream.Length,
                        BindFlags = BindFlags.IndexBuffer,
                    });
            }

        }

        override public void ResetForDraw()
        {
            Device.ImmediateContext.InputAssembler.InputLayout
                = VertexLayout;
            Device.ImmediateContext.InputAssembler.SetVertexBuffers(
                0, new VertexBufferBinding(VertexBuffer, VertexSizeInBytes, 0));
            Device.ImmediateContext.InputAssembler.SetIndexBuffer(
                IndexBuffer, SlimDX.DXGI.Format.R32_UInt, 0);
            Device.ImmediateContext.InputAssembler.PrimitiveTopology
                = PrimitiveTopology.TriangleList;
            EffectManager.SetWorld(World);

        }

        override public void Draw()
        {
            if (!Visible)
                return;
            EffectManager.SetMaterial(m_Material);
            EffectManager.SetTexture(Texture);
            EffectManager.SetTechAndPass(0, 0);
            Device.ImmediateContext.DrawIndexed(m_Material.Faces.Count * 3, m_IndexOffset, 0);

        }


        public void SetIndexBuffer(Buffer indexBuffer, bool isCommon, int indexOffset)
        {
            if (isCommon && indexBuffer != null)
            {
                IndexBuffer = indexBuffer;
            }
            else
            {
                initIndexBuffer(Device);
            }
            m_IndexOffset = indexOffset;
            m_IsCommonIndexBuffer = isCommon;
        }

        override public void Dispose()
        {
            if (m_IsCommonIndexBuffer)
                IndexBuffer = null;
            else
                IndexBuffer.Dispose();
            base.Dispose();

        }
    }
}
