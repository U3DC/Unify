using System;
using System.Collections.Generic;
using Rhino;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.DocObjects.Tables;
using Unify.Utilities;
using Unify.UnifyCommon;

// Unify: Leland Jobson
// Unify: Konrad K Sobon
// Unify: David Mans

namespace Unify
{
    [System.Runtime.InteropServices.Guid("7cce0799-e273-49c2-ab0b-fe44c5ba3417"),
    Rhino.Commands.CommandStyle(Rhino.Commands.Style.ScriptRunner)]

    public class UnifyCommand : Command
    {
        public UnifyCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static UnifyCommand Instance
        {
            get; private set;
        }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "UnifyExport"; }
        }

        public string folderPath = "";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            //// Get desired folder location
            //FolderBrowserDialog fbd = new FolderBrowserDialog();
            //DialogResult result = fbd.ShowDialog();
            //if (result == DialogResult.OK)
            //{
            //    folderPath = fbd.SelectedPath;
            //}
            //else
            //{
            //    return Result.Cancel;
            //}

            //ObjRef[] Ground; // The landscape
            //ObjRef[] Floors; // Create collider objects on specified floors

            //// Terrain will be separately exported and be given editor options relational to context.
            //RhinoGet.GetMultipleObjects("Select ground and terrain objects. Strike Enter if none.", true, ObjectType.AnyObject, out Ground);

            //// Floors will be separately exoprted and set as hidden objects with mesh colliders.
            //RhinoGet.GetMultipleObjects("Select occupiable floors. Strike Enter if none.", true, ObjectType.AnyObject, out Floors);

            List<List<object>> writeOutList = new List<List<object>>();
            List<Guid> objToExport = new List<Guid>();

            // get all geometry objects
            ObjectTable allObjects = Rhino.RhinoDoc.ActiveDoc.Objects;
            List<object> geoList = new List<object>();
            foreach (RhinoObject ro in allObjects)
            {
                UnifyGeometry geo = new UnifyGeometry();

                geo.ObjType = "GeometryObject";
                geo.Layer = RhinoDoc.ActiveDoc.Layers[ro.Attributes.LayerIndex].FullPath.Replace(":", "_");
                geo.Guid = ro.Id;

                geoList.Add(geo);
                objToExport.Add(ro.Id);
            }
            writeOutList.Add(geoList);

            // get all named view objects
            NamedViewTable allCameras = Rhino.RhinoDoc.ActiveDoc.NamedViews;
            List<object> camList = new List<object>();
            foreach (ViewInfo vi in allCameras)
            {
                UnifyCamera cam = new UnifyCamera();

                cam.ObjType = "ViewCamera";
                cam.Guid = vi.Viewport.Id;
                cam.Location = vi.Viewport.CameraLocation.ToString();

                camList.Add(cam);
            }
            writeOutList.Add(camList);

            // get all lights
            LightTable allLights = Rhino.RhinoDoc.ActiveDoc.Lights;
            List<object> lightList = new List<object>();
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

                    // target target from location + direction
                    Rhino.Geometry.Transform xf = Rhino.Geometry.Transform.Translation(light.LightGeometry.Direction);
                    Rhino.Geometry.Point3d target = loc;
                    loc.Transform(xf);
                    unifyObj.Target = target.ToString();
                }
                else
                {
                    // target target from location + direction
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
            writeOutList.Add(lightList);

            // get all materials by layers
            LayerTable allLayers = Rhino.RhinoDoc.ActiveDoc.Layers;
            List<object> matList = new List<object>();
            foreach (Layer layer in allLayers)
            {
                int renderMatIndex = layer.RenderMaterialIndex;
                Material rhinoMat = Rhino.RhinoDoc.ActiveDoc.Materials[renderMatIndex];
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
            writeOutList.Add(matList);

            // write out file info for unity importer to process
            List<object> allData = new List<object>();
            Dictionary<string, string> metaDic = new Dictionary<string, string>();
            metaDic.Add("OBJName", System.IO.Path.GetFileNameWithoutExtension(RhinoDoc.ActiveDoc.Name) + ".obj");
            UnifyMetaData metaData = new UnifyMetaData(metaDic);
            allData.Add(metaData);
            writeOutList.Add(allData);

            // export to OBJ
            Utility.ExportOBJ(objToExport);

            // write the settings file
            bool success = Utility.ExportSettings(writeOutList);

            if (success)
            {
                return Result.Success;
            }
            return Result.Failure;     
        }
    }

}