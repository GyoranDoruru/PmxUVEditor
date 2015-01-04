using System.Runtime.InteropServices;
using SlimDX;
using SlimDX.Direct3D9;
using PEPlugin.Pmx;

namespace MyUVEditor
{
    public class CustomVertex
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct PositionOnly
        {
            public static readonly VertexFormat FVF = VertexFormat.Position;
            public static readonly int Stride = Vector3.SizeInBytes;

            public Vector3 Position;

            public PositionOnly(Vector3 pos)
            {
                Position = pos;
            }
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct TransformedColored
        {
            public Vector4 Position;
            public int Color;
            public static readonly VertexFormat format = VertexFormat.Diffuse | VertexFormat.PositionRhw;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PositionColored
        {
            public static readonly VertexFormat FVF = VertexFormat.Position | VertexFormat.Diffuse;
            public static readonly int Stride = Vector3.SizeInBytes + sizeof(int);

            public Vector3 Position;
            public int Color;

            public PositionColored(Vector3 pos, int col)
            {
                Position = pos;
                Color = col;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PositionTextured
        {
            public static readonly VertexFormat FVF = VertexFormat.Position | VertexFormat.Texture1;
            //public static readonly int Stride = Vector3.SizeInBytes + 2 * sizeof(float);
            public static readonly int Stride = Marshal.SizeOf(typeof(PositionTextured));
            public Vector3 Position;
            public float u, v;

            public PositionTextured(Vector3 pos, float u, float v)
            {
                Position = pos;
                this.u = u;
                this.v = v;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PositionNormal
        {
            public static readonly VertexFormat FVF = VertexFormat.PositionNormal;
            public static readonly int Stride = 2 * Vector3.SizeInBytes;

            public Vector3 Position;
            public Vector3 Normal;

            public PositionNormal(Vector3 pos, Vector3 normal)
            {
                Position = pos;
                Normal = normal;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PositionNormalTextured
        {
            public PositionNormalTextured(Vector3 position, Vector3 normal, float Tu, float Tv)
            {
                this.Position = position;
                this.Normal = normal;
                this.UV = new Vector2(Tu, Tv);
            }

            public PositionNormalTextured(float[] floates)
            {
                this.Position = new Vector3(floates[0], floates[1], floates[2]);
                this.Normal = new Vector3(floates[3], floates[4], floates[5]);
                this.UV = new Vector2(floates[6], floates[7]);
            }

            public Vector3 Position;
            public Vector3 Normal;
            public Vector2 UV;
            public static readonly VertexFormat Format = VertexFormat.Position | VertexFormat.Normal | VertexFormat.Texture1;

            internal static int SizeInBytes()
            {
                return Marshal.SizeOf(typeof(PositionNormalTextured));
            }
            public float Tu
            {
                get { return this.UV.X; }
                set { this.UV.X = value; }
            }
            public float Tv
            {
                get { return this.UV.Y; }
                set { this.UV.Y = value; }
            }
            public float X
            {
                get { return this.Position.X; }
                set { this.Position.X = value; }
            }
            public float Y
            {
                get { return this.Position.Y; }
                set { this.Position.Y = value; }
            }
            public float Z
            {
                get { return this.Position.Z; }
                set { this.Position.Z = value; }
            }

            public float Nx
            {
                get { return this.Normal.X; }
                set { this.Normal.X = value; }
            }
            public float Ny
            {
                get { return this.Normal.Y; }
                set { this.Normal.Y = value; }
            }
            public float Nz
            {
                get { return this.Normal.Z; }
                set { this.Normal.Z = value; }
            }

        }
    }

    struct PMXVertex
    {
        Vector3 Pos;
        Vector3 Norm;
        Vector2 UV;
        Vector4 UVA1;
        Vector4 UVA2;
        Vector4 UVA3;
        Vector4 UVA4;
        public PMXVertex(IPXVertex v)
        {
            Pos = v.Position.ToVector3();
            Norm = v.Normal.ToVector3();
            UV = v.UV.ToVector2();
            UVA1 = v.UVA1.ToVector4();
            UVA2 = v.UVA2.ToVector4();
            UVA3 = v.UVA3.ToVector4();
            UVA4 = v.UVA4.ToVector4();
        }
        public static VertexElement[] GetElements()
        {
            short offset = 0;
            VertexElement[] result = new VertexElement[7];
            result[0] = new VertexElement
            {
                Offset = offset,
                Type = DeclarationType.Float3,
                Usage = DeclarationUsage.Position
            };
            offset += (short)Marshal.SizeOf(typeof(Vector3));

            result[1] = new VertexElement
            {
                Offset = offset,
                Type = DeclarationType.Float3,
                Usage = DeclarationUsage.Normal
            };
            offset += (short)Marshal.SizeOf(typeof(Vector3));
            
            result[2] = new VertexElement
            {
                Offset = offset,
                Type = DeclarationType.Float2,
                Usage = DeclarationUsage.TextureCoordinate
            };
            offset += (short)Marshal.SizeOf(typeof(Vector2));

            for (byte i = 1; i < 5; i++)
            {
                result[2+i] = new VertexElement
                {
                    Offset = offset,
                    Type = DeclarationType.Float4,
                    Usage = DeclarationUsage.TextureCoordinate,
                    UsageIndex = i
                };
                offset += (short)Marshal.SizeOf(typeof(Vector2));                
            }
            return result;
        }
    }
}