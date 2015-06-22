using System.Collections.Generic;
using System.ComponentModel;
using SlimDX;
using SlimDX.Direct3D11;
using PEPlugin.Pmx;

namespace MyUVEditor.DirectX11
{
    using VIDictionary = Dictionary<IPXVertex, int>;
    class DrawablePmx : DrawableTriangle
    {
        protected Buffer IndexBuffer { get; private set; }
        protected bool m_IsCommonIndexBuffer;

        internal IPXPmx Pmx { get; private set; }
        private Device m_Device;
        override protected Device Device { get { return m_Device; } }
        private List<DrawableMaterial> m_Materials;
        internal VIDictionary VertexIndexDic { get; private set; }

        public DrawablePmx(IPXPmx pmx)
        {
            World = Matrix.Identity;
            Visible = true;
            Pmx = pmx;
            m_Materials = new List<DrawableMaterial>(pmx.Material.Count);
            VertexIndexDic = new VIDictionary();
        }

        override protected void initVertexBuffer(Device device, BackgroundWorker worker)
        {
            int index = 0;
            var vertices = new PmxVertexStruct[Pmx.Vertex.Count];
            worker.ReportProgress(0, "頂点読み込み中");
            foreach (var v in Pmx.Vertex)
            {
                vertices[index] = new PmxVertexStruct(v);
                VertexIndexDic.Add(v, index);
                index++;
                worker.ReportProgress(index * 100 / Pmx.Vertex.Count, "頂点読み込み中");
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

        protected void initIndexBuffer(Device device, BackgroundWorker worker)
        {
            int faceLength = 0;
            foreach (var m in Pmx.Material)
                faceLength += m.Faces.Count;
            int[] vertexIndices = new int[faceLength * 3];
            int index = 0;
            worker.ReportProgress(0, "面頂点読み込み中");
            foreach (var m in Pmx.Material)
            {
                foreach (var f in m.Faces)
                {
                    vertexIndices[index] = VertexIndexDic[f.Vertex1];
                    vertexIndices[index + 1] = VertexIndexDic[f.Vertex2];
                    vertexIndices[index + 2] = VertexIndexDic[f.Vertex3];
                    index += 3;
                    worker.ReportProgress(index*100/vertexIndices.Length, "面頂点読み込み中");

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

        public void SetDrawablePmx(MyCommonContents common, BackgroundWorker worker)
        {
            if (VertexLayout != null)
                VertexLayout.Dispose();
            if (VertexBuffer != null)
                VertexBuffer.Dispose();
            if (IndexBuffer != null)
                IndexBuffer.Dispose();
            if (EffectManager != null)
                EffectManager.Dispose();

            m_Device = common.Effect.Device;
            SetEffectManager(new EffectManager11(common.Effect), false);
            SetTexture(null, true);
            initVertexLayout(Device, Effect);
            initVertexBuffer(Device, worker);
            initIndexBuffer(Device, worker);
            foreach(var m in m_Materials)
                m.Dispose();
            m_Materials.Clear();
            int count = 0;
            foreach (var ipxm in Pmx.Material)
            {
                var tmpMaterial = new DrawableMaterial(Pmx, ipxm, VertexIndexDic);
                m_Materials.Add(tmpMaterial);
                tmpMaterial.SetEffectManager(EffectManager, true);
                tmpMaterial.SetTexture(common.GetTexture(ipxm.Tex), true);
                tmpMaterial.setVertexLayout(VertexLayout, true, PmxVertexStruct.SizeInBytes);
                tmpMaterial.SetVertexBuffer(VertexBuffer, true, worker);
                tmpMaterial.SetIndexBuffer(IndexBuffer, true, count,worker);
                count += ipxm.Faces.Count * 3;
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
        }

        public override void Draw()
        {
            if (!Visible)
                return;
            foreach (var m in m_Materials)
            {
                m.ResetForDraw();
                m.Draw();
            }
        }

        
        private void Draw(int i)
        {
            m_Materials[i].ResetForDraw();
            m_Materials[i].Draw();
        }

        public override void Dispose()
        {
            foreach (var m in m_Materials)
                m.Dispose();
            m_Materials.Clear();
            if (m_IsCommonIndexBuffer)
                IndexBuffer = null;
            else
                IndexBuffer.Dispose();

            base.Dispose();
        }
    }
}
