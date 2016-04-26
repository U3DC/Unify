using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.DocObjects;
using Rhino;
using Newtonsoft.Json;

namespace Unify.Utilities
{
    public class UnifyLight
    {
        public string ObjType { get; set; }
        public string Guid { get; set; }
        public string Ambient { get; set; }
        public string Diffuse { get; set; }
        public string Direction { get; set; }
        public string Intensity { get; set; }
        public string Location { get; set; }
        public string Specular { get; set; }
        public string PowerCandela { get; set; }
        public string PowerLumens { get; set; }
        public string powerWatts { get; set; }

        public UnifyLight(LightObject light)
        {
            this.ObjType = "Light";
            this.Guid = light.Id.ToString();
            this.Ambient = light.LightGeometry.Ambient.ToString();
            this.Diffuse = light.LightGeometry.Diffuse.ToString();
            this.Direction = light.LightGeometry.Direction.ToString();
            this.Intensity = light.LightGeometry.Intensity.ToString();
            this.Location = light.LightGeometry.Location.ToString();
            this.Specular = light.LightGeometry.Specular.ToString();
            this.PowerCandela = light.LightGeometry.PowerCandela.ToString();
            this.PowerLumens = light.LightGeometry.PowerLumens.ToString();
            this.powerWatts = light.LightGeometry.PowerWatts.ToString();
        }
    }
    public class UnifyCamera
    {
        public string ObjType { get; set; }
        public string Guid { get; set; }
        public string Name { get; set; }
        public string LensLength { get; set; }
        public string Location { get; set; }
        public string Target { get; set; }
        public string ClippingPlaneNear { get; set; }
        public string ClippingPlaneFar { get; set; }

        public UnifyCamera(ViewInfo cam)
        {
            this.ObjType = "ViewCamera";
            this.Guid = cam.Viewport.Id.ToString();
            this.Name = cam.Name;
            this.LensLength = cam.Viewport.Camera35mmLensLength.ToString();
            this.Location = cam.Viewport.CameraLocation.ToString();
            this.Target = cam.Viewport.TargetPoint.ToString();
            this.ClippingPlaneNear = cam.Viewport.FrustumNear.ToString();
            this.ClippingPlaneFar = cam.Viewport.FrustumFar.ToString();
        }
    }

    public class UnifyObj
    {
        public string ObjType { get; set; }
        public string Layer { get; set; }
        public string Guid { get; set; }
        public string MatId { get; set; }
        public string Ambient { get; set; }
        public string Diffuse { get; set; }
        public string Emission { get; set; }
        public string Specular { get; set; }
        public string IOR { get; set; }
        public string Reflectivity { get; set; }
        public string Shine { get; set; }
        public string Transparency { get; set; }
        public string Renderer { get; set; }
        public string MatName { get; set; }

        public UnifyObj(RhinoObject obj)
        {
            Material mat = obj.GetMaterial(false);

            this.ObjType = "GeometryObject";
            this.Layer = RhinoDoc.ActiveDoc.Layers[obj.Attributes.LayerIndex].FullPath.Replace(":", "_");
            this.Guid = obj.Id.ToString();
            this.MatId = mat.Id.ToString();
            this.Ambient = mat.AmbientColor.ToArgb().ToString();
            this.Diffuse = mat.DiffuseColor.ToArgb().ToString();
            this.Emission = mat.EmissionColor.ToArgb().ToString();
            this.Specular = mat.EmissionColor.ToArgb().ToString();
            this.IOR = mat.IndexOfRefraction.ToString();
            this.Reflectivity = mat.Reflectivity.ToString();
            this.Shine = mat.Shine.ToString();
            this.Transparency = mat.Transparency.ToString();
            this.Renderer = mat.RenderPlugInId.ToString();
            this.MatName = mat.Name;
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