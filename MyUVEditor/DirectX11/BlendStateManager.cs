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
                AlphaToCoverageEnable = true,
                IndependentBlendEnable = true
            };
            blendStateDesc.RenderTargets[0] = new RenderTargetBlendDescription
            {
                BlendEnable = true,
                BlendOperationAlpha = BlendOperation.Add,
                BlendOperation = BlendOperation.Add,
                DestinationBlend = BlendOption.InverseSourceAlpha,
                DestinationBlendAlpha = BlendOption.InverseSourceAlpha,
                RenderTargetWriteMask = ColorWriteMaskFlags.All,
                SourceBlend = BlendOption.SourceAlpha,
                SourceBlendAlpha = BlendOption.SourceAlpha
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
