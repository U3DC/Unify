using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Unify.UnifyCommon;
using Unify.Utilities;

namespace Unify
{
    public partial class UnifyForm : Form
    {
        InputData inputData;
        string SelectedProject;
        List<UnifyLayer> checkedNodes = new List<UnifyLayer>();
        private Dictionary<string, bool> designOptions = new Dictionary<string, bool>();

        public UnifyForm(InputData inputData)
        {
            this.inputData = inputData;

            InitializeComponent();

            // populate projects dropdown
            if (this.inputData.Projects.Count > 0)
            {
                PopulateDropdownDictionary(cbProjects, this.inputData.Projects);

                // set selected value
                cbProjects.SelectedIndex = 0;
                this.SelectedProject = cbProjects.SelectedValue as string;
            }

            // populate Cameras Dropdown
            PopulateDropDownBinding(cbCameras, this.inputData.Cameras);

            // populate Checked Box List of Cameras
            PopulateCamerasCheckList(lbCameras, this.inputData.Cameras);

            btnExport.DialogResult = DialogResult.OK;
            btnCancel.DialogResult = DialogResult.Cancel;

            // populate MeshCollider Layers
            lbAllLayers.Items.AddRange(inputData.Layers.ToArray());
            lbAllLayers.DisplayMember = "Name";
            lbSelectedLayers.DisplayMember = "Name";
            lbAllLayers.Sorted = true;

            // populate tree view
            PopulateTreeView(this.inputData.Layers, this.inputData.NestingLevel);
            treeView1.ExpandAll();

            // refresh form
            this.Refresh();
            LoadPresets();
        }

        private void PopulateTreeView(List<UnifyLayer> data, int levels)
        {
            if (data.Count > 0)
            {
                List<UnifyLayer> topLevelNodes = data.Where(x => x.Level == 1).ToList();

                List<TreeNode> arrTreeNodes = new List<TreeNode>();
                foreach (UnifyLayer ul in topLevelNodes)
                {
                    TreeNode topNode = new TreeNode();
                    topNode.Name = ul.ShortName;
                    topNode.Text = ul.ShortName;
                    topNode.Tag = ul;
                    arrTreeNodes.Add(topNode);
                }
                treeView1.Nodes.AddRange(arrTreeNodes.ToArray<TreeNode>());

                TreeNode nd = null;
                for (int i = 2; i < levels + 1; i++)
                {
                    List<UnifyLayer> levelXNodes = data.Where(x => x.Level == i).ToList();

                    foreach (UnifyLayer ul in levelXNodes)
                    {
                        TreeNode node = new TreeNode();
                        node.Name = ul.ShortName;
                        node.Text = ul.ShortName;
                        node.Tag = ul;

                        nd = treeView1.FlattenTree()
                            .Where(x => ((UnifyLayer)x.Tag).Guid.Equals(ul.Parent))
                            .FirstOrDefault();

                        if (nd != null)
                        {
                            nd.Nodes.Add(node);
                        }

                    }
                }
            }
        }

        private void LoadPresets()
        {
            FormPresets presets = null;
            try
            {
                string json = File.ReadAllText(this.SelectedProject);
                presets = JsonConvert.DeserializeObject<FormPresets>(json);
            }
            catch { }

            if (presets != null)
            {
                if (presets.AssetsLocation != null)
                {
                    // set project path and input data
                    tbFolderPath.Text = presets.AssetsLocation;
                    this.inputData.UnityProjectPath = presets.AssetsLocation;
                }

                if (presets.OriginCamera != null)
                {
                    // set selected origin camera
                    int index = cbCameras.FindStringExact(presets.OriginCamera);
                    if (index != -1)
                    {
                        cbCameras.SelectedIndex = index;
                    }
                }

                if (presets.JumpCameras != null)
                {
                    // set jump cameras
                    // modify source list order
                    for (int i = 0; i < lbCameras.Items.Count; i++)
                    {
                        UnifyCamera cam = lbCameras.Items[i] as UnifyCamera;
                        if (presets.JumpCameras.ContainsKey(cam.Name))
                        {
                            this.inputData.Cameras.Remove(cam);
                            this.inputData.Cameras.Insert(presets.JumpCameras[cam.Name].Index, cam);
                        }
                    }

                // re-set the list box bounding to re-set the order.
                ((ListBox)lbCameras).DataSource = null;
                    ((ListBox)lbCameras).DataSource = this.inputData.Cameras;
                    ((ListBox)lbCameras).DisplayMember = "Name";

                    // set check boxes
                    foreach (KeyValuePair<string, ValuePair> item in presets.JumpCameras)
                    {
                        int index1 = lbCameras.FindStringExact(item.Key);
                        if (index1 != -1)
                        {
                            lbCameras.SetItemCheckState(index1, item.Value.Checked == true ? CheckState.Checked : CheckState.Unchecked);
                        }
                    }
                }

                if (presets.MeshColliders != null)
                {
                    // set nesh colliders
                    foreach (KeyValuePair<string, bool> item in presets.MeshColliders)
                    {
                        int index2 = lbAllLayers.FindStringExact(item.Key);
                        int index3 = lbSelectedLayers.FindStringExact(item.Key);
                        if (index2 != -1 && item.Value == true)
                        {
                            lbSelectedLayers.Items.Add(lbAllLayers.Items[index2]);
                            lbAllLayers.Items.RemoveAt(index2);
                        }
                        if (index3 != -1 && item.Value == false)
                        {
                            lbAllLayers.Items.Add(lbSelectedLayers.Items[index3]);
                            lbSelectedLayers.Items.RemoveAt(index3);
                        }
                    }
                }

                if (presets.DesignOptions != null)
                {
                    // set Design Options
                    TreeNode nd = null;
                    IEnumerable<TreeNode> treeViewFlat = treeView1.FlattenTree();
                    foreach (KeyValuePair<string, bool> item in presets.DesignOptions)
                    {
                        nd = treeViewFlat.Where(x => ((UnifyLayer)x.Tag).Name == item.Key).FirstOrDefault();
                        if (nd != null)
                        {
                            nd.Checked = item.Value;
                        }
                    }
                }
            }
        }

        private void SavePresets()
        {
            FormPresets presets = new FormPresets();

            // save assets location
            presets.AssetsLocation = tbFolderPath.Text;

            // save origin camera selection
            presets.OriginCamera = ((UnifyCamera)cbCameras.SelectedValue).Name;

            // save jump cameras
            Dictionary<string, ValuePair> jumpCameras = new Dictionary<string, ValuePair>();
            for (int i = 0; i < lbCameras.Items.Count; i++)
            {
                UnifyCamera camera = lbCameras.Items[i] as UnifyCamera;
                if (lbCameras.GetItemChecked(i))
                {
                    jumpCameras.Add(camera.Name, new ValuePair(true, i));
                }
                else
                {
                    jumpCameras.Add(camera.Name, new ValuePair(false, i));
                }

            }
            presets.JumpCameras = jumpCameras;

            // save mesh colliders
            Dictionary<string, bool> meshColliders = new Dictionary<string, bool>();
            for (int i = 0; i < lbAllLayers.Items.Count; i++)
            {
                UnifyLayer layer = lbAllLayers.Items[i] as UnifyLayer;
                meshColliders.Add(layer.Name, false);
            }
            for (int i = 0; i < lbSelectedLayers.Items.Count; i++)
            {
                UnifyLayer layer = lbSelectedLayers.Items[i] as UnifyLayer;
                meshColliders.Add(layer.Name, true);
            }
            presets.MeshColliders = meshColliders;

            // save design options
            this.designOptions = new Dictionary<string, bool>();
            SerializeTreeView(treeView1);
            presets.DesignOptions = this.designOptions;

            // write to project file
            string json = JsonConvert.SerializeObject(presets, Formatting.Indented);
            File.WriteAllText(this.SelectedProject, json);
        }

        private void StoreNodeRecursive(TreeNode treeNode)
        {
            designOptions.Add(((UnifyLayer)treeNode.Tag).Name, treeNode.Checked);
            foreach (TreeNode tn in treeNode.Nodes)
            {
                StoreNodeRecursive(tn);
            }
        }

        private void SerializeTreeView(TreeView treeView)
        {
            TreeNodeCollection nodes = treeView.Nodes;
            foreach (TreeNode n in nodes)
            {
                StoreNodeRecursive(n);
            }
        }

        private void PopulateDropdownDictionary<TKey, TValue>(ComboBox control, SortedDictionary<TKey, TValue> dict)
        {
            if (dict != null
                && dict.Count > 0)
            {
                control.DataSource = new BindingSource(dict, null);
                control.DisplayMember = "Key";
                control.ValueMember = "Value";
            }
            else
            {
                control.DataSource = null;
            }
        }

        private void PopulateDropDownBinding(ComboBox control, List<UnifyCamera> cameras)
        {
            BindingList<UnifyCamera> bindingList = new BindingList<UnifyCamera>();
            foreach (UnifyCamera cam in cameras)
            {
                bindingList.Add(cam);
            }
            control.DataSource = bindingList;
            control.DisplayMember = "Name";
        }

        private void PopulateCamerasCheckList(CheckedListBox control, List<UnifyCamera> cameras)
        {
            // leave ValueMember unspecified and by default it will be bound to UnifyCamera
            ((ListBox)control).DataSource = cameras;
            ((ListBox)control).DisplayMember = "Name";
        }

        private void cbProjects_SelectionChangeCommitted(object sender, System.EventArgs e)
        {
            string project = cbProjects.SelectedValue as string;
            this.SelectedProject = project;

            LoadPresets();
        }

        private void btnNewProject_Click(object sender, System.EventArgs e)
        {
            string projectName = Path.Combine(inputData.ProjectsFolderPath, tbProjectName.Text + ".txt");

            if (!this.inputData.Projects.ContainsKey(Path.GetFileNameWithoutExtension(projectName)))
            {
                // create new project txt file
                File.Create(projectName).Dispose();

                // set current project to new file and update input data
                this.SelectedProject = projectName;
                this.inputData.Projects.Add(Path.GetFileNameWithoutExtension(projectName), projectName);

                // refresh projects dropdown
                PopulateDropdownDictionary(cbProjects, this.inputData.Projects);
                cbProjects.Text = Path.GetFileNameWithoutExtension(projectName);

                // reset text in Text Box
                tbProjectName.Text = "Project Name";
            }
        }

        private void btnDeleteProject_Click(object sender, System.EventArgs e)
        {
            if (cbProjects.SelectedValue != null)
            {
                // delete selected project file
                string selectedPath = cbProjects.SelectedValue as string;
                File.Delete(selectedPath);

                // update input data
                this.inputData.Projects.Remove(Path.GetFileNameWithoutExtension(cbProjects.SelectedValue as string));

                if (this.inputData.Projects.Count > 0)
                {
                    // update projects dropdown
                    PopulateDropdownDictionary(cbProjects, this.inputData.Projects);
                    cbProjects.SelectedIndex = 0;
                    this.SelectedProject = cbProjects.SelectedValue as string;
                }
                else
                {
                    cbProjects.DataSource = null;
                    cbProjects.Text = "Projects";
                }

                // reload presets
                LoadPresets();
            }
        }

        private void btnFolderPath_Click(object sender, System.EventArgs e)
        {
            // get folder directory and store in inputData
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                this.inputData.UnityProjectPath = folderBrowserDialog1.SelectedPath;
                tbFolderPath.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void btnExport_Click(object sender, System.EventArgs e)
        {
            // deploy assets to specified Unity location
            if (this.inputData.UnityProjectPath != null)
            {
                Utility.CopyDir(this.inputData.PluginFolderPath + @"\Assets", this.inputData.UnityProjectPath);
            }

            // set origin camera property
            // jump cameras are data bound so property should be set already for those
            if (cbCameras.SelectedValue != null)
            {
                UnifyCamera camera = cbCameras.SelectedValue as UnifyCamera;
                camera.IsPlayerOriginCamera = true;
            }

            if (lbCameras.Items.Count > 0)
            {
                foreach (var item in lbCameras.CheckedItems)
                {
                    UnifyCamera cam = item as UnifyCamera;
                    cam.IsPlayerJumpCamera = true;
                }
            }

            // set MeshCollider property for Layers
            foreach (object o in lbSelectedLayers.Items)
            {
                UnifyLayer layer = o as UnifyLayer;
                layer.MeshCollider = true;
            }
            foreach (object o in lbAllLayers.Items)
            {
                UnifyLayer layer = o as UnifyLayer;
                layer.MeshCollider = false;
            }

            // set layers singled out for design options
            this.inputData.DesignOptions = this.checkedNodes;

            this.inputData.ProcessExports();

            // save form presets
            SavePresets();

            // close form
            this.Close();
        }

        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            // close form
            this.Close();
        }

        private void MoveListBoxItems(ListBox source, ListBox destination)
        {
            ListBox.SelectedObjectCollection sourceItems = source.SelectedItems;
            foreach (var item in sourceItems)
            {
                destination.Items.Add(item);
            }
            while (source.SelectedItems.Count > 0)
            {
                source.Items.Remove(source.SelectedItems[0]);
            }
        }

        private void btnAddLayer_Click(object sender, System.EventArgs e)
        {
            MoveListBoxItems(lbAllLayers, lbSelectedLayers);
        }

        private void btnRemoveLayer_Click(object sender, System.EventArgs e)
        {
            MoveListBoxItems(lbSelectedLayers, lbAllLayers);
        }

        private void lbSelectedLayers_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = lbSelectedLayers.IndexFromPoint(e.Location);
            if (index != ListBox.NoMatches)
            {
                var item = lbSelectedLayers.Items[index];
                lbAllLayers.Items.Add(item);
                lbSelectedLayers.Items.Remove(item);
            }
        }

        private void lbAllLayers_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = lbAllLayers.IndexFromPoint(e.Location);
            if (index != ListBox.NoMatches)
            {
                var item = lbAllLayers.Items[index];
                lbSelectedLayers.Items.Add(item);
                lbAllLayers.Items.Remove(item);
            }
        }

        public void MoveItem(int direction)
        {
            // Checking selected item
            if (lbCameras.SelectedItem == null || lbCameras.SelectedIndex < 0)
                return; // No selected item - nothing to do

            // Calculate new index using move direction
            int newIndex = lbCameras.SelectedIndex + direction;

            // Checking bounds of the range
            if (newIndex < 0 || newIndex >= lbCameras.Items.Count)
                return; // Index out of range - nothing to do

            UnifyCamera selected = lbCameras.SelectedItem as UnifyCamera;

            inputData.Cameras.RemoveAt(lbCameras.SelectedIndex);
            inputData.Cameras.Insert(newIndex, selected);

            ((ListBox)lbCameras).DataSource = null;
            ((ListBox)lbCameras).DataSource = this.inputData.Cameras;
            ((ListBox)lbCameras).DisplayMember = "Name";

            // Restore selection
            lbCameras.SetSelected(newIndex, true);
        }

        private void btnUp_Click(object sender, System.EventArgs e)
        {
            MoveItem(-1);
        }

        private void btnDown_Click(object sender, System.EventArgs e)
        {
            MoveItem(1);
        }

        private void cbCameras_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            UnifyCamera selected = cbCameras.SelectedValue as UnifyCamera;
        }

        private void btnPresetRefresh_Click(object sender, System.EventArgs e)
        {
            LoadPresets();
        }

        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            // check all child nodes as well
            if (e.Action != TreeViewAction.Unknown)
            {
                UnifyLayer node = (UnifyLayer)e.Node.Tag;

                if (e.Node.Nodes.Count > 0)
                {
                    this.CheckAllChildNodes(e.Node, e.Node.Checked, node.Name);
                }
                if (e.Node.Checked)
                {
                    node.DesignOptionName = node.Name;
                    this.checkedNodes.Add(node);
                }
                else
                {
                    node.DesignOptionName = "";
                    this.checkedNodes.Remove(node);
                }
            }
        }

        private void CheckAllChildNodes(TreeNode treeNode, bool nodeChecked, string parentName)
        {
            foreach (TreeNode node in treeNode.Nodes)
            {
                node.Checked = nodeChecked;
                UnifyLayer layer = (UnifyLayer)node.Tag;

                if (nodeChecked)
                {
                    layer.DesignOptionName = parentName;
                    checkedNodes.Add(layer);
                }
                else
                {
                    layer.DesignOptionName = "";
                    checkedNodes.Remove(layer);
                }
                if (node.Nodes.Count > 0)
                {
                    this.CheckAllChildNodes(node, nodeChecked, parentName);
                }
            }
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/ksobon/Unify");
        }
    }
}
