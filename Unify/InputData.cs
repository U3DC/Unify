using Newtonsoft.Json;
using Rhino;
using Rhino.DocObjects;
using Rhino.DocObjects.Tables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Unify.UnifyCommon;
using Unify.Utilities;

namespace Unify
{
    public class InputData
    {
        public RhinoDoc doc;
        public SortedDictionary<string, string> Projects = new SortedDictionary<string, string>();
        public string ProjectsFolderPath;
        public string PluginFolderPath;
        public string UnityProjectPath;
        public List<Guid> ObjToExport;

        // assets to be exported
        public List<UnifyCamera> Cameras;
        public List<UnifyGeometry> Geometry;
        public List<UnifyLight> Lights;
        public List<UnifyMaterial> Materials;
        public List<UnifyMetaData> MetaData;
        public List<UnifyLayer> Layers;

        public InputData(RhinoDoc _doc)
        {
            this.doc = _doc;

            GetProjects();
            GetLights();
            GetGeometry();
            GetCameras();
            GetMaterials();
            GetMetaData();
            GetLayers();
        }

        public void ProcessExports()
        {
            // run OBJ Export
            doc.Objects.UnselectAll();
            doc.Objects.Select(this.ObjToExport);
            string objOptions = Utility.GetOBJOptions();
            string fileName = "\\" + Path.GetFileNameWithoutExtension(doc.Name) + ".obj";
            string filePath = "\"" + this.UnityProjectPath + "\\Resources\\Model" + fileName + "\" ";
            string script = string.Concat("_-Export ", filePath, objOptions, " y=y", " _Enter _Enter");
            RhinoApp.RunScript(script, true);
            RhinoApp.RunScript("_-SelNone", true);

            // run Unify settings export
            Dictionary<string, List<object>> exportObjects = new Dictionary<string, List<object>>()
            {
                { "Geometry", this.Geometry.Cast<object>().ToList() },
                { "Cameras", this.Cameras.Cast<object>().ToList() },
                { "Lights", this.Lights.Cast<object>().ToList() },
                { "Materials", this.Materials.Cast<object>().ToList() },
                { "MetaData", this.MetaData.Cast<object>().ToList() },
                { "Layers", this.Layers.Cast<object>().ToList() }
            };
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All
            };
            string json = JsonConvert.SerializeObject(exportObjects, Formatting.Indented, settings);
            File.WriteAllText(this.UnityProjectPath + @"\Resources\" + "UnifySettings.txt", json);
        }

        private void GetProjects()
        {
            // get rhino plug-in dll folder
            string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
            string localAssemblyFolder = new Uri(assemblyFolder).LocalPath;
            string projectsPath = Path.Combine(localAssemblyFolder, @"Projects\");
            string[] projects = Directory.GetFiles(projectsPath, "*.txt", SearchOption.TopDirectoryOnly);

            SortedDictionary<string, string> projectDict = new SortedDictionary<string, string>();
            foreach (string p in projects)
            {
                projectDict.Add(Path.GetFileNameWithoutExtension(p), p);
            }

            this.Projects = projectDict;
            this.ProjectsFolderPath = projectsPath;
            this.PluginFolderPath = localAssemblyFolder;
        }

        public void GetMetaData()
        {
            // write out file info for unity importer to process
            List<UnifyMetaData> allData = new List<UnifyMetaData>();
            Dictionary<string, string> metaDic = new Dictionary<string, string>();
            metaDic.Add("OBJName", Path.GetFileNameWithoutExtension(doc.Name) + ".obj");
            UnifyMetaData metaData = new UnifyMetaData(metaDic);
            allData.Add(metaData);

            this.MetaData = allData;
        }

        private void GetLayers()
        {
            List<UnifyLayer> unifyLayers = new List<UnifyLayer>();
            foreach (Layer l in doc.Layers)
            {
                UnifyLayer ul = new UnifyLayer();
                ul.ObjType = "Layer";
                ul.Guid = l.Id;
                ul.Name = l.FullPath.Replace(":", "_");
                ul.MeshCollider = false;
                unifyLayers.Add(ul);
            }

            this.Layers = unifyLayers;
        }

        private void GetMaterials()
        {
            // get all materials by layers
            LayerTable allLayers = doc.Layers;
            List<UnifyMaterial> matList = new List<UnifyMaterial>();
            foreach (Layer layer in allLayers)
            {
                int renderMatIndex = layer.RenderMaterialIndex;
                Material rhinoMat = doc.Materials[renderMatIndex];
                UnifyMaterial mat = new UnifyMaterial();

                mat.ObjType = "MaterialObject";
                mat.Guid = rhinoMat.Id;
                mat.Name = rhinoMat.Name;
                mat.Diffuse = Utility.ColorToString(rhinoMat.DiffuseColor);
                mat.SpecularColor = Utility.ColorToString(rhinoMat.SpecularColor);
                mat.EmissionColor = Utility.ColorToString(rhinoMat.EmissionColor);
                mat.ReflectionColor = Utility.ColorToString(rhinoMat.ReflectionColor);
                mat.Metallic = rhinoMat.Shine;

                if (rhinoMat.GetBitmapTexture() != null) mat.DiffuseTexture = rhinoMat.GetBitmapTexture().FileName;
                if (rhinoMat.GetTransparencyTexture() != null) mat.TransparencyTexture = rhinoMat.GetTransparencyTexture().FileName;
                if (rhinoMat.GetEnvironmentTexture() != null) mat.EnvironmentTexture = rhinoMat.GetEnvironmentTexture().FileName;
                if (rhinoMat.GetBumpTexture() != null) mat.BumpTexture = rhinoMat.GetBumpTexture().FileName;

                mat.Transparency = rhinoMat.Transparency;
                mat.UniqueName = layer.FullPath.Replace("::", "__");

                matList.Add(mat);
            }

            this.Materials = matList;
        }

        private void GetLights()
        {
            LightTable allLights = doc.Lights;
            List<UnifyLight> lightList = new List<UnifyLight>();
            foreach (LightObject light in allLights)
            {
                UnifyLight unifyObj = new UnifyLight();

                unifyObj.ObjType = "LightObject";
                unifyObj.LightType = light.LightGeometry.LightStyle.ToString();
                unifyObj.Guid = light.Id;
                unifyObj.Diffuse = Utility.ColorToString(light.LightGeometry.Diffuse);
                unifyObj.UniqueName = light.LightGeometry.LightStyle.ToString() + "-" + light.Id.ToString();
                unifyObj.Intensity = light.LightGeometry.Intensity;

                if (light.LightGeometry.LightStyle == Rhino.Geometry.LightStyle.WorldRectangular)
                {
                    // adjust location, rhino reports location at lower left corner
                    // unity reports location at center of the area/rectangle
                    Rhino.Geometry.Point3d loc = new Rhino.Geometry.Point3d(
                        light.LightGeometry.Location.X + (light.LightGeometry.Length.Length * 0.5),
                        light.LightGeometry.Location.Y + (light.LightGeometry.Width.Length * 0.5),
                        light.LightGeometry.Location.Z);
                    unifyObj.Location = loc.ToString();

                    // create target from location + direction
                    Rhino.Geometry.Transform xf = Rhino.Geometry.Transform.Translation(light.LightGeometry.Direction);
                    Rhino.Geometry.Point3d target = loc;
                    loc.Transform(xf);
                    unifyObj.Target = target.ToString();
                }
                else
                {
                    // create target from location + direction
                    Rhino.Geometry.Transform xf = Rhino.Geometry.Transform.Translation(light.LightGeometry.Direction);
                    Rhino.Geometry.Point3d target = light.LightGeometry.Location;
                    target.Transform(xf);
                    unifyObj.Target = target.ToString();

                    unifyObj.Location = light.LightGeometry.Location.ToString();
                }

                unifyObj.Range = light.LightGeometry.Direction.Length;
                unifyObj.SpotAngle = light.LightGeometry.SpotAngleRadians;
                unifyObj.ShadowIntensity = light.LightGeometry.SpotLightShadowIntensity;
                unifyObj.Width = light.LightGeometry.Width.Length;
                unifyObj.Length = light.LightGeometry.Length.Length;

                if (light.IsDeleted)
                {
                    unifyObj.Deleted = true;
                }

                lightList.Add(unifyObj);
            }

            this.Lights = lightList;
        }

        private void GetGeometry()
        {
            // get all geometry objects
            ObjectTable allObjects = doc.Objects;

            List<Guid> objExport = new List<Guid>();
            List<UnifyGeometry> geoList = new List<UnifyGeometry>();
            foreach (RhinoObject ro in allObjects)
            {
                UnifyGeometry geo = new UnifyGeometry();

                geo.ObjType = "GeometryObject";
                geo.Layer = doc.Layers[ro.Attributes.LayerIndex].FullPath.Replace(":", "_");
                geo.Guid = ro.Id;
                geo.MeshCollider = false;

                geoList.Add(geo);
                objExport.Add(ro.Id);
            }

            this.Geometry = geoList;
            this.ObjToExport = objExport;
        }

        private void GetCameras()
        {
            // get all named view objects
            NamedViewTable allCameras = doc.NamedViews;
            List<UnifyCamera> camList = new List<UnifyCamera>();
            foreach (ViewInfo vi in allCameras)
            {
                UnifyCamera cam = new UnifyCamera();
                cam.ObjType = "ViewCamera";
                cam.Guid = vi.Viewport.Id;
                cam.Location = vi.Viewport.CameraLocation.ToString();
                cam.Target = vi.Viewport.CameraDirection.ToString();
                cam.Name = vi.Name;

                camList.Add(cam);
            }

            this.Cameras = camList;
        }
    }
}
