using Rhino;
using Rhino.Commands;
using Rhino.UI;
using RhinoWindows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfWebViewTest;

namespace UrbanDesign.RhinoCommands
{
    public class WpfWebViewTest_Command : Command
    {
        public override string EnglishName => "WebViewTest";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            var mn = new MainWindow();

            mn.MakeRhinoMainWindowTheOwner();

            mn.ShowSemiModal(RhinoApp.MainWindowHandle());


            //System.Threading.Thread thread = new System.Threading.Thread(() =>
            //{
            //    MainWindow win = new MainWindow();
            //    win.ShowDialog();
            //});

            //thread.SetApartmentState(System.Threading.ApartmentState.STA);
            //thread.Start();

            return Result.Success;
        }
    }
}
