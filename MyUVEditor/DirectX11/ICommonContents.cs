using System.Collections.Generic;
using System.ComponentModel;
using SlimDX.Direct3D11;
using PEPlugin;
using PEPlugin.Pmx;
namespace MyUVEditor.DirectX11
{
    interface ICommonContents
    {
        void Load(Device device, BackgroundWorker worker);
        void Unload();
        void SetRunArgsAndPmx(IPERunArgs args);
        IPXPmx Pmx { get; }
        List<IDrawable> CommonDrawables { get; }
        Effect Effect { get; }
        ShaderResourceView GetTexture(string key);
    }
}
