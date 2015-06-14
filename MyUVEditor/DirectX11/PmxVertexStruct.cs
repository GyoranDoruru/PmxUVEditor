using System.Runtime.InteropServices;
using SlimDX;
using SlimDX.Direct3D11;
namespace MyUVEditor.DirectX11
{
    struct PmxVertexStruct
    {
        internal Vector3 Position;
        internal Vector2 Tex;

        internal static readonly InputElement[] VertexElements = new[]{
            new InputElement{
                SemanticName = "SV_Position",
                Format = SlimDX.DXGI.Format.R32G32B32_Float},
            new InputElement{
                SemanticName = "TEXCOORD",
                Format = SlimDX.DXGI.Format.R32G32_Float,
                AlignedByteOffset = InputElement.AppendAligned},
        };

        internal static int SizeInBytes
        {
            get
            {
                return Marshal.SizeOf(typeof(PmxVertexStruct));
            }
        }
    }
}