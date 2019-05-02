﻿using System;
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
                += (o, args) => { m_MaterialIndex = ((ComboBox)o).SelectedIndex; };
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
            DrawablePmx.Draw(m_MaterialIndex, 4);
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
