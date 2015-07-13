using SlimDX;
using SlimDX.Direct3D11;
using PEPlugin.Pmx;
namespace MyUVEditor.DirectX11
{
    interface IDrawable:System.IDisposable
    {
        Matrix World { get; }
        EffectManager11 EffectManager { get; }
        void ResetForDraw();
        void Draw();
        bool Visible { get; }
    }
}
