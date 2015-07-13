using System;
using System.Collections.Generic;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.D3DCompiler;
using PEPlugin.Pmx;

namespace MyUVEditor.DirectX11
{
    using EffectVariableList = List<EffectVariable>;
    using SemanticsDic = Dictionary<string, List<EffectVariable>>;
    using AnnotationDic = Dictionary<string, Dictionary<string, List<EffectVariable>>>;
    using TexKind = TextureUtility.TextureKind;

    class EffectManager11 : IDisposable
    {
        static HashSet<string> SemanticsGeometryTrans = new HashSet<string>{
            "world","view","projection","worldview","viewprojection","worldviewprojection"
        };
        static string GetDeffaultAnnotation(string semantics)
        {
            switch (semantics)
            {
                case "world":
                case "view":
                case "projection":
                case "worldview":
                case "viewprojection":
                case "worldviewprojection":
                case "position":
                case "direction":
                    return "camera";
                case "diffuse":
                case "ambient":
                case "specular":
                case "edgecolor":
                case "emissive":
                case "specularpower":
                case "tooncolor":
                    return "geometry";
                case "groundshadowcolor":
                    return "light";
            }
            foreach (var s in SemanticsGeometryTrans)
            {
                if (semantics.StartsWith(s))
                {
                    return "camera";
                }
            }
            return "";
        }

        static public Effect InitEffect(Device device)
        {
            using (ShaderBytecode shaderByteCode = ShaderBytecode.Compile(
                Properties.Resources.myeffect, "fx_5_0", ShaderFlags.None, EffectFlags.None))
            {
                return new Effect(device, shaderByteCode);
            }
        }

        static public ShaderSignature Signature(Effect effect)
        {
            return effect.GetTechniqueByIndex(0).GetPassByIndex(0).Description.Signature;
        }

        public Effect Effect { get; private set; }
        private AnnotationDic m_EffectValueDic;  // annotation-semantics-valuelist
        public EffectManager11(Effect effect)
        {
            Effect = effect;
            InitEffectValueDic();
        }

        private void InitEffectValueDic()
        {
            m_EffectValueDic = new AnnotationDic();
            for (int i = 0; i < Effect.Description.GlobalVariableCount; i++)
            {
                var eValue = Effect.GetVariableByIndex(i);
                var semantics = eValue.Description.Semantic.ToLower();
                string annotation = "";
                for (int j = 0; j < eValue.Description.AnnotationCount; j++)
                {
                    var val = eValue.GetAnnotationByIndex(j);
                    if (val.Description.Name.ToLower() != "object")
                        continue;
                    var valString = val.AsString();
                    if (valString == null)
                        continue;
                    annotation = valString.GetString().ToLower();
                    break;
                }
                if (annotation == "")
                {
                    annotation = GetDeffaultAnnotation(semantics);
                }
                if (!m_EffectValueDic.ContainsKey(annotation))
                    m_EffectValueDic.Add(annotation, new SemanticsDic());
                var semdic = m_EffectValueDic[annotation];
                if (!semdic.ContainsKey(semantics))
                    semdic.Add(semantics, new EffectVariableList());
                semdic[semantics].Add(eValue);
            }
        }

        private EffectVariableList GetVarialble(string semantics, string annotation)
        {
            semantics = semantics.ToLower();
            annotation = annotation.ToLower();
            SemanticsDic semDic;
            if(!m_EffectValueDic.TryGetValue(annotation, out semDic))
                return null;
            EffectVariableList variableList;
            if (!semDic.TryGetValue(semantics, out variableList))
                return null;
            return variableList;
        }

        private void SetVariable(string semantics, string annotation, Matrix matrix)
        {
            var list = GetVarialble(semantics, annotation);
            if (list == null) return;
            foreach(var v in list)
                v.AsMatrix().SetMatrix(matrix);
        }
        private void SetVariable(string semantics, string annotation, Vector2 vector)
        {
            var list = GetVarialble(semantics, annotation);
            if (list == null) return;
            foreach (var v in list)
                v.AsVector().Set(vector);
        }
        private void SetVariable(string semantics, string annotation, Vector3 vector)
        {
            var list = GetVarialble(semantics, annotation);
            if (list == null) return;
            foreach (var v in list)
                v.AsVector().Set(vector);
        }
        private void SetVariable(string semantics, string annotation, Vector4 vector)
        {
            var list = GetVarialble(semantics, annotation);
            if (list == null) return;
            foreach (var v in list)
                v.AsVector().Set(vector);
        }
        private void SetVariable(string semantics, string annotation, float value)
        {
            var list = GetVarialble(semantics, annotation);
            if (list == null) return;
            foreach (var v in list)
                v.AsScalar().Set(value);
        }
        private void SetVariable(string semantics, string annotation, ShaderResourceView resource)
        {
            var list = GetVarialble(semantics, annotation);
            if (list == null) return;
            foreach (var v in list)
                v.AsResource().SetResource(resource);
        }
        public void SetTechAndPass(int techIndex, int passIndex)
        {
            Effect.GetTechniqueByIndex(techIndex).GetPassByIndex(passIndex).Apply(
                Effect.Device.ImmediateContext);
        }

        public void SetWorld(Matrix matrix)
        {
            SetVariable("world", "camera", matrix);
        }
        public void SetCamera(Camera.ICamera camera){
            SemanticsDic semanDic = m_EffectValueDic["camera"];
            foreach (var pair in semanDic)
            {
                string semantics = pair.Key;
                string baseMatrixName = "";
                foreach (var geoSem in SemanticsGeometryTrans)
                {
                    if (semantics.StartsWith(geoSem))
                        baseMatrixName = geoSem;
                }
                if (baseMatrixName != "")
                {
                    Matrix matrix;
                    switch (baseMatrixName) {
                        case "world":
                            matrix = camera.World;
                            break;
                        case "view":
                            matrix = camera.View;
                            break;
                        case "projection":
                            matrix = camera.Projection;
                            break;
                        case "worldview":
                            matrix = camera.World*camera.View;
                            break;
                        case "viewprojection":
                            matrix = camera.View * camera.Projection;
                            break;
                        case "worldviewprojection":
                            matrix = camera.WorldViewProjection;
                            break;
                        default:
                            matrix = Matrix.Identity;
                            break;
                    }
                    string option = semantics.Replace(baseMatrixName, "");
                    switch (option)
                    {
                        case "transpose":
                            matrix = Matrix.Transpose(matrix);
                            break;
                        case "inverse":
                            matrix = Matrix.Invert(matrix);
                            break;
                    }
                    SetVariable(semantics, "camera", matrix);
                }
                else
                {
                    switch (semantics)
                    {
                        case "position":
                            SetVariable(semantics, "camera", camera.Position);
                            break;
                        case "direction":
                            SetVariable(semantics, "camera", camera.Direction);
                            break;
                    }
                }
            }
        }

        public void SetObjectTexture(ShaderResourceView texture)
        {
            SetVariable("materialtexture", "", texture);

        }
        public void SetMaterial(IPXMaterial material, ShaderResourceView[] textures)
        {
            if (material != null)
            {
                SetVariable("diffuse", "geometry", material.Diffuse.ToVector4());
                SetVariable("ambient", "geometry", V4ToVector3(material.Diffuse));
                SetVariable("emissive", "geometry", material.Ambient.ToVector3());
                SetVariable("specular", "geometry", material.Specular.ToVector3());
                SetVariable("specularpower", "geometry", material.Power);
            }
            SetVariable("materialtexture", "", textures[(int)TexKind.OBJ]);
            SetVariable("materialtoontexture","",textures[(int)TexKind.TOON]);
            SetVariable("materialspheremap", "", textures[(int)TexKind.SPHERE]);
        }
        public void SetLight(Light.ILight light)
        {
            SetVariable("worldviewprojection", "light", light.LightWorldViewProjection);
            SetVariable("direction", "light", light.LightDirection);
            SetVariable("diffuse", "light", light.Diffuse);
            SetVariable("ambient", "light", light.Ambient);
            SetVariable("specular", "light", light.Specular);
        }
        public void Dispose()
        {
            Effect = null;
        }

        static Vector3 V4ToVector3(PEPlugin.SDX.V4 src)
        {
            return new Vector3(src.X, src.Y, src.Z);
        }
    }

}
