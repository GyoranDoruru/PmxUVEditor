using SlimDX.Direct3D11;
using PEPlugin;
using PEPlugin.Pmx;
namespace MyUVEditor.DirectX11
{
    interface ICommonContents
    {
        void Load(Device device);
        void Unload();
        void SetRunArgsAndPmx(IPERunArgs args);
        IPXPmx Pmx { get; }
        Effect Effect { get; }
        InputLayout VertexLayout { get; }
        Buffer VertexBuffer { get; }
        Buffer IndexBuffer { get; }
        ShaderResourceView GetTexture(string key);
    }
}
