using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using SlimDX.Direct3D9;

namespace MyUVEditor
{
    class EffectManager
    {
        static readonly string defaultPath = "test.fx";
        Effect Effect;
        EffectManager(Device device)
        {
            var dicTec = new Dictionary<string, EffectHandle>();
            Effect = Effect.FromFile(device, defaultPath, ShaderFlags.None);
            EffectHandle h = null;
            EffectHandle hNext;
            do
            {
                hNext = Effect.FindNextValidTechnique(h);
                string name = Effect.GetTechniqueDescription(hNext).Name;
                dicTec.Add(name, hNext);
                h = hNext;

            } while (h != null);
        }
    }
}
