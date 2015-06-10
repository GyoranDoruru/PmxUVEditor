using SlimDX.Direct3D11;
using PEPlugin;
namespace MyUVEditor.DirectX11
{
    interface ICommonContents
    {
        void Load(Device device);
        void Unload();
        void Set(IPERunArgs args);
    }
}
