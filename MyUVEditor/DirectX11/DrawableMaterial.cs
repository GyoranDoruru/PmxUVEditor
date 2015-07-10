using System.Collections.Generic;
using System.ComponentModel;
using SlimDX;
using SlimDX.Direct3D11;
using PEPlugin.Pmx;
namespace MyUVEditor.DirectX11
{
    using VIDictionary = Dictionary<IPXVertex, int>;
    using TKind = TextureUtility.TextureKind;
    class DrawableMaterial
    {
        public Matrix World { get; private set; }
        public bool Visible { get; set; }
        private int IndexOffset { get; set; }
        private IPXMaterial OriginMaterial { get; set; }
        private ShaderResourceView[] Textures { get; set; }

        public DrawableMaterial(IPXMaterial material, int indexOffset)
        {
            World = Matrix.Identity;
            Visible = true;
            OriginMaterial = material;
            IndexOffset = indexOffset;
            Textures = new ShaderResourceView[(int)TKind.NUM];
        }

        private void SetEffect(EffectManager11 effectManager) {
            effectManager.SetMaterial(OriginMaterial, Textures);
            switch (OriginMaterial.SphereMode)
            {
                case SphereType.None:
                case SphereType.Mul:
                case SphereType.SubTex:
                    effectManager.SetTechAndPass(0, 0);
                    break;
                case SphereType.Add:
                    effectManager.SetTechAndPass(1, 0);
                    break;
            }
        }

        public void Draw(EffectManager11 effectManager)
        {
            if (!Visible)
                return;
            SetEffect(effectManager);
            effectManager.Effect.Device.ImmediateContext.DrawIndexed(
                OriginMaterial.Faces.Count * 3, IndexOffset, 0);

        }

        public void SetTextures(MyCommonContents common)
        {
            Textures[(int)TKind.OBJ] = common.GetTexture(OriginMaterial.Tex);
            Textures[(int)TKind.SPHERE] = common.GetTexture(OriginMaterial.Sphere);
            Textures[(int)TKind.TOON] = common.GetTexture(OriginMaterial.Toon);
        }
    }
}
