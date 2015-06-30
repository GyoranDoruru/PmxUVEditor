using System.Runtime.InteropServices;
using SlimDX;
using SlimDX.Direct3D11;
using PEPlugin.Pmx;
namespace MyUVEditor.DirectX11
{
    struct PmxVertexStruct
    {
        internal Vector3 Position;
        internal Vector3 Normal;
        internal Vector2 Tex;

        internal static readonly InputElement[] VertexElements = new[]{
            new InputElement{
                SemanticName = "SV_Position",
                Format = SlimDX.DXGI.Format.R32G32B32_Float},
            new InputElement{
                SemanticName = "NORMAL",
                Format = SlimDX.DXGI.Format.R32G32B32_Float,
                AlignedByteOffset = InputElement.AppendAligned},
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

        internal PmxVertexStruct(IPXVertex vertex)
        {
            Position = vertex.Position.ToVector3();
            Normal = vertex.Normal.ToVector3();
            Tex = vertex.UV.ToVector2();
        }
    }
}