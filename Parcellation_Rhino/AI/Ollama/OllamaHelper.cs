using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace UrbanDesign.AI.Ollama
{
    public static class OllamaHelper
    {

        public static async Task<string> GetOllamaResponse(string userMessage, object context, Action<string, string> sendToWebViewUi)
        {
            using (var httpClient = new HttpClient())
            {
                string apiUrl = "http://localhost:11434/api/chat";

                // Convert context object to JSON string
                string contextJson = JsonSerializer.Serialize(context);

                var chatRequest = new ChatRequest("urban", [new Message("user", $"{userMessage}")],false);


                string jsonBody = JsonSerializer.Serialize(chatRequest);

                //\n\nContext:\n{contextJson}

                var request = new HttpRequestMessage(HttpMethod.Post, apiUrl)
                {
                    Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
                };

                var response = await httpClient.SendAsync(request);
                string responseBody = await response.Content.ReadAsStringAsync();

                try
                {
                    var responseJson = JsonSerializer.Deserialize<JsonElement>(responseBody);

                    if (responseJson.TryGetProperty("message", out JsonElement responseElement))
                    {
                        if (responseElement.TryGetProperty("content", out JsonElement answerElm))
                        {
                            var chat = JsonSerializer.Deserialize<ChatMessage>(responseElement);
                            string answer = answerElm.ToString();
                            sendToWebViewUi(userMessage, answer);
                            return answer;
                        }
                        else
                        {
                            sendToWebViewUi(userMessage, "Error: Response key not found.");
                            return "Error: Response key not found.";
                        }

                        

                    }
                    else
                    {
                        sendToWebViewUi(userMessage, "Error: Response key not found.");
                        return "Error: Response key not found.";
                    }
                }
                catch (JsonException)
                {
                    sendToWebViewUi(userMessage, "Error: Invalid JSON response from Ollama.");
                    return "Error: Invalid JSON response from Ollama.";
                }
            }
        }

    }
}
