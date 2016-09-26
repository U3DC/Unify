using Newtonsoft.Json;
using Rhino;
using Rhino.DocObjects;
using Rhino.DocObjects.Tables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Unify.UnifyCommon;
using Unify.Utilities;

namespace Unify
{
    public static class SOExtension
    {
        public static IEnumerable<TreeNode> FlattenTree(this TreeView tv)
        {
            return FlattenTree(tv.Nodes);
        }

        public static IEnumerable<TreeNode> FlattenTree(this TreeNodeCollection coll)
        {
            return coll.Cast<TreeNode>()
                        .Concat(coll.Cast<TreeNode>()
                                    .SelectMany(x => FlattenTree(x.Nodes)));
        }
    }

    public struct ValuePair
    {
        public bool Checked;
        public int Index;

        public ValuePair(bool x, int y)
        {
            Checked = x;
            Index = y;
        }
    }

    public class FormPresets
    {
        public string AssetsLocation { get; set; }
        public string OriginCamera { get; set; }
        public Dictionary<string, ValuePair> JumpCameras { get; set; }
        public Dictionary<string, bool> MeshColliders { get; set; }
        public Dictionary<string, bool> DesignOptions { get; set; }

        public FormPresets()
        {

        }
    }
    public class InputData
    {
        public RhinoDoc doc;
        public SortedDictionary<string, string> Projects = new SortedDictionary<string, string>();
        public string ProjectsFolderPath;
        public string PluginFolderPath;
        public string UnityProjectPath;
        public List<Guid> ObjToExport;
        public int NestingLevel;

        // assets to be exported
        public List<UnifyCamera> Cameras;
        public List<UnifyLight> Lights;
        public List<UnifyMaterial> Materials;
        public List<UnifyMetaData> MetaData;
        public List<UnifyLayer> Layers;
        public List<UnifyLayer> DesignOptions;

        public InputData(RhinoDoc _doc)
        {
            this.doc = _doc;

            GetProjects();
            GetLights();
            GetCameras();
            GetMaterialsAndLayers();
            GetMetaData();
            GetGeometry();
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
                { "DesignOptions", this.DesignOptions.Cast<object>().ToList() },
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

        private void GetGeometry()
        {
            List<Guid> objExport = new List<Guid>();
            foreach (RhinoObject ro in doc.Objects)
            {
                objExport.Add(ro.Id);
            }
            this.ObjToExport = objExport;
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

        private void GetMaterialsAndLayers()
        {
            List<UnifyMaterial> matList = new List<UnifyMaterial>();
            List<UnifyLayer> unifyLayers = new List<UnifyLayer>();
            this.NestingLevel = 0;

            // get all materials by layers
            foreach (Layer layer in doc.Layers)
            {
                // process Layer info
                UnifyLayer ul = new UnifyLayer();
                ul.ObjType = "Layer";
                ul.Guid = layer.Id;
                ul.Name = layer.FullPath.Replace(":", "_");
                ul.MeshCollider = false;
                ul.Parent = layer.ParentLayerId;
                ul.Level = layer.FullPath.Split(new string[] { "::" }, StringSplitOptions.None).Count();
                ul.ShortName = layer.Name;

                if (ul.Level > this.NestingLevel) this.NestingLevel = ul.Level;
                unifyLayers.Add(ul);

                // process material info
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
            this.Layers = unifyLayers;
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
                cam.CameraLocation = vi.Viewport.CameraLocation.ToString();
                cam.CameraTarget = vi.Viewport.FrustumCenterPoint(2).ToString();
                cam.Name = vi.Name;

                camList.Add(cam);
            }

            this.Cameras = camList;
        }
    }
}
