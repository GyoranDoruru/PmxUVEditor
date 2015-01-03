using System.Drawing;
using SlimDX;
using SlimDX.Direct3D9;

namespace MyUVEditor
{
    class MaterialManager
    {
        private Color4 XF = Color.Black;
        private Color4 BG = Color.White;
        private Color4 VP = Color.Black;
        private Color4 SP = Color.Red;
        private Color4 SA = new Color4(0.1f, 1.0f, 1.0f, 1.0f);
        private Color4 NV = Color.Yellow;
        public Color4 BGColor { get; private set; }
        public Material ForXFile
        {
            get
            {
                Material m = new Material
                {
                    Diffuse = Color.Black,
                    Emissive = XF,
                };
                return m;
            }
        }
        public Material ForTexBackGround
        {
            get
            {
                Material m = new Material
                {
                    Emissive = BG,
                };
                return m;
            }
        }
        public Material ForVertexPoint
        {
            get
            {
                Material m = new Material
                {
                    Emissive = VP,
                    Diffuse = Color.Black
                };
                return m;
            }
        }
        public Material ForSelectedPoint
        {
            get
            {
                Material m = new Material
                {
                    Diffuse = Color.Black,
                    Emissive = SP
                };
                return m;
            }
        }
        public Material ForSelectedArea
        {
            get
            {
                Material m = new Material
                {
                    Diffuse = Color.Black,
                    Emissive = SA
                };
                return m;
            }
        }
        public Material ForNearVertex
        {
            get
            {
                Material m = new Material
                {
                    Diffuse = Color.Black,
                    Emissive = NV
                };
                return m;
            }
        }

        public MaterialManager()
        {
            BGColor = new Color4(0.3f, 0.3f, 0.3f);
        }
    }
}
