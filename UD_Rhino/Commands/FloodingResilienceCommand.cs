using Parcellation.UD.Flooding;
using Rhino;
using Rhino.Commands;
using Rhino.UI;
using UD.UI.Flooding;



namespace Parcellation.Commands
{
    public class FloodingResilienceCommand : Command
    {
        public override string EnglishName => "Flooding";

        public FloodingUi Ui { get; set; }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {

            if (null == Ui)
            {
                Ui = new FloodingUi(new Uri("http://localhost:3000/flooding"))
                { Owner = RhinoEtoApp.MainWindow};
                Ui.Closed += Ui_Closed;
                Ui.Show(doc);
            }

           return Result.Success;
        }

        private void Ui_Closed(object sender, EventArgs e)
        {
            Ui.Dispose();
            Ui = null;
        }
    }
}
