using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using MyUVEditor.Camera;
using MyUVEditor.Light;

namespace MyUVEditor.DirectX11
{
    class RenderTargetContentsUV:RenderTargetContents
    {
        int m_MaterialIndex = 0;
		public RenderTargetContentsUV(SlimDX.Direct3D11.Device device, SwapChain swapChain, Control client)
			: base(device, swapChain, client)
        {
            UVEditorMainForm form = null;
            for(var c = client;c!=null;c = c.Parent)
            {
                form = c as UVEditorMainForm;
                if (form != null)
                    break;
            }
            if (form == null)
                return;

            form.SelectedMaterial.SelectedIndexChanged
                += (o, args) => {
                    m_MaterialIndex = ((ComboBox)o).SelectedIndex;
                    if (Selector == null)
                        return;
                    var m = this.DrawablePmx.Pmx.Material[m_MaterialIndex];
                    var indices = new HashSet<int>();
                    foreach (var f in m.Faces)
                    {
                        indices.Add(DrawablePmx.VertexIndexDic[f.Vertex1]);
                        indices.Add(DrawablePmx.VertexIndexDic[f.Vertex2]);
                        indices.Add(DrawablePmx.VertexIndexDic[f.Vertex3]);
                    }
                    Selector.setFilter(indices);
                };
        }

        protected override void InitCamera()
		{
			if (Camera != null)
				Camera.Dispose();
			Camera = new Camera2D(Client);
		}

        public override void LoadContent(ICommonContents commonContents)
        {
            base.LoadContent(commonContents);
            sprite = new DrawableSprite(commonContents.Pmx);
            sprite.SetDrawablePmx(commonContents, null);
            initSelector(commonContents.Pmx);
        }

        public override void UnloadContent()
        {
            sprite.Dispose();
            base.UnloadContent();
        }

        DrawableSprite sprite;

		protected override void DrawContents()
        {
            // sprite
            sprite.EffectManager.SetCamera(this.Camera);
            sprite.ResetForDraw();
            sprite.Draw(m_MaterialIndex, 2);

            // uv triangles
            DrawablePmx.ResetForDraw();
            DrawablePmx.EffectManager.SetCamera(this.Camera);
            DrawablePmx.Draw(m_MaterialIndex, 3);

            // points
            this.GraphicsDevice.ImmediateContext.InputAssembler.PrimitiveTopology
                = PrimitiveTopology.PointList;
            Vector4 green = new Vector4(0, 1, 0, 1);
            Vector4 red = new Vector4(1, 0, 0, 1);
            DrawablePmx.EffectManager.setPointColor(green);
            DrawablePmx.Draw(m_MaterialIndex, 4);

            if (Selector.IsChanged)
            {
                DrawablePmx.ResetSelectedIndicesBuffer(Selector.Selected);
                Selector.IsChanged = false;
            }
            if (Selector.Selected != null && Selector.Selected.Length > 0)
            {
                DrawablePmx.EffectManager.setPointColor(red);
                DrawablePmx.EffectManager.SetTechAndPass(4, 0);
                GraphicsDevice.ImmediateContext.ClearDepthStencilView(DepthStencil, DepthStencilClearFlags.Depth, 1, 0);
                DrawablePmx.DrawSelectedVertices(Selector.Selected.Length);
            }

            // selector
        }

        private void initSelector(PEPlugin.Pmx.IPXPmx pmx)
        {
            Selector = new Selector.RectSelector(pmx, Camera);
        }

        public Selector.ISelector Selector
        {
            get { return m_selector; }
            set { setSelector(value); }
        }
        private Selector.ISelector m_selector;
        private void setSelector(Selector.ISelector selector)
        {
            if (m_selector != null)
                m_selector.DisconnectControl(Client);
            m_selector = selector;
            m_selector.ConnectControl(Client);
            m_selector.Camera = Camera;
        }


    }
}
