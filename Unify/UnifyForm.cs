using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using Unify.UnifyCommon;
using Unify.Utilities;

namespace Unify
{
    public partial class UnifyForm : Form
    {
        InputData inputData;
        string SelectedProject;

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

                // activate other controls
                gbCharacterLocation.Enabled = true;
                gbProjectLocation.Enabled = true;
            }

            // populate Cameras Dropdown
            PopulateDropDownBinding(cbCameras, this.inputData.Cameras);

            // populate Checked Box List of Cameras
            PopulateCamerasCheckList(lbCameras, this.inputData.Cameras);
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
            ((ListBox)control).DataSource = cameras;
            ((ListBox)control).DisplayMember = "Name";
            ((ListBox)control).ValueMember = "IsPlayerJumpCamera";

            for (int i = 0; i < control.Items.Count; i++)
            {
                UnifyCamera cam = (UnifyCamera)control.Items[i];
                control.SetItemChecked(i, cam.IsPlayerJumpCamera);
            }
        }

        private void cbProjects_SelectionChangeCommitted(object sender, System.EventArgs e)
        {
            string project = cbProjects.SelectedValue as string;
            this.SelectedProject = project;

            // activate other controls
            gbProjectLocation.Enabled = true;
            gbCharacterLocation.Enabled = true;
        }

        private void btnNewProject_Click(object sender, System.EventArgs e)
        {
            string projectName = Path.Combine(inputData.ProjectsFolderPath, tbProjectName.Text + ".txt");

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

        private void btnDeleteProject_Click(object sender, System.EventArgs e)
        {
            if (this.SelectedProject != null)
            {
                // delete selected project file
                string selectedPath = cbProjects.SelectedValue as string;
                File.Delete(selectedPath);

                // update input data
                this.inputData.Projects.Remove(Path.GetFileNameWithoutExtension(cbProjects.SelectedValue as string));

                // update projects dropdown
                PopulateDropdownDictionary(cbProjects, this.inputData.Projects);
                cbProjects.SelectedIndex = 0;
                this.SelectedProject = cbProjects.SelectedValue as string;
            }
        }

        private void btnFolderPath_Click(object sender, System.EventArgs e)
        {
            // get folder directory and store in inputData
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                this.inputData.unityProjectPath = folderBrowserDialog1.SelectedPath;
                tbFolderPath.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void btnExport_Click(object sender, System.EventArgs e)
        {
            // deploy assets to specified Unity location
            if (this.inputData.unityProjectPath != null)
            {
                Utility.CopyDir(this.inputData.PluginFolderPath + @"\Assets", this.inputData.unityProjectPath);
            }

            this.Close();
        }

        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }
    }
}
