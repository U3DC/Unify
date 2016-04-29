using Newtonsoft.Json;
using Rhino;
using Rhino.DocObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Unify.Utilities
{
    public class UnifyObject
    {
        // all
        public virtual string ObjType { get; set; }
        public virtual string Guid { get; set; }
        public virtual string Name { get; set; }
        public virtual string UniqueName { get; set; }

        // geometry
        public virtual string Layer { get; set; }

        // lights
        public virtual string LightType { get; set; }
        public virtual string Diffuse { get; set; } // shared w materials
        public virtual string Target { get; set; }
        public virtual string Intensity { get; set; }
        public virtual string Range { get; set; }
        public virtual string SpotAngle { get; set; }
        public virtual string ShadowIntensity { get; set; }
        public virtual string Location { get; set; }

        // materials
        public virtual string DiffuseTexture { get; set; }
        public virtual string SpecularColor { get; set; }
        public virtual string EmissionColor { get; set; }
        public virtual string ReflectionColor { get; set; }
        public virtual string Metallic { get; set; }
        public virtual string TransparencyTexture { get; set; }
        public virtual string EnvironmentTexture { get; set; }
        public virtual string BumpTexture { get; set; }
        public virtual string Transparency { get; set; }
    }

    public class UnifyLight : UnifyObject
    {
        public override string ObjType { get; set; }
        public override string LightType { get; set; }
        public override string Guid { get; set; }
        public override string Diffuse { get; set; }
        public override string Target { get; set; }
        public override string Intensity { get; set; }
        public override string Location { get; set; }
        public override string Range { get; set; }
        public override string SpotAngle { get; set; }
        public override string ShadowIntensity { get; set; }

        public UnifyLight(LightObject light)
        {
            this.ObjType = "LightObject";
            this.LightType = light.LightGeometry.LightStyle.ToString();
            this.Guid = light.Id.ToString();
            this.Diffuse = Utility.ConvertColor(light.LightGeometry.Diffuse);
            this.UniqueName = light.LightGeometry.LightStyle.ToString() + "-" + light.Id.ToString();

            // target target from location + direction
            Rhino.Geometry.Transform xf = Rhino.Geometry.Transform.Translation(light.LightGeometry.Direction);
            Rhino.Geometry.Point3d target = light.LightGeometry.Location;
            target.Transform(xf);
            this.Target = target.ToString();

            this.Intensity = light.LightGeometry.Intensity.ToString();
            this.Location = light.LightGeometry.Location.ToString();
            this.Range = light.LightGeometry.Direction.Length.ToString();
            this.SpotAngle = light.LightGeometry.SpotAngleRadians.ToString();
            this.ShadowIntensity = light.LightGeometry.SpotLightShadowIntensity.ToString();
        }
    }

    public class UnifyMaterial : UnifyObject
    {
        public override string ObjType { get; set; }
        public override string Guid { get; set; }
        public override string Name { get; set; }
        public override string UniqueName { get; set; }
        public override string Diffuse { get; set; }
        public override string SpecularColor { get; set; }
        public override string EmissionColor { get; set; }
        public override string ReflectionColor { get; set; }
        public override string Metallic { get; set; }
        public override string DiffuseTexture { get; set; }
        public override string TransparencyTexture { get; set; }
        public override string EnvironmentTexture { get; set; }
        public override string BumpTexture { get; set; }
        public override string Transparency { get; set; }

        public UnifyMaterial(Material mat)
        {
            this.ObjType = "MaterialObject";
            this.Guid = mat.Id.ToString();
            this.Name = mat.Name;
            this.Diffuse = Utility.ConvertColor(mat.DiffuseColor);
            this.SpecularColor = Utility.ConvertColor(mat.SpecularColor);
            this.EmissionColor = Utility.ConvertColor(mat.EmissionColor);
            this.ReflectionColor = Utility.ConvertColor(mat.ReflectionColor);
            this.Metallic = mat.Shine.ToString();

            Texture diffuseTexture = mat.GetBitmapTexture();
            Texture transTexture = mat.GetTransparencyTexture();
            Texture envTexture = mat.GetEnvironmentTexture();
            Texture bumpTexture = mat.GetBumpTexture();
            if (diffuseTexture != null) this.DiffuseTexture = diffuseTexture.FileName;
            if (transTexture != null) this.TransparencyTexture = transTexture.FileName;
            if (envTexture != null) this.EnvironmentTexture = envTexture.FileName;
            if (bumpTexture != null) this.BumpTexture = bumpTexture.FileName;

            this.Transparency = mat.Transparency.ToString();
        }
    }
    public class UnifyGeometry : UnifyObject
    {
        public override string ObjType { get; set; }
        public override string Layer { get; set; }
        public override string Guid { get; set; }

        public UnifyGeometry(RhinoObject obj)
        {
            this.ObjType = "GeometryObject";
            this.Layer = RhinoDoc.ActiveDoc.Layers[obj.Attributes.LayerIndex].FullPath.Replace(":", "_");
            this.Guid = obj.Id.ToString();
        }
    }

    public class UnifyCamera : UnifyObject
    {
        public override string ObjType { get; set; }
        public override string Guid { get; set; }
        public override string Location { get; set; }

        public UnifyCamera(ViewInfo cam)
        {
            this.ObjType = "ViewCamera";
            this.Guid = cam.Viewport.Id.ToString();
            this.Location = cam.Viewport.CameraLocation.ToString();
        }
    }

    static class Utility
    {
        public static string ConvertColor(System.Drawing.Color col)
        {
            string r = col.R.ToString();
            string g = col.G.ToString();
            string b = col.B.ToString();
            return String.Join(",", new string[] { r, g, b });
        }

        private static string GetOBJOptions()
        {
            StringBuilder sb = new StringBuilder();
            string[] objOptions = new string[]
                {
                    "_Geometry=_Mesh ",
                    "_EndOfLine=CRLF ",
                    "_ExportRhinoObjectNames=_DoNotExportObjectNames ",
                    "_ExportMeshTextureCoordinates=_Yes ",
                    "_ExportMeshVertexNormals=_Yes ",
                    "_CreateNGons=_No ",
                    "_ExportMaterialDefinitions=_No ",
                    "_YUp=_Yes ",
                    "_WrapLongLines=_Yes ",
                    "_VertexWelding=_Unmodified ",
                    "_WritePrecision=16 ",
                    "_Enter ",

                    "_DetailedOptions ",
                    "_JaggedSeams=_No ",
                    "_PackTextures=_No ",
                    "_Refine=_Yes ",
                    "_SimplePlane=_No ",

                    "_AdvancedOptions ",
                    "_Angle=50 ",
                    "_AspectRatio=0 ",
                    "_Distance=0.0",
                    "_Density=0 ",
                    "_Density=0.45 ",
                    "_Grid=0 ",
                    "_MaxEdgeLength=0 ",
                    "_MinEdgeLength=0.0001 "
                };
            for (int i = 0; i < objOptions.Length; i++)
            {
                sb.Append(objOptions[i]);
            }
            return sb.ToString();
        }

        public static void ExportOBJ(List<Guid> objs, string folderPath)
        {
            RhinoDoc.ActiveDoc.Objects.UnselectAll();
            RhinoDoc.ActiveDoc.Objects.Select(objs);

            string objOptions = GetOBJOptions();
            string fileName = "\\" + System.IO.Path.GetFileNameWithoutExtension(RhinoDoc.ActiveDoc.Name) + ".obj ";
            string filePath = folderPath + fileName;
            string script = string.Concat("_-Export ", filePath, objOptions, " y=y", " _Enter _Enter");
            RhinoApp.RunScript(script, false);
            RhinoApp.RunScript("_-SelNone", true);
        }

        public static bool WriteSetings(List<List<object>> objs, string settingsPath)
        {
            string json = JsonConvert.SerializeObject(objs, Formatting.Indented);
            System.IO.File.WriteAllText(settingsPath, json);
            return true;
        }
    }
}
