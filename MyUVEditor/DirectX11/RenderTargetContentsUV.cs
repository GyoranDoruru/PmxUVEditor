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
		Matrix spWorld = Matrix.Identity;
		public RenderTargetContentsUV(SlimDX.Direct3D11.Device device, SwapChain swapChain, Control client)
			: base(device, swapChain, client) { }

		protected override void InitCamera()
		{
			if (Camera != null)
				Camera.Dispose();
			Camera = new Camera2D(Client);
		}

		protected override void DrawContents()
        {
			// sprite
			//DrawableList[0].ResetForDraw();
			//DrawableList[0].Draw();
			//Matrix spWorld = DrawableList[0].World;

			// uv triangles
			var dpmx = (DrawablePmx)DrawableList[0];
			dpmx.ResetForDraw();
			dpmx.EffectManager.SetWorld(spWorld);
			dpmx.Draw(m_MaterialIndex, dpmx.EffectManager, 2);

		}
    }
}
