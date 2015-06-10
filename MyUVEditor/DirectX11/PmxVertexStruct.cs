using System.Runtime.InteropServices;
using SlimDX;
using SlimDX.Direct3D11;
namespace MyUVEditor.DirectX11
{
    struct PmxVertexStruct
    {
        public Vector3 Position;
        public Vector2 Tex;

        public static readonly InputElement[] VertexElements = new[]{
            new InputElement{
                SemanticName = "SV_Position",
                Format = SlimDX.DXGI.Format.R32G32B32_Float},
            new InputElement{
                SemanticName = "TEXCOORD",
                Format = SlimDX.DXGI.Format.R32G32_Float,
                AlignedByteOffset = InputElement.AppendAligned},
        };

        public static int SizeInBytes
        {
            get
            {
                return Marshal.SizeOf(typeof(PmxVertexStruct));
            }
        }
    }
}