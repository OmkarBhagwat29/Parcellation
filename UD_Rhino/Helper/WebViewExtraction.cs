using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Parcellation.Helper
{
    public static class WebViewExtraction
    {

        public static void ExtractResources()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Parcellation.out.parcellation.html"; // Adjust to your actual resource name

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var fileStream = new FileStream("C:\\Temp\\parcellation.html", FileMode.Create, FileAccess.Write))
            {
                stream.CopyTo(fileStream);
            }
        }
    }
}
