using SlimDX;
using SlimDX.Direct3D11;
using PEPlugin.Pmx;
namespace MyUVEditor.DirectX11
{
    interface IDrawable:System.IDisposable
    {
        Matrix World { get; }
        void ResetForDraw();
        void Draw();
        bool Visible { get; }
    }
}
