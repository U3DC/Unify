using System;
using System.Collections.Generic;
using Rhino;
using Rhino.Commands;
using Rhino.Input;
using Rhino.DocObjects;
using Rhino.DocObjects.Tables;
using System.Windows.Forms;
using UnityUtilities;

// Unify: Leland Jobson

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
            get { return "UnityOutCommand"; }
        }

        public string folderPath = "";
        public string settingsPath = "";

        /* TO DO
            - Parse blocks into separate objs and lists of positions/orientations
            - Use Gh obj exporter
        */
        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // Get desired folder location
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            DialogResult result = fbd.ShowDialog();
            if (result == DialogResult.OK)
            {
                folderPath = fbd.SelectedPath;
                settingsPath = folderPath + "\\Settings.txt";
            }
            else
            {
                return Result.Cancel;
            }

            ObjRef[] Ground; // The landscape
            ObjRef[] Floors; // Create collider objects on specified floors

            // Terrain will be separately exported and be given editor options relational to context.
            RhinoGet.GetMultipleObjects("Select ground and terrain objects. Strike Enter if none.", true, ObjectType.AnyObject, out Ground);

            // Floors will be separately exoprted and set as hidden objects with mesh colliders.
            RhinoGet.GetMultipleObjects("Select occupiable floors. Strike Enter if none.", true, ObjectType.AnyObject, out Floors);

            ObjectTable allObjects = Rhino.RhinoDoc.ActiveDoc.Objects;
            NamedViewTable allCameras = Rhino.RhinoDoc.ActiveDoc.NamedViews;
            LightTable allLights = Rhino.RhinoDoc.ActiveDoc.Lights;

            List<object> writeOutList = new List<object>();
            List<Guid> objToExport = new List<Guid>();

            // get all geometry objects
            foreach (RhinoObject ro in allObjects)
            {
                writeOutList.Add(new UnifyGeometry(ro));
                objToExport.Add(ro.Id);
            }

            // get all named view objects
            foreach (ViewInfo vi in allCameras)
            {
                writeOutList.Add(new UnifyCamera(vi));
            }

            // get all lights
            foreach (LightObject lo in allLights)
            {
                writeOutList.Add(new UnifyLight(lo));
            }

            // export to OBJ
            InputData export = new InputData(); 
            export.ExportOBJ(objToExport, folderPath);

            // write the settings file
            bool success = export.WriteSetings(writeOutList, settingsPath);

            // Export the Bitmap Table
            var Bitmaps = RhinoDoc.ActiveDoc.Bitmaps.ExportToFiles(folderPath, 2);
            

            if (success)
            {
                return Result.Success;
            }
            return Result.Failure;     
        }
    }

}