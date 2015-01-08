using System;
using System.Drawing;
using System.Collections.Generic;
using SlimDX;
using SlimDX.Direct3D9;
using System.IO;
using System.Reflection;


namespace MyUVEditor
{
    public class MaterialManager:IDisposable
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

        private Texture White;
        public Dictionary<string, Texture> DicObjTex { get; private set; }
        public Dictionary<string, Texture> DicToonTex { get; private set; }
        public Dictionary<string, Texture> DicSphTex { get; private set; }
        string toonDir;
        public Light Light { get; private set; }
        public MaterialManager(string toonDir = @"D:\Documents\MMD\PmxEditor_0222\_data\toon")
        {
            BGColor = new Color4(0.3f, 0.3f, 0.3f);
            Light = new Light
            {
                Type = LightType.Directional,
                Diffuse = Color.White,
                Ambient = Color.GhostWhite,
                Direction = new Vector3(0.0f, -1.0f, 0.0f)
            };
            this.toonDir = toonDir;
            DicObjTex = new Dictionary<string, Texture>();
            DicSphTex = new Dictionary<string, Texture>();
            DicToonTex = new Dictionary<string, Texture>();
        }

        private void SetWhite(Device device)
        {
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                using (Stream s = assembly.GetManifestResourceStream("MyUVEditor.WhiteTex.bmp"))
                {
                    White = Texture.FromStream(device, s);
                }
            }
            catch (SlimDXException ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
        }

        private void SetDic(Device device, HashSet<string> hash, Dictionary<string, Texture> dic)
        {
            foreach (var s in hash)
            {
                if (s != "" && File.Exists(s))
                {
                    dic.Add(s, Texture.FromFile(device, s));
                }
                else
                {
                    dic.Add(s, White);
                }
            }

        }
        public void SetTexDics(PMXMesh mesh)
        {
            SetWhite(mesh.Device);
            string savedCurrentDir = Directory.GetCurrentDirectory();

            Directory.SetCurrentDirectory(Path.GetDirectoryName(mesh.Pmx.FilePath));
            HashSet<string> hashObj = new HashSet<string>();
            HashSet<string> hashToon = new HashSet<string>();
            HashSet<string> hashSph = new HashSet<string>();
            foreach (var m in mesh.Pmx.Material)
            {
                hashObj.Add(m.Tex);
                hashToon.Add(m.Toon);
                hashSph.Add(m.Sphere);
            }
            SetDic(mesh.Device, hashObj, DicObjTex);
            SetDic(mesh.Device, hashToon, DicToonTex);
            SetDic(mesh.Device, hashSph, DicSphTex);

            // デフォルトToon追加
            Directory.SetCurrentDirectory(toonDir);
            string[] defToon = 
            {
                "toon01.bmp", "toon02.bmp",
                "toon03.bmp", "toon04.bmp",
                "toon05.bmp", "toon06.bmp",
                "toon07.bmp", "toon08.bmp",
                "toon09.bmp", "toon10.bmp"
            };
            foreach (string s in defToon)
            {
                if (!DicToonTex.ContainsKey(s))
                {
                    DicToonTex.Add(s, Texture.FromFile(mesh.Device, s));
                }
            }

            Directory.SetCurrentDirectory(savedCurrentDir);

        }


        public void Dispose()
        {
            foreach (var t in DicObjTex.Values)
            {
                t.Dispose();
            }
            foreach (var t in DicToonTex.Values)
            {
                t.Dispose();
            }
            foreach (var t in DicSphTex.Values)
            {
                t.Dispose();
            }
            White.Dispose();
        }
    }
}
