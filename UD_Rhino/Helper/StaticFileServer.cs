
using System.IO;
using System.Net;


namespace Parcellation.Helper
{
    public class StaticFileServer
    {
        private readonly HttpListener _listener = new HttpListener();
        private readonly string _basePath;
        private bool _isRunning;

        public StaticFileServer(string basePath, int port = 3000)
        {
            _basePath = basePath;
            _listener.Prefixes.Add($"http://localhost:{port}/");
        }

        public void Start()
        {
            _isRunning = true;
            _listener.Start();

            Task.Run(() =>
            {
                while (_isRunning)
                {
                    var context = _listener.GetContext();
                    Task.Run(() => HandleRequest(context));
                }
            });
        }

        public void Stop()
        {
            _isRunning = false;
            _listener.Stop();
        }

        private void HandleRequest(HttpListenerContext context)
        {
            string urlPath = context.Request.Url.AbsolutePath.TrimStart('/');

            // Default to "index.html" if the URL path is empty
            if (string.IsNullOrEmpty(urlPath)) urlPath = "index.html";

            // Ensure we properly resolve paths inside _next and other folders
            string filePath = Path.Combine(_basePath, urlPath);

            // If the file doesn't exist, respond with 404
            if (!File.Exists(filePath))
            {
                context.Response.StatusCode = 404;
                using var writer = new StreamWriter(context.Response.OutputStream);
                writer.Write("404 Not Found");
            }
            else
            {
                try
                {
                    byte[] content = File.ReadAllBytes(filePath);
                    context.Response.ContentType = GetMimeType(filePath);
                    context.Response.ContentLength64 = content.Length;
                    context.Response.OutputStream.Write(content, 0, content.Length);
                }
                catch (Exception ex)
                {
                    // In case there's an issue reading the file, return a 500 error
                    context.Response.StatusCode = 500;
                    using var writer = new StreamWriter(context.Response.OutputStream);
                    writer.Write($"500 Internal Server Error: {ex.Message}");
                }
            }

            context.Response.OutputStream.Close();
        }

        private string GetMimeType(string filePath)
        {
            string ext = Path.GetExtension(filePath).ToLowerInvariant();
            return ext switch
            {
                ".html" => "text/html",
                ".js" => "application/javascript",
                ".css" => "text/css",
                ".json" => "application/json",
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".svg" => "image/svg+xml",
                ".woff" => "font/woff",
                ".woff2" => "font/woff2",
                ".ttf" => "font/ttf",
                ".eot" => "application/vnd.ms-fontobject",
                _ => "application/octet-stream"
            };
        }
    }
}
