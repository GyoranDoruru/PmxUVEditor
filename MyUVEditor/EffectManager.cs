using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using SlimDX.Direct3D9;

namespace MyUVEditor
{
    public class EffectManager
    {
        static readonly string defaultPath
            = @"D:\Documents\Visual Studio 2010\Projects\MyUVEditor\MyUVEditor\test.fx";
        static readonly string ObjTecName = "MainTec";
        static readonly string UVTecName = "UVTec";

        Effect Effect;
        public EffectHandle ObjTec { get; private set; }
        public EffectHandle UVTec { get; private set; }
        public EffectManager(Device device)
        {
            try
            {
                Console.WriteLine(System.IO.File.Exists(defaultPath));
                var dicTec = new Dictionary<string, EffectHandle>();
                Effect = Effect.FromFile(device, defaultPath, ShaderFlags.Debug);
                EffectHandle h = Effect.FindNextValidTechnique(null);
                while(h!=null)
                {
                    string name = Effect.GetTechniqueDescription(h).Name;
                    dicTec.Add(name, h);
                    h = Effect.FindNextValidTechnique(h);
                }
                ObjTec = dicTec[ObjTecName];
                UVTec = dicTec[UVTecName];
            }
            catch (SlimDX.CompilationException se)
            {
                Console.WriteLine(se);
            }
        }
    }
}
