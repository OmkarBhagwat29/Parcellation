using Rhino;
using Rhino.Commands;
using Rhino.UI;
using UrbanDesign.Ui.Views;

namespace Parcellation.Commands
{
    public class Parcellation_Command : Command
    {
        public override string EnglishName => "Parcellation";

        /// <summary>
        /// Form accessor
        /// </summary>
        private ParcellationView Form
        {
            get;
            set;
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            //MainWindow win = new();
            //win.ShowSemiModal(RhinoApp.MainWindowHandle());

            if (null == Form)
            {
                Form = new ParcellationView { Owner = RhinoEtoApp.MainWindow };
                Form.Closed += OnFormClosed;
                Form.Show(doc);
            }
            return Result.Success;
        }

        private void OnFormClosed(object sender, EventArgs e)
        {
            Form.Dispose();
            Form = null;
        }

    }
}
