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

        internal static SlimDX.Direct3D9.VertexElement[] D9VertexElements()
        {
            var elements = new SlimDX.Direct3D9.VertexElement[3];
            short offset = 0;
            elements[0] = new SlimDX.Direct3D9.VertexElement{
                Stream = 0,
                Offset = offset,
                Type = SlimDX.Direct3D9.DeclarationType.Float3,
                Method = SlimDX.Direct3D9.DeclarationMethod.Default,
                Usage = SlimDX.Direct3D9.DeclarationUsage.Position,
                UsageIndex = 0
            };
            offset += (short)Marshal.SizeOf(typeof(Vector3));
            elements[1] = new SlimDX.Direct3D9.VertexElement{
                Offset = offset,
                Type = SlimDX.Direct3D9.DeclarationType.Float3,
                Method = SlimDX.Direct3D9.DeclarationMethod.Default,
                Usage = SlimDX.Direct3D9.DeclarationUsage.Normal,
                UsageIndex = 0
            };
            offset += (short)Marshal.SizeOf(typeof(Vector3));
            elements[2] = new SlimDX.Direct3D9.VertexElement
            {
                Offset = offset,
                Type = SlimDX.Direct3D9.DeclarationType.Float2,
                Method = SlimDX.Direct3D9.DeclarationMethod.Default,
                Usage = SlimDX.Direct3D9.DeclarationUsage.TextureCoordinate,
                UsageIndex = 0
            };
            return elements;
        }

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