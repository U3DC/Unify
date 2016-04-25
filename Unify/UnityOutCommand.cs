using System;
using System.Collections.Generic;
using Rhino;
using Rhino.Display;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using Rhino.DocObjects;
using Rhino.DocObjects.Tables;
using System.Drawing;
using System.Text;
using System.IO;
using System.Linq;
using Rhino.PlugIns;

// Unify: Leland Jobson

namespace MyProject3
{
    [System.Runtime.InteropServices.Guid("7cce0799-e273-49c2-ab0b-fe44c5ba3417"),
        Rhino.Commands.CommandStyle(Rhino.Commands.Style.ScriptRunner)] // Added Scriptrunner prevents Rhino command collision f-ups.

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

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {



            /* TO DO
               - Replace paths with AppData path
               - Parse blocks into separate objs and lists of positions/orientations
               - Use Gh obj exporter

            */



            bool UseActive = true;

            ObjRef[] Ground; // The landscape
            ObjRef[] Floors; // Create collider objects on specified floors
            ObjRef[] Lights; // Use to create Unity lights

            // Get user Input

            Rhino.Input.RhinoGet.GetBool("Set your view to your desired virtual position. Hit Enter when ready", false, "Cancel", "Ready", ref UseActive);
            RhinoViewport ActiveViewport = doc.Views.ActiveView.ActiveViewport;
            Rhino.Input.RhinoGet.GetMultipleObjects("Select ground and terrain objects. Strike Enter if none.", true, ObjectType.AnyObject, out Ground); // Terrain will be separately exported and be given editor options relational to context.
            Rhino.Input.RhinoGet.GetMultipleObjects("Select occupiable floors. Strike Enter if none.", true, ObjectType.AnyObject, out Floors); // Floors will be separately exoprted and set as hidden objects with mesh colliders.
            Rhino.Input.RhinoGet.GetMultipleObjects("Select light objects. Strike Enter if none.", true, ObjectType.Light, out Lights); // Light objects will be exported separately. On the unity side, their centroids will be extracted and used to create new point light objects.

            // Create the directory where document information will be stored.

            Directory.CreateDirectory(@"C:\Users\LJobson\Downloads\Unity_Research\Solar\Assets\Models");
            string settingspath = @"C:\Users\LJobson\Downloads\Unity_Research\Solar\Assets\Models\Settings.txt";


            // Grab all camera information from the viewport.

            double ViewLens = ActiveViewport.Camera35mmLensLength;
            var ViewH = ActiveViewport.Bounds.Height;
            var ViewW = ActiveViewport.Bounds.Width;
            var ViewLoc = ActiveViewport.CameraLocation;
            var ViewTar = ActiveViewport.CameraTarget;
            var IsOrtho = ActiveViewport.IsParallelProjection;

            // Define our lists of info we wish to collect

            List<Guid> ObjIDs = new List<Guid>();
            List<int> MatIDs = new List<int>();
            List<Color> Ambient = new List<Color>();
            List<Color> Diffuse = new List<Color>();
            List<Color> Emission = new List<Color>();
            List<Color> Specular = new List<Color>();
            List<double> IOR = new List<double>();
            List<double> Reflectivity = new List<double>();
            List<double> Shine = new List<double>();
            List<double> Transparency = new List<double>();
            List<Guid> Renderer = new List<Guid>();
            List<string> Name = new List<string>();
            List<string> ObjGuids = new List<string>();
            List<string> Bit = new List<string>();
            List<string> Bump = new List<string>();
            List<string> Enviro = new List<string>();

            // Clear Lists

            ObjIDs.Clear();
            MatIDs.Clear();
            Ambient.Clear();
            Diffuse.Clear();
            Emission.Clear();
            Specular.Clear();
            IOR.Clear();
            Reflectivity.Clear();
            Shine.Clear();
            Transparency.Clear();
            Renderer.Clear();
            Name.Clear();
            Bit.Clear();
            Bump.Clear();
            Enviro.Clear();

            // Grab blocks and non blocks

            List<RhinoObject> blockObjects = new List<RhinoObject>();
            List<RhinoObject> nonblockObjects = new List<RhinoObject>();

            ObjectTable allObjects = Rhino.RhinoDoc.ActiveDoc.Objects;
            foreach (RhinoObject anObject in allObjects)
            {
                if (anObject.IsReference == true)
                {
                    nonblockObjects.Add(anObject);
                } else
                {
                    blockObjects.Add(anObject);
                }
            }



            // Match up order of current settings file in case of update. New items will be added to the end, old items will keep their spots, and deleted items will be converted to null.

            if (File.Exists(settingspath))
            {
                MatchSettings(nonblockObjects, settingspath);
            }


            // Iterator for appending geometry attributes

            int count = 0;

            foreach (RhinoObject geoitem in nonblockObjects)
            {
                // Grab the material attached to the current item

                Material itemmaterial = geoitem.GetMaterial(false);

                // Not sure if getting the material Id is worth it, but collecting it anyway.

                var MattableID = MaterialCheck(itemmaterial);

                // Apppend found information to our lists


                ObjGuids.Add(geoitem.Id.ToString());
                MatIDs.Add(MattableID);
                Ambient.Add(itemmaterial.AmbientColor);
                Diffuse.Add(itemmaterial.DiffuseColor);
                Emission.Add(itemmaterial.EmissionColor);
                Specular.Add(itemmaterial.SpecularColor);
                IOR.Add(itemmaterial.IndexOfRefraction);
                Reflectivity.Add(itemmaterial.Reflectivity);
                Shine.Add(itemmaterial.Shine);
                Transparency.Add(itemmaterial.Transparency);
                Renderer.Add(itemmaterial.RenderPlugInId);
                Name.Add(itemmaterial.Name);

                // Using Runscript to export objects

                var ItemGuid = geoitem.Id;
                var ItemRef = new ObjRef(ItemGuid);

                var PluginList = PlugIn.GetInstalledPlugIns();
                var asString = string.Join(";", PluginList);
                RhinoApp.WriteLine(asString, EnglishName);
                string filePath = @"C:\Users\LJobson\Downloads\Unity_Research\Solar\Assets\Models\Item";
                string extension = ".obj";
                string script = string.Concat("_-Export ", filePath, count.ToString(), extension, " y=y", " _Enter _Enter");
                RhinoDoc.ActiveDoc.Objects.Select(ItemRef);
                RhinoApp.WriteLine(script, EnglishName); // Debugger
                RhinoApp.RunScript(script, false);
                // "-Export " + FILE_NAME + ".obj y=y Enter Enter"
                RhinoApp.WriteLine(count.ToString(), EnglishName); // Debugger
                RhinoApp.RunScript("_-SelNone", true);
                count += 1;

            }


            // Write the settings file:

            bool SuccessBool = WriteOut(ObjGuids, MatIDs, Ambient, Diffuse, Emission, Specular, IOR, Reflectivity, Shine, Transparency, Renderer, Name, Bit, Bump, Enviro, ViewH, ViewW, ViewLoc, ViewTar, ViewLens);

            // Export the Bitmap Table

            var Bitmaps = RhinoDoc.ActiveDoc.Bitmaps.ExportToFiles(@"C:\Users\LJobson\Downloads\Unity_Research\Solar\Assets\Models\", 2);
            
            // Return some beeps


            if (SuccessBool == true)
            {
                Console.Beep();
            }
            else
            {
                Console.Beep();
                Console.Beep();
                Console.Beep();
            }

            // OBJ Export Tests

            Guid objExporter = Guid.Parse("45758256-e154-4995-bd0b-d5173a33c281");
            if (objExporter != null)
            {
                RhinoApp.WriteLine(objExporter.ToString(), EnglishName);
                var OBJExporter = PlugIn.Find(objExporter);
                var OBJModules = OBJExporter.Assembly.FullName;
                object Sets = OBJExporter.Assembly;

                var Modulename = string.Join(",", OBJModules.ToString());
                RhinoApp.WriteLine("TBD", EnglishName);
            }
            else
            {
                RhinoApp.WriteLine("Guid failed to collect", EnglishName);
            }

                           
            return Result.Success; // End the command
        }


        protected int MaterialCheck(Material MaterialToChk)
        {
            Guid Id = MaterialToChk.Id;
            var MatTableID = RhinoDoc.ActiveDoc.Materials.Find(Id, false);       

            // Note - If no material, will return value -1.

            return MatTableID;
        }

        protected bool WriteOut(List<string> ObjGuids, List<int> MatIDs, List<Color> Ambient, List<Color> Diffuse, List<Color> Emission, List<Color> Specular, List<double> IOR, List<double> Reflectivity, List<double> Shine, List<double> Transparency, List<Guid> Renderer, List<string> Name, List<string> Bit, List<string> Bump, List<string> Enviro, double ViewH, double ViewW, Point3d ViewLoc, Point3d ViewTar, double ViewLens)
        {
            // Make a debug checklist that will be pushed into a second file.


            List<string> DebugChecklist = new List<string>();

            //  OBJ GUIDS

            string GUIDS = string.Join("!", ObjGuids.ToArray());

            if (GUIDS != null)
            {
                DebugChecklist.Add("Guids = GOOD");
            } else {
                DebugChecklist.Add("Guids = ERROR");
            }

            //  MAT IDS  need to convert all members into strings before I can use the above snippet.
            
            List<string> StrMatIds = new List<string>();

            foreach (int ID in MatIDs)
            {
                string idString = ID.ToString();
                StrMatIds.Add(idString);
            }

            string MATIDS = string.Join("!", StrMatIds.ToArray());

            if (MATIDS != null)
            {
                DebugChecklist.Add("Mat IDS = GOOD");
            }
            else
            {
                DebugChecklist.Add("Mat IDS = ERROR");
            }

            // AMBIENT need to see how color formats to string.

            List<string> AmbientStr = new List<string>();

            foreach (Color col in Ambient)
            {
                string ColorString = col.ToString();
                AmbientStr.Add(ColorString);
            }

            /////////////////DEBUG///////////////////////////

            RhinoApp.WriteLine(AmbientStr.Count.ToString(), EnglishName);

            string AMBIENT = string.Join("!", AmbientStr.ToArray());

            if (AMBIENT != null)
            {
                DebugChecklist.Add("Ambient = GOOD");
            }
            else
            {
                DebugChecklist.Add("Ambient= ERROR");
            }


            // DIFFUSE need to see how color formats to string.

            List<string> DiffuseStr = new List<string>();

            foreach (Color col in Diffuse)
            {
                string ColorString = col.ToString();
                DiffuseStr.Add(ColorString);
            }

            string DIFFUSE = string.Join("!", DiffuseStr.ToArray());

            if (DIFFUSE != null)
            {
                DebugChecklist.Add("Diffuse = GOOD");
            }
            else
            {
                DebugChecklist.Add("Diffuse = ERROR");
            }


            // EMISSION same deal. 

            List<string> EmissStr = new List<string>();

            foreach (Color col in Emission)
            {
                string ColorString = col.ToString();
                EmissStr.Add(ColorString);
            }

            string EMISSION = string.Join("!", EmissStr.ToArray());

            if (EMISSION != null)
            {
                DebugChecklist.Add("Emission = GOOD");
            }
            else
            {
                DebugChecklist.Add("Emission = ERROR");
            }

            // SPECULAR

            List<string> SpecStr = new List<string>();

            foreach (Color col in Specular)
            {
                string ColorString = col.ToString();
                SpecStr.Add(ColorString);
            }

            string SPECULAR = string.Join("!", SpecStr.ToArray());

            if (SPECULAR != null)
            {
                DebugChecklist.Add("Specular = GOOD");
            }
            else
            {
                DebugChecklist.Add("Specular = ERROR");
            }

            // IOR - Double, same idea as Int

            List<string> IORStr = new List<string>();

            foreach (double IORitem in IOR)
            {
                string IORiteamStr = IORitem.ToString();
                IORStr.Add(IORiteamStr);
            }

            string INDEXOFREF = string.Join("!", IORStr.ToArray());

            if (INDEXOFREF != null)
            {
                DebugChecklist.Add("IOR = GOOD");
            }
            else
            {
                DebugChecklist.Add("IOR = ERROR");
            }

            // REFLECTIVITY - Double

            List<string> RefStr = new List<string>();

            foreach (double RefItem in Reflectivity)
            {
                string RefItemStr = RefItem.ToString();
                RefStr.Add(RefItemStr);
            }

            string REFLECTIVITY = string.Join("!", RefStr.ToArray());

            if (REFLECTIVITY != null)
            {
                DebugChecklist.Add("Reflectivity = GOOD");
            }
            else
            {
                DebugChecklist.Add("Reflectivity = ERROR");
            }

            // SHINE - Double

            List<string> ShineStr = new List<string>();

            foreach (double ShineItem in Shine)
            {
                string ShineItemStr = ShineItem.ToString();
                ShineStr.Add(ShineItemStr);
            }

            string SHINE = string.Join("!", ShineStr.ToArray());

            if (SHINE != null)
            {
                DebugChecklist.Add("Shine = GOOD");
            }
            else
            {
                DebugChecklist.Add("Shine = ERROR");
            }

            // TRANSPARENCY - Double

            List<string> TransStr = new List<string>();

            foreach (double TransItem in Transparency)
            {
                string TransStrItem = TransItem.ToString();
                TransStr.Add(TransStrItem);
            }

            string TRANSPARENCY = string.Join("!", TransStr.ToArray());

            if (TRANSPARENCY != null)
            {
                DebugChecklist.Add("Transparency = GOOD");
            }
            else
            {
                DebugChecklist.Add("Transparency = ERROR");
            }

            // RENDERER - Guid

            List<string> RenderStr = new List<string>();

            foreach (Guid RendItem in Renderer)
            {
                string rendstr = RendItem.ToString();
                RenderStr.Add(rendstr);
            }

            string RENDERER = string.Join("!", RenderStr.ToArray());

            if (RENDERER != null)
            {
                DebugChecklist.Add("Renderer = GOOD");
            }
            else
            {
                DebugChecklist.Add("Renderer = ERROR");
            }

            // NAME - String

            string NAMES = string.Join("!", Name.ToArray());

            if (NAMES != null)
            {
                DebugChecklist.Add("Names = GOOD");
            }
            else
            {
                DebugChecklist.Add("Names = ERROR");
            }

            // BIT - String of path

            string BITS = string.Join("!", Bit.ToArray());

            if (BITS != null)
            {
                DebugChecklist.Add("Bitmaps = GOOD");
            }
            else
            {
                DebugChecklist.Add("Bitmaps = ERROR");
            }

            // BUMP - String of path

            string BUMP = string.Join("!", Bump.ToArray());

            if (BUMP != null)
            {
                DebugChecklist.Add("Bump = GOOD");
            }
            else
            {
                DebugChecklist.Add("Bump = ERROR");
            }

            // ENVIRO - String of path

            string ENVIRO = string.Join("!", Enviro.ToArray());

            if (ENVIRO != null)
            {
                DebugChecklist.Add("Enviroment Map = GOOD");
            }
            else
            {
                DebugChecklist.Add("Environment Map = ERROR");
            }

            // VIEWH - Int
            // ViewW - Int
            // ViewLoc - Point3d

            string VIEWLOC = ViewLoc.ToString();

            // ViewTar - Point3d

            string VIEWTAR = ViewTar.ToString();

            // ViewLens - Double

            //------------------------------------------------------------------------------------------------------//

            // RETURN DEBUG IN CONSOLE

            bool Success = true;

            string ChecklistFlat = string.Join(" | ", DebugChecklist.ToArray());

            RhinoApp.WriteLine(ChecklistFlat, EnglishName);


            // WRITE FILE //

            // Create the Unify Folder on C
            //string TodayStr = System.DateTime.Now.ToString();  // Will work on creating a custom path based on filename..
            //string CurrentDocName = RhinoDoc.ActiveDoc.DocumentId.ToString();
            //string PathHeader = "C:\\Unify";
            //string FileType = ".txt";

            //string DocPath = RhinoDoc.ActiveDoc.Path.ToString();
            //var NewPath = Path.Combine(DocPath, "Unify");

            string filename = @"C:\Users\LJobson\Downloads\Unity_Research\Solar\Assets\Models\Settings.txt"; // Need to connect this back to the original public statement.
            File.Delete(@"C:\Users\LJobson\Downloads\Unity_Research\Solar\Assets\Models\Settings.txt"); // Clear for next file
            FileStream fs = null;

            try
            {
                fs = new FileStream(filename, FileMode.CreateNew);
                using (StreamWriter writer = new StreamWriter(fs, Encoding.Unicode))
                {
                    writer.Write(GUIDS + Environment.NewLine + MATIDS + Environment.NewLine + AMBIENT + Environment.NewLine + AMBIENT + Environment.NewLine + DIFFUSE + Environment.NewLine + EMISSION + Environment.NewLine + SPECULAR + Environment.NewLine + INDEXOFREF + Environment.NewLine + REFLECTIVITY + Environment.NewLine + SHINE + Environment.NewLine + RENDERER + Environment.NewLine + NAMES + Environment.NewLine + BITS + Environment.NewLine + BUMP + Environment.NewLine + ENVIRO + Environment.NewLine + ViewH + Environment.NewLine + ViewW + Environment.NewLine + VIEWLOC + Environment.NewLine + VIEWTAR + Environment.NewLine + ViewLens); // I will add the contents to be sent to the streamwriter file within here.
                }
            }
            finally
            {
                if (fs != null)
                    fs.Dispose();
            }

            return Success;
        }

        protected void MatchSettings(List<RhinoObject> nonblockObjects,string settingspath)
        {
            List<int> newHashes = new List<int>();
            List<int> oldHashes = new List<int>();

            newHashes.Clear();
            oldHashes.Clear();
                 
            foreach (var GeoItem in nonblockObjects)
            {
                var ItemHash = GeoItem.Id.GetHashCode();
                newHashes.Add(ItemHash);

            }

            string Readline;
            string[] Entries;

            try
            {
                using (StreamReader theReader = new StreamReader(settingspath)) 
                {
                    do
                    {
                        Readline = theReader.ReadLine();

                        if (Readline != null)
                        {
                            Entries = Readline.Split(char.Parse("!"));
                        }
                    }
                    while (Readline != null);

                    theReader.Close();
                }
            }
            catch (System.Exception e)
            {
                RhinoApp.WriteLine(e.Message, EnglishName);
                RhinoApp.WriteLine("Error - Settings file Could not be parsed.", EnglishName);
            } 
        }
    }

}