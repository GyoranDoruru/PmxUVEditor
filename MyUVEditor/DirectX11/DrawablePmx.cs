using System.Collections.Generic;
using System.ComponentModel;
using SlimDX;
using SlimDX.Direct3D11;
using PEPlugin.Pmx;

namespace MyUVEditor.DirectX11
{
    using VIDictionary = Dictionary<IPXVertex, int>;
    class DrawablePmx : IDrawable
    {
        public Matrix World { get; private set; }
        public bool Visible { get; set; }
        protected Device Device { get; set; }
        protected Buffer VertexBuffer { get; set; }
        protected Buffer IndexBuffer { get; set; }
        protected InputLayout VertexLayout { get; set; }

        private bool m_IsCommonEffectManager;
        public EffectManager11 EffectManager { get; private set; }

        public IPXPmx Pmx { get; protected set; }
        private List<DrawableMaterial> m_Materials;
        public VIDictionary VertexIndexDic { get; protected set; }

        public DrawablePmx(IPXPmx pmx)
        {
            World = Matrix.Identity;
            Visible = true;
            Pmx = pmx;
            m_Materials = new List<DrawableMaterial>(pmx.Material.Count);
            VertexIndexDic = new VIDictionary(pmx.Vertex.Count);
        }

        protected void initVertexBuffer(Device device, BackgroundWorker worker)
        {
            int index = 0;
            var vertices = new PmxVertexStruct[Pmx.Vertex.Count];
            if (worker!=null)
                worker.ReportProgress(0, "頂点読み込み中");
            foreach (var v in Pmx.Vertex)
            {
                vertices[index] = new PmxVertexStruct(v);
                VertexIndexDic.Add(v, index);
                index++;
                if (worker != null)
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
            if (worker != null)
                worker.ReportProgress(0, "面頂点読み込み中");
            foreach (var m in Pmx.Material)
            {
                foreach (var f in m.Faces)
                {
                    vertexIndices[index] = VertexIndexDic[f.Vertex1];
                    vertexIndices[index + 1] = VertexIndexDic[f.Vertex2];
                    vertexIndices[index + 2] = VertexIndexDic[f.Vertex3];
                    index += 3;
                    if (worker != null)
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

        protected void initVertexLayout(Device device, Effect effect)
        {
            VertexLayout = new InputLayout(
                device,
                EffectManager11.Signature(effect, 0, 0),
                PmxVertexStruct.VertexElements);
        }

        public void SetEffectManager(EffectManager11 effectManager, bool isCommon)
        {
            EffectManager = effectManager;
            m_IsCommonEffectManager = isCommon;
        }

        public void SetDrawablePmx(ICommonContents common, BackgroundWorker worker)
        {
            if (VertexLayout != null)
                VertexLayout.Dispose();
            if (VertexBuffer != null)
                VertexBuffer.Dispose();
            if (IndexBuffer != null)
                IndexBuffer.Dispose();
            if (EffectManager != null)
                EffectManager.Dispose();

            Device = common.Effect.Device;
            SetEffectManager(new EffectManager11(common.Effect), false);
            initVertexLayout(Device, common.Effect);
            initVertexBuffer(Device, worker);
            initIndexBuffer(Device, worker);
            m_Materials.Clear();
            int indexOffset = 0;
            foreach (var ipxMaterial in Pmx.Material)
            {
                var tmpMaterial = new DrawableMaterial(ipxMaterial, indexOffset);
                tmpMaterial.SetTextures(common);
                m_Materials.Add(tmpMaterial);
                indexOffset += ipxMaterial.Faces.Count * 3;
            }

        }

        public void ResetForDraw()
        {
            Device.ImmediateContext.InputAssembler.InputLayout
                = VertexLayout;
            Device.ImmediateContext.InputAssembler.SetVertexBuffers(
                0, new VertexBufferBinding(VertexBuffer, PmxVertexStruct.SizeInBytes, 0));
            Device.ImmediateContext.InputAssembler.SetIndexBuffer(
                IndexBuffer, SlimDX.DXGI.Format.R32_UInt, 0);
            Device.ImmediateContext.InputAssembler.PrimitiveTopology
                = PrimitiveTopology.TriangleList;
            EffectManager.SetWorld(World);
        }

        public void Draw()
        {
            if (!Visible)
                return;
            var vm = Connector.Args.Host.Connector.View.PmxViewHelper.PartsSelect.GetCheckedMaterialIndices();
			if (vm == null)
			{
				vm = new int[m_Materials.Count];
				for (int i = 0; i < m_Materials.Count; ++i)
				{
					vm[i] = i;
				}
			}
			foreach (int i in vm)
            {
                if (i >= m_Materials.Count) break;
                m_Materials[i].SetEffect(EffectManager);
                m_Materials[i].Draw(EffectManager);

            }
            //foreach (var m in m_Materials)
            //{
            //    m.SetEffect(EffectManager);
            //    m.Draw(EffectManager);
            //}
        }


        public void Draw(int i, int technique)
        {
            m_Materials[i].SetEffect(EffectManager, technique);
            m_Materials[i].Draw(EffectManager);
        }

        Buffer selectedIndices;
        public void ResetSelectedIndicesBuffer(int[] indices)
        {
            if (selectedIndices != null && !selectedIndices.Disposed)
                selectedIndices.Dispose();

            if(indices==null || indices.Length == 0)
            {
                selectedIndices = null;
                return;
            }

            using (DataStream indexStream
                = new DataStream(indices, true, true))
            {
                selectedIndices = new Buffer(
                    this.Device,
                    indexStream,
                    new BufferDescription
                    {
                        SizeInBytes = (int)indexStream.Length,
                        BindFlags = BindFlags.IndexBuffer,
                    });
            }

        }

        public void DrawSelectedVertices(int count)
        {
            if (selectedIndices == null)
                return;
            Device.ImmediateContext.InputAssembler.SetIndexBuffer(
                selectedIndices, SlimDX.DXGI.Format.R32_UInt, 0);
            Device.ImmediateContext.InputAssembler.PrimitiveTopology
                = PrimitiveTopology.PointList;
            EffectManager.Effect.Device.ImmediateContext.DrawIndexed
                (count, 0, 0);
        }


        public virtual void Dispose()
        {
            m_Materials.Clear();
            if(!VertexLayout.Disposed)
                VertexLayout.Dispose();
            if (!VertexBuffer.Disposed)
                VertexBuffer.Dispose();
            if(!IndexBuffer.Disposed)
                IndexBuffer.Dispose();
            if (m_IsCommonEffectManager)
                EffectManager = null;
            else
                EffectManager.Dispose();
            Device = null;
        }
    }
}
