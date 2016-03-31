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
        public bool Visible { get; set; }
        private int IndexOffset { get; set; }
        private IPXMaterial OriginMaterial { get; set; }
        private ShaderResourceView[] Textures { get; set; }

        public DrawableMaterial(IPXMaterial material, int indexOffset)
        {
            Visible = true;
            OriginMaterial = material;
            IndexOffset = indexOffset;
            Textures = new ShaderResourceView[(int)TKind.NUM];
        }

        public void SetEffect(EffectManager11 effectManager)
        {
            effectManager.SetMaterial(OriginMaterial, Textures);
            switch (OriginMaterial.SphereMode)
            {
                case SphereType.None:
                case SphereType.Mul:
                case SphereType.SubTex:
                    SetEffect(effectManager, 0);
                    break;
                case SphereType.Add:
                    SetEffect(effectManager, 1);
                    break;
            }
        }
        public void SetEffect(EffectManager11 effectManager, int technique)
        {
            effectManager.SetMaterial(OriginMaterial, Textures);
            effectManager.SetTechAndPass(technique, 0);

        }

        public void Draw(EffectManager11 effectManager)
        {
            if (!Visible)
                return;
            effectManager.Effect.Device.ImmediateContext.DrawIndexed(
                OriginMaterial.Faces.Count * 3, IndexOffset, 0);

        }

        public void SetTextures(MyCommonContents common)
        {
            Textures[(int)TKind.OBJ] = common.GetTexture(OriginMaterial.Tex);
            Textures[(int)TKind.TOON] = common.GetTexture(OriginMaterial.Toon);
            Textures[(int)TKind.SPHERE] = common.GetTexture(
                (OriginMaterial.SphereMode == SphereType.Add && OriginMaterial.Sphere == MyCommonContents.WhiteTexPath)
                ? MyCommonContents.BlackTexPath
                : OriginMaterial.Sphere);

        }
    }
}
