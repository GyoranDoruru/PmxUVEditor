using System.Collections.Generic;
using SlimDX;
using SlimDX.Direct3D11;
using PEPlugin.Pmx;


namespace MyUVEditor.DirectX11
{
    class DrawableSprite : DrawablePmx
    {
        private float m_ratio = 1.0f;  // width/height

        static IPXPmx SpritePmx(IPXPmx pmx)
        {
            var res = PEPlugin.PEStaticBuilder.Pmx.Pmx();
            for (int i = 0; i < 4; ++i)
            {
                res.Vertex.Add(PEPlugin.PEStaticBuilder.Pmx.Vertex());
            }

            res.Vertex[1].Position.X = 1.0f;
            res.Vertex[1].UV.U = 1.0f;

            res.Vertex[2].Position.X = 1.0f;
            res.Vertex[2].Position.Y = 1.0f;
            res.Vertex[2].UV.U = 1.0f;
            res.Vertex[2].UV.V = 1.0f;

            res.Vertex[3].Position.Y = 1.0f;
            res.Vertex[3].UV.V = 1.0f;

            var f1 = PEPlugin.PEStaticBuilder.Pmx.Face();
            var f2 = PEPlugin.PEStaticBuilder.Pmx.Face();
            f1.Vertex1 = res.Vertex[0];
            f1.Vertex2 = res.Vertex[1];
            f1.Vertex3 = res.Vertex[2];
            f2.Vertex1 = res.Vertex[0];
            f2.Vertex2 = res.Vertex[2];
            f2.Vertex3 = res.Vertex[3];
            foreach (var m in pmx.Material)
            {
                var cm = (IPXMaterial)m.Clone();
                cm.Faces.Clear();
                cm.Faces.Add(f1);
                cm.Faces.Add(f2);
                res.Material.Add(cm);
            }
            res.Normalize();
            return res;
        }

        public DrawableSprite(IPXPmx pmx) : base(SpritePmx(pmx)) { }


    }
}
