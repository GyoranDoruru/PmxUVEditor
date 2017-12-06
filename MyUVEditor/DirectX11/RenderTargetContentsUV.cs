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
			: base(device, swapChain, client) { }

		protected override void InitCamera()
		{
			if (Camera != null)
				Camera.Dispose();
			Camera = new Camera2D(Client);
		}

        public override void LoadContent(ICommonContents commonContents)
        {
            base.LoadContent(commonContents);
            //sprite = new DrawableSprite();
            //sprite.SetDrawableSprite(commonContents); //error
        }

        public override void UnloadContent()
        {
            //sprite.Dispose();
            base.UnloadContent();
        }

        DrawableSprite sprite;

		protected override void DrawContents()
        {
			// sprite
			//DrawableList[0].ResetForDraw();
			//DrawableList[0].Draw();
			//Matrix spWorld = DrawableList[0].World;

			// uv triangles
			var dpmx = (DrawablePmx)DrawableList[0];
			dpmx.ResetForDraw();
			dpmx.EffectManager.SetCamera(this.Camera);
			dpmx.Draw(m_MaterialIndex, dpmx.EffectManager, 3);

		}
    }
}
