using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.Direct3D11;

namespace MyUVEditor.DirectX11
{
    class BlendStateManager:IDisposable
    {
        public BlendStateManager(Device device)
        {
            var blendStateDesc = new BlendStateDescription
            {
                AlphaToCoverageEnable = false,
                IndependentBlendEnable = false
            };
            blendStateDesc.RenderTargets[0] = new RenderTargetBlendDescription
            {
                BlendEnable = true,
                BlendOperation = BlendOperation.Add,
                BlendOperationAlpha = BlendOperation.Add,
                DestinationBlend = BlendOption.InverseSourceAlpha,
                DestinationBlendAlpha = BlendOption.Zero,
                RenderTargetWriteMask = ColorWriteMaskFlags.All,
                SourceBlend = BlendOption.SourceAlpha,
                SourceBlendAlpha = BlendOption.One
            };
            BlendState = BlendState.FromDescription(device, blendStateDesc);
        }
        public BlendState BlendState { get; private set; }




        public void Dispose()
        {
            BlendState.Dispose();
        }
    }
}
