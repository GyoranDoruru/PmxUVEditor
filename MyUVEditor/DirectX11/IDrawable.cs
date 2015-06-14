using SlimDX;
using SlimDX.Direct3D11;
using PEPlugin.Pmx;
namespace MyUVEditor.DirectX11
{
    interface IDrawable:System.IDisposable
    {
        void Draw();
    }
}
