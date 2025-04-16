using Rhino.Commands;
using Rhino.UI;
using Rhino;


using UD.UI.Map;

namespace Parcellation.Commands
{
    public class MapCommand : Command
    {
        public override string EnglishName => "UDMap";

        public MapUi Ui { get; set; }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {

            if (null == Ui)
            {
                Ui = new MapUi(new Uri("http://localhost:3000/map"))
                { Owner = RhinoEtoApp.MainWindow };
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
