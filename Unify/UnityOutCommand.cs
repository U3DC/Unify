using System;
using System.Collections.Generic;
using Rhino;
using Rhino.Commands;
using Rhino.Input;
using Rhino.DocObjects;
using Rhino.DocObjects.Tables;
using System.Windows.Forms;
using Unify.Utilities;

// Unify: Leland Jobson
// Unify: Konrad K Sobon
// Unify: David Mans

namespace Unify
{
    [System.Runtime.InteropServices.Guid("7cce0799-e273-49c2-ab0b-fe44c5ba3417"),
    Rhino.Commands.CommandStyle(Rhino.Commands.Style.ScriptRunner)]

    public class UnityOutCommand : Command
    {
        public UnityOutCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static UnityOutCommand Instance
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
                geoList.Add(new UnifyGeometry(ro));
                objToExport.Add(ro.Id);
            }
            writeOutList.Add(geoList);

            // get all named view objects
            NamedViewTable allCameras = Rhino.RhinoDoc.ActiveDoc.NamedViews;
            List<object> camList = new List<object>();
            foreach (ViewInfo vi in allCameras)
            {
                camList.Add(new UnifyCamera(vi));
            }
            writeOutList.Add(camList);

            // get all lights
            LightTable allLights = Rhino.RhinoDoc.ActiveDoc.Lights;
            List<object> lightList = new List<object>();
            foreach (LightObject lo in allLights)
            {
                UnifyLight unifyObj = new UnifyLight(lo);
                if (lo.IsDeleted)
                {
                    unifyObj.Deleted = true;
                }
                lightList.Add(unifyObj);
            }
            writeOutList.Add(lightList);

            // get all materials by layers
            LayerTable allLayers = Rhino.RhinoDoc.ActiveDoc.Layers;
            List<object> matList = new List<object>();
            foreach (Layer l in allLayers)
            {
                int renderMatIndex = l.RenderMaterialIndex;
                Material mat = Rhino.RhinoDoc.ActiveDoc.Materials[renderMatIndex];
                UnifyMaterial uMat = new UnifyMaterial(mat);
                string matUniqueName = l.FullPath.Replace("::", "__");
                uMat.UniqueName = matUniqueName;
                matList.Add(uMat);
            }
            writeOutList.Add(matList);

            // write out file info for unity importer to process
            List<object> allData = new List<object>();
            Dictionary<string, string> metaDic = new Dictionary<string, string>();
            metaDic.Add("FolderPath", folderPath);
            metaDic.Add("OBJName", System.IO.Path.GetFileNameWithoutExtension(RhinoDoc.ActiveDoc.Name) + ".obj");
            metaDic.Add("SettingsName", "UnifySettings.txt");
            UnifyMetaData metaData = new UnifyMetaData(metaDic);
            allData.Add(metaData);
            writeOutList.Add(allData);

            // export to OBJ
            Utility.ExportOBJ(objToExport);

            // write the settings file
            bool success = Utility.WriteSetings(writeOutList);

            if (success)
            {
                return Result.Success;
            }
            return Result.Failure;     
        }
    }

}