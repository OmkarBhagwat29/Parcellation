
using UD.UI.Parcellation_UI;

namespace UD.UD.UI.Parcellation_UI.Helpers
{
    public static class CommandRunner
    {
        public static void UseCommandAction(this CommandAction action, Action doAction)
        {
            try
            {
                doAction.Invoke();
            }
            catch (Exception ex)
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
                output = doAction.Invoke();
            }
            catch
            {

                throw;
            }

            return output;
        }
    }
}
