using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SlimDX;
using SlimDX.Direct3D9;

namespace MyUVEditor
{
    public partial class DXViewUV : DXView
    {
        public DXViewUV()
        {
            InitializeComponent();
            this.Camera = new CameraUV(this);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }

        public override Result Render(PMXMesh pmx)
        {
            Device device = swapChain.Device;
            device.SetRenderTarget(0, swapChain.GetBackBuffer(0));
            device.SetRenderState(RenderState.ZEnable, true);
            device.DepthStencilSurface = depthSurface;
            device.Viewport = Viewport;
            //    device.SetRenderState(RenderState.Lighting, true);
            //    device.SetLight(0, pmx.MatManager.Light);
            //    device.EnableLight(0, true);
            device.SetTransform(TransformState.World, Camera.World);
            device.SetTransform(TransformState.View, Camera.View);
            device.SetTransform(TransformState.Projection, Camera.Projection);
            Sprite texSprite = pmx.TexSprite;
            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, pmx.MatManager.BGColor, 1.0f, 0);
            device.BeginScene();
            texSprite.Begin(SpriteFlags.ObjectSpace);
            //texSprite.Begin(SpriteFlags.None);
            Vector3 Pos = Vector3.Zero;
            for (int i = 0; i < pmx.Pmx.Material.Count; i++)
            //for (int i = 0; i < 1; i++)
            {
                ExtendedMaterial m = pmx.GetMaterials()[i];
                Texture tex = pmx.MatManager.DicObjTex[m.TextureFileName];
                int width = tex.GetLevelDescription(0).Width;
                int height = tex.GetLevelDescription(0).Height;
                Matrix scaling = Matrix.Scaling(new Vector3(1.0f /width, 1.0f / height, 1.0f));
                Matrix transpose = Matrix.Translation(Pos);
                texSprite.Transform = scaling * transpose;
                texSprite.Draw(tex,Color.White);
                if (i % 7 == 6)
                {
                    Pos.X = 0;
                    Pos.Y += 1;
                }
                else
                {
                    Pos.X += 1;
                }
            }
            texSprite.End();
            device.EndScene();
            return swapChain.Present(Present.None);
        }
    }
}
