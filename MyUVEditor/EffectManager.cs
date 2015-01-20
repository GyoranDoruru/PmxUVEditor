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

        public Effect Effect;
        public EffectHandle ObjTec { get; private set; }
        public EffectHandle UVTec { get; private set; }
        EffectHandle[,] geoHandles = new EffectHandle[(int)eGeoSemantics.NUM, (int)eGeoAnotations.NUM];
        EffectHandle hWorld;
        EffectHandle hView;
        EffectHandle hWorldViewProj;
        EffectHandle hMatDiffuse;
        EffectHandle hMatAmbient;
        EffectHandle hMatEmissive;
        EffectHandle hMatSpecular;
        EffectHandle hMatSpecPower;
        EffectHandle hToonColor;
        EffectHandle hLightDiffuse;
        EffectHandle hLightAmbient;
        EffectHandle hLightSpecular;


        public EffectManager(Device device)
        {
            try
            {
                //device.SetRenderState(RenderState.ShadeMode,ShadeMode.Gouraud);
                var dicTec = new Dictionary<string, EffectHandle>();
                Effect = Effect.FromFile(device, defaultPath, ShaderFlags.Debug);
                EffectHandle h = Effect.FindNextValidTechnique(null);
                while (h != null)
                {
                    string name = Effect.GetTechniqueDescription(h).Name;
                    dicTec.Add(name, h);
                    h = Effect.FindNextValidTechnique(h);
                }
                ObjTec = dicTec[ObjTecName];
                UVTec = dicTec[UVTecName];
                GetGeoHandle();
            }
            catch (SlimDX.CompilationException se)
            {
                Console.WriteLine(se);
            }
        }

        public void BeginUVTec()
        {
            Effect.Technique = UVTec;
            Effect.Begin();
        }
        public void EndTec()
        {
            Effect.End();
        }
        public void BeginPass(int i)
        {
            Effect.BeginPass(i);
        }

        public void EndPass()
        {
            Effect.EndPass();
        }

        public void GetGeoHandle()
        {
            for (int i = 0; i < Effect.Description.Parameters; i++)
            {
                EffectHandle h = Effect.GetParameter(null, i);
                ParameterDescription pdesc = Effect.GetParameterDescription(h);
                eGeoSemantics geoSem;
                if (!Enum.TryParse(pdesc.Semantic, out geoSem)) continue;
                int geoAnnot = 0;
                for (int j = 0; j < pdesc.Annotations; i++)
                {
                    EffectHandle a = Effect.GetAnnotation(h, j);
                    ParameterDescription adesc = Effect.GetParameterDescription(a);
                    if (adesc.Name == "Object")
                    {
                        string astring = Effect.GetString(a);
                        eGeoAnotations eGeoAnnot;
                        if (Enum.TryParse(astring, out eGeoAnnot))
                        {
                            geoAnnot = (int)eGeoAnnot;
                        }
                        else
                        {
                            geoAnnot = (int)eGeoAnotations.NUM;
                        }
                        break;
                    }
                }
                if (geoSem < eGeoSemantics.NUM && geoAnnot < (int)eGeoAnotations.NUM)
                    geoHandles[(int)geoSem, geoAnnot] = h;
            }
        }

        public void SetMatrix(Camera c)
        {
            EffectHandle h = geoHandles[(int)eGeoSemantics.WORLDVIEWPROJECTION, (int)eGeoAnotations.Camera];
            Effect.SetValue(h, c.WorldViewProjection);
        }
    }
}
