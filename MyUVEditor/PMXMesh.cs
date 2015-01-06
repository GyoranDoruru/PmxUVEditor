using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using SlimDX.Direct3D9;
using PEPlugin.Pmx;

//using PNTVertex = MyUVEditor.CustomVertex.PositionNormalTextured;
using PNTVertex = MyUVEditor.PMXVertex;

namespace MyUVEditor
{
    public class PMXMesh : Mesh
    {
        private IPXPmx Pmx;
        private int[] IndexArray;
        //private PMXVertex[] VertexArray;
        private PNTVertex[] VertexArray;
        private AttributeRange[] ARangeArray;
        private HashSet<int>[] VIndices;
        public ExtendedMaterial[] ExMaterialArray { get; private set; }
        public MaterialManager MatManager { get; private set; }

        static public PMXMesh GetPMXMesh(Device device, IPXPmx pmx)
        {
            try
            {
                int faceCount = 0;
                foreach (var m in pmx.Material)
                    faceCount += m.Faces.Count;
                return new PMXMesh(pmx, device, faceCount);
            }
            catch (SlimDXException e)
            {
                Console.WriteLine(e);
                throw e;
            }
        }

        private PMXMesh(IPXPmx pmx, Device device, int faceCount)
            : base(device, faceCount, pmx.Vertex.Count, MeshFlags.Use32Bit | MeshFlags.Managed, PNTVertex.Format)
//            : base(device, faceCount, pmx.Vertex.Count, MeshFlags.Use32Bit | MeshFlags.Managed, PNTVertex.GetElements())
        {
            MatManager = new MaterialManager();
            Pmx = pmx;
            InitVertexArray();
            InitVertexArray();

            var vDic = GetVDic();
            InitIndexArray(vDic);
            InitMaterialArray(vDic);

            SetIndexBuffer();
            SetVertexBuffer();
            SetAttributeTable(ARangeArray);
            SetARangeBuffer();
            SetMaterials(ExMaterialArray);
        }

        private Dictionary<IPXVertex, int> GetVDic()
        {
            Dictionary<IPXVertex, int> vDic = new Dictionary<IPXVertex, int>();
            for (int i = 0; i < Pmx.Vertex.Count; i++)
            {
                vDic.Add(Pmx.Vertex[i], i);
            }
            return vDic;
        }

        private void InitIndexArray(Dictionary<IPXVertex,int> vDic)
        {
            IndexArray = new int[FaceCount * 3];
            VIndices = new HashSet<int>[Pmx.Material.Count];
            int count = 0;
            int midx = 0;
            int vIndex;
            foreach (var m in Pmx.Material)
            {
                VIndices[midx] = new HashSet<int>();
                foreach (var f in m.Faces)
                {
                    vIndex = vDic[f.Vertex1];
                    IndexArray[count] = vIndex;
                    VIndices[midx].Add(vIndex);
                    count++;
                    vIndex = vDic[f.Vertex2];
                    IndexArray[count] = vIndex;
                    VIndices[midx].Add(vIndex);
                    count++;
                    vIndex = vDic[f.Vertex3];
                    IndexArray[count] = vIndex;
                    VIndices[midx].Add(vIndex);
                    count++;
                }
                midx++;
            }


        }
        
        private void InitVertexArray()
        {
            VertexArray = new PNTVertex[VertexCount];
            for (int i = 0; i < VertexCount; i++)
            {
                VertexArray[i] = new PNTVertex(Pmx.Vertex[i]);
            }
        }
        
        private void InitMaterialArray(Dictionary<IPXVertex,int> vDic)
        {
            ARangeArray = new AttributeRange[Pmx.Material.Count];
            ExMaterialArray = new ExtendedMaterial[Pmx.Material.Count];
            int faceOffset = 0;
            for (int i = 0; i < Pmx.Material.Count; i++)
            {
                IPXMaterial m = Pmx.Material[i];
                ARangeArray[i] = new AttributeRange
                {
                    AttribId = i,
                    FaceCount = m.Faces.Count,
                    FaceStart = faceOffset,
                    VertexCount = VIndices[i].Count,
                    VertexStart = vDic[m.Faces[0].Vertex1]
                };
                faceOffset += m.Faces.Count;
                ExMaterialArray[i] = new ExtendedMaterial
                {
                    MaterialD3D = new Material
                    {
                        //Ambient = new Color4(1,m.Ambient.R,m.Ambient.G,m.Ambient.B),
                        Emissive = new Color4(1,m.Ambient.R,m.Ambient.G,m.Ambient.B),
                        Diffuse = m.Diffuse.ToColor4(),
                        Power = m.Power,
                        Specular = new Color4(m.Specular.ToColor3())
                    },
                    TextureFileName = m.Tex
                };

            }
        }
        private void SetIndexBuffer()
        {
            using (DataStream data = LockIndexBuffer(LockFlags.None))
            {
                data.WriteRange(IndexArray);
                UnlockIndexBuffer();
            }
        }
        private void SetVertexBuffer()
        {
            using (DataStream data = LockVertexBuffer(LockFlags.None))
            {
                data.WriteRange(VertexArray);
                UnlockVertexBuffer();
            }
        }
        private void SetARangeBuffer()
        {
            using (DataStream data = LockAttributeBuffer(LockFlags.None))
            {
                foreach (var a in ARangeArray)
                {
                    for (int i = 0; i < a.FaceCount;i++ )
                    {
                        data.Write<int>(a.AttribId);
                    }
                }
                UnlockAttributeBuffer();
            }
        }
    }
}
