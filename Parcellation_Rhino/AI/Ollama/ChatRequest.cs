using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrbanDesign.AI
{
    public record ChatRequest(string Model,List<Message>Messages,bool Stream);

    public record Message(string role, string content);
}
