using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.D3DCompiler;
using PEPlugin.Pmx;

namespace MyUVEditor.DirectX11
{
    class EffectManager11:IDisposable
    {
        public Effect Effect { get; private set; }

        public EffectManager11(Effect effect)
        {
            Effect = effect;
        }

        static public Effect InitEffect(Device device)
        {
            using (ShaderBytecode shaderByteCode = ShaderBytecode.Compile(
                Properties.Resources.myeffect, "fx_5_0", ShaderFlags.None, EffectFlags.None))
            {
                return new Effect(device, shaderByteCode);
            }
        }

        static public ShaderSignature Signature(Effect effect)
        {
            return effect.GetTechniqueByIndex(0).GetPassByIndex(0).Description.Signature;
        }

        public void SetTechAndPass(int techIndex, int passIndex)
        {
            Effect.GetTechniqueByIndex(techIndex).GetPassByIndex(passIndex).Apply(
                Effect.Device.ImmediateContext);
        }

        public void SetWorld(Matrix world)
        {
            Effect.GetVariableByName("World").AsMatrix().SetMatrix(world);
        }

        public void SetViewProjection(Matrix viewProjection)
        {
            Effect.GetVariableByName("ViewProjection").AsMatrix().SetMatrix(
                viewProjection);
        }

        public void SetTexture(ShaderResourceView texture)
        {
            Effect.GetVariableByName("diffuseTexture").AsResource().SetResource(texture);
        }

        public void SetMaterial(IPXMaterial material) { }
        public void Dispose()
        {
            Effect = null;
        }
    }
}
