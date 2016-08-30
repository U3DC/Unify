using Rhino;
using Rhino.Commands;

// Unify: Leland Jobson
// Unify: Konrad K Sobon
// Unify: David Mans

namespace Unify
{
    [System.Runtime.InteropServices.Guid("7cce0799-e273-49c2-ab0b-fe44c5ba3417"), CommandStyle(Style.ScriptRunner)]
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
            // launch Unify window
            var result = Result.Cancel;
            if (mode == RunMode.Interactive)
            {
                InputData inputData = new InputData(doc);
                var form = new UnifyForm(inputData) { StartPosition = System.Windows.Forms.FormStartPosition.CenterParent };
                var dialog_result = form.ShowDialog(RhinoApp.MainWindow());
                if (dialog_result == System.Windows.Forms.DialogResult.OK)
                {
                    result = Result.Success;
                }
            }
            return result;
        }
    }

}