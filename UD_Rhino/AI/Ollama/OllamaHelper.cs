
using Parcellation.AI.Ollama;


namespace UrbanDesign.AI
{
    public static class OllamaHelper
    {
        public static string CurrentQuery;
        public static string Context;

        public static OllamaApiClient Ollama = new OllamaApiClient(new Uri("http://localhost:11434"));

        public static ChatRequestBuilder Request = new ChatRequestBuilder();

        public static async Task GetOllamaResponseFromFunctionCalling(Action<string, string> sendToWebViewUi)
        {
            ChatRequest req = new ChatRequest();
            req.Model = "urban";

            var context = new { Query = CurrentQuery, QueryContext = Context };
            var contextString = JsonSerializer.Serialize(context);

            req.Messages = new List<Message>() {
            new Message{ Content=contextString,Role="user"}
            };

            var response  = Ollama.ChatAsync(req);

            var resBuilder = new StringBuilder();
            await foreach (var responseStream in response)
            {
                if (responseStream?.Message.Content is not null)
                {
                    resBuilder.Append(responseStream?.Message.Content);
                }
            }

            sendToWebViewUi(CurrentQuery, resBuilder.ToString());
        }


        public static async Task OllamaFunctionCall(Functions functions,
            string query,
            Action<string, string> sendToWebViewUi)
        {
            CurrentQuery = query;

            Request.ClearMessages();

            var req = Request.AddMessage(query, "user").Build();

            var chat = Ollama.ChatAsync(req);

            var functionDetails = new List<FunctionDetails>();

            await foreach (var responseStream in chat)
            {
                if (responseStream?.Message.ToolCalls?.Count() > 0)
                {
                    var tools = responseStream.Message.ToolCalls.ToList();

                    if (tools is null)
                        continue;

                    foreach (var tool in tools)
                    {
                        var func = tool.Function;

                        var name = func.Name;
                        var arguments = func.Arguments;

                        var funcString = JsonSerializer.Serialize(func);
                        var argString = JsonSerializer.Serialize(arguments);

                        var fParams = JsonSerializer.Deserialize<FunctionParameters>(argString);

                        var fDetail = new FunctionDetails(name, fParams, "");

                        functionDetails.Add(fDetail);
                    }
                    
                }
            }

            functionDetails.ForEach(fDetail => {var content =  functions.Execute(fDetail);

                if(content!=string.Empty)
                    sendToWebViewUi(query, content);
            });


        }
    }
}
