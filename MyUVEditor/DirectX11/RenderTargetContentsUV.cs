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
        protected virtual void DrawContnts()
        {
            // sprite
            DrawableList[0].ResetForDraw();
            DrawableList[0].Draw();
            Matrix spWorld = DrawableList[0].World;

            // uv triangles
            var dpmx = (DrawablePmx)DrawableList[1];
            dpmx.ResetForDraw();
            dpmx.EffectManager.SetWorld(spWorld);
            dpmx.Draw(m_MaterialIndex, dpmx.EffectManager,2);

        }
    }
}
