using Newtonsoft.Json;
using Rhino;
using System;
using System.Collections.Generic;
using System.Text;

namespace Unify.Utilities
{
    static class Utility
    {
        public static string ColorToString(System.Drawing.Color col)
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

        public static void ExportOBJ(List<Guid> objs)
        {
            RhinoDoc.ActiveDoc.Objects.UnselectAll();
            RhinoDoc.ActiveDoc.Objects.Select(objs);

            string objOptions = GetOBJOptions();
            string fileName = "\\" + System.IO.Path.GetFileNameWithoutExtension(RhinoDoc.ActiveDoc.Name) + ".obj ";
            string filePath = "C:\\Temp" + fileName;
            string script = string.Concat("_-Export ", filePath, objOptions, " y=y", " _Enter _Enter");
            RhinoApp.RunScript(script, false);
            RhinoApp.RunScript("_-SelNone", true);
        }

        public static bool ExportSettings(List<List<object>> objs)
        {
            string json = JsonConvert.SerializeObject(objs, Formatting.Indented);
            System.IO.File.WriteAllText("C:\\Temp\\" + "UnifySettings.txt", json);
            return true;
        }
    }
}
