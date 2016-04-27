using Newtonsoft.Json;
using Rhino;
using Rhino.DocObjects;
using System;
using System.Collections.Generic;

namespace UnityUtilities
{
    public class UnifyObject
    {
        public virtual string ObjType { get; set; }
        public virtual string Guid { get; set; }
        public virtual string LightType { get; set; }
        public virtual string Diffuse { get; set; }
        public virtual string Target { get; set; }
        public virtual string Intensity { get; set; }
        public virtual string Location { get; set; }
        public virtual string Range { get; set; }
        public virtual string SpotAngle { get; set; }
        public virtual string ShadowIntensity { get; set; }

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
            this.ObjType = "Light";
            this.LightType = light.LightGeometry.LightStyle.ToString();
            this.Guid = light.Id.ToString();
            this.Diffuse = light.LightGeometry.Diffuse.ToArgb().ToString();

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
    public class UnifyCamera : UnifyObject
    {
        public override string ObjType { get; set; }
        public override string Guid { get; set; }
        //public string Name { get; set; }
        //public string LensLength { get; set; }
        public override string Location { get; set; }
        //public string Target { get; set; }
        //public string ClippingPlaneNear { get; set; }
        //public string ClippingPlaneFar { get; set; }

        public UnifyCamera(ViewInfo cam)
        {
            this.ObjType = "ViewCamera";
            this.Guid = cam.Viewport.Id.ToString();
            //this.Name = cam.Name;
            //this.LensLength = cam.Viewport.Camera35mmLensLength.ToString();
            this.Location = cam.Viewport.CameraLocation.ToString();
            //this.Target = cam.Viewport.TargetPoint.ToString();
            //this.ClippingPlaneNear = cam.Viewport.FrustumNear.ToString();
            //this.ClippingPlaneFar = cam.Viewport.FrustumFar.ToString();
        }
    }

    public class UnifyGeometry : UnifyObject
    {
        public override string ObjType { get; set; }
        //public string Layer { get; set; }
        public override string Guid { get; set; }
        //public string MatId { get; set; }
        public override string Diffuse { get; set; }
        //public string Emission { get; set; }
        //public string Specular { get; set; }
        //public string IOR { get; set; }
        //public string Reflectivity { get; set; }
        //public string Shine { get; set; }
        //public string Transparency { get; set; }
        //public string Renderer { get; set; }
        //public string MatName { get; set; }

        public UnifyGeometry(RhinoObject obj)
        {
            Material mat = obj.GetMaterial(false);

            this.ObjType = "GeometryObject";
            //this.Layer = RhinoDoc.ActiveDoc.Layers[obj.Attributes.LayerIndex].FullPath.Replace(":", "_");
            this.Guid = obj.Id.ToString();
            //this.MatId = mat.Id.ToString();
            this.Diffuse = mat.DiffuseColor.ToArgb().ToString();
            //this.Emission = mat.EmissionColor.ToArgb().ToString();
            //this.Specular = mat.EmissionColor.ToArgb().ToString();
            //this.IOR = mat.IndexOfRefraction.ToString();
            //this.Reflectivity = mat.Reflectivity.ToString();
            //this.Shine = mat.Shine.ToString();
            //this.Transparency = mat.Transparency.ToString();
            //this.Renderer = mat.RenderPlugInId.ToString();
            //this.MatName = mat.Name;
        }

    }

    public class InputData
    {
        public InputData()
        {

        }

        public void ExportOBJ(List<Guid> objs, string folderPath)
        {
            RhinoDoc.ActiveDoc.Objects.UnselectAll();
            RhinoDoc.ActiveDoc.Objects.Select(objs);

            string filePath = folderPath + "\\Item.obj";
            string script = string.Concat("_-Export ", filePath, " y=y", " _Enter _Enter");
            RhinoApp.RunScript(script, false);
            RhinoApp.RunScript("_-SelNone", true);
        }

        public bool WriteSetings(List<object> objs, string settingsPath)
        {
            string json = JsonConvert.SerializeObject(objs.ToArray(), Formatting.Indented);
            System.IO.File.WriteAllText(settingsPath, json);
            return true;
        }
    }
}
