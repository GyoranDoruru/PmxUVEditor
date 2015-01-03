using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace MyUVEditor
{
    class TextureInfo
    {
        string ex = "";
        int width = 256;
        int height = 256;
        public float Aspect{get{return (float)width/(float)height;}}

        public TextureInfo(string filename)
        {
            if (filename == null || filename == "") return;
            ex = Path.GetExtension(filename);
            try
            {
                using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    switch (ex)
                    {
                        case ".tga":
                        case ".TGA":
                            TGAReader(fs);
                            break;
                        case ".dds":
                        case ".DDS":
                            DDSReader(fs);
                            break;
                        default:
                            OTherReader(fs);
                            break;
                    }
                }
            }
            catch { return; }
        }

        private byte[] SizeFromBin(FileStream fs, int offset, int length)
        {
            byte[] result = new byte[length * 2];
            fs.Seek(offset, SeekOrigin.Begin);
            fs.Read(result, 0, length * 2);
            return result;
        }
        private void TGAReader(FileStream fs)
        {
            byte[] size = SizeFromBin(fs, 12, 2);
            width = BitConverter.ToUInt16(size, 0);
            height = BitConverter.ToUInt16(size, 2);
        }
        private void DDSReader(FileStream fs)
        {
            byte[] size = SizeFromBin(fs, 12, 4);
            width = (int)BitConverter.ToUInt32(size, 0);
            height = (int)BitConverter.ToUInt32(size, 4);
        }
        private void OTherReader(FileStream fs)
        {
            Image image = Image.FromStream(fs);
            width = image.Size.Width;
            height = image.Size.Height;
        }
    }
}
