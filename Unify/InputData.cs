using Rhino.DocObjects;
using Rhino.DocObjects.Tables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Unify.UnifyCommon;

namespace Unify
{
    public class InputData
    {
        public Rhino.RhinoDoc doc;
        public SortedDictionary<string, string> Projects = new SortedDictionary<string, string>();
        public string ProjectsFolderPath;
        public string PluginFolderPath;
        public string unityProjectPath;
        public List<UnifyCamera> Cameras;

        public InputData(Rhino.RhinoDoc _doc)
        {
            this.doc = _doc;

            GetProjects();
            GetCameras();
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
                cam.Name = vi.Name;

                camList.Add(cam);
            }

            this.Cameras = camList;
        }
    }
}
