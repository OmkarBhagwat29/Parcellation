using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrbanDesign.AI.Ollama
{
    public record ChatResponse();

    public record ChatMessage(string role,string content);
}
