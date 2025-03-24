using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace UrbanDesign.Ui.Helpers
{
    public static class CommandRunner
    {
        public static void UseCommandAction(this CommandAction action, Action doAction)
        {
			try
			{
				doAction.Invoke();
			}
			catch(Exception ex)
			{

				Rhino.UI.Dialogs.ShowMessage($"An error occurred: {ex.Message}", "Error");
                Rhino.RhinoApp.WriteLine($"Exception: {ex}");
            }
		}

		public static TReturn UseCommandAction<TReturn>(this CommandAction action, Func<TReturn> doAction)
		{
            TReturn output = default;
            try
			{
              output =  doAction.Invoke();
            }
			catch
			{

				throw;
			}

			return output;
        }
    }
}
