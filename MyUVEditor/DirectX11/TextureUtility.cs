using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using SlimDX;
using SlimDX.Direct3D11;

namespace MyUVEditor.DirectX11
{
    class TextureUtility
    {
        static internal Texture2D BitmapToTexture(
            SlimDX.Direct3D11.Device device, System.Drawing.Bitmap bitmap)
        {
            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Png);
                int len = (int)stream.Position;
                stream.Position = 0;
                return Texture2D.FromStream(device, stream, (int)stream.Length);
            }

        }

        static internal ShaderResourceView CreateWhiteTex(Device device)
        {
            using (var texture = BitmapToTexture(device, Properties.Resources.WhiteTex))
            {
                return new ShaderResourceView(device, texture);
            }
        }

        static internal ShaderResourceView CreateTex(Device device, string path)
        {
            try
            {
                string ext = Path.GetExtension(path);
                if (ext == ".tga") return CreateTGATex(device, path);
                else return ShaderResourceView.FromFile(device, path);
            }
            catch (System.Exception ex) { System.Windows.Forms.MessageBox.Show(ex.Message); return null; }
        }

        static private ShaderResourceView CreateTGATex(Device device, string path)
        {
            using (var tgaBitMap = Paloma.TargaImage.LoadTargaImage(path))
            {
                using (var stream = new MemoryStream())
                {
                    tgaBitMap.Save(stream, ImageFormat.Png);
                    stream.Position = 0;
                    return ShaderResourceView.FromStream(device, stream, (int)stream.Length);
                }
            }
        }

        internal enum TextureKind
        {
            OBJ = 0,
            TOON,
            SPHERE,
            NUM
        }

        static public Color4 GetTexLeftBottomColor(string path)
        {
            var bitmap = new System.Drawing.Bitmap(path);
            Color pixel = bitmap.GetPixel(0, bitmap.Height - 1);
            return new Color4(pixel);
        }
    }
}
