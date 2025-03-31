using System.Collections.Generic;

namespace Parcellation.AI;

public class ChatRequestBuilder
{
    private readonly ChatRequest _chatRequest = new();
    private readonly List<Message> _messages = new();
    private readonly List<Tool> _tools = new();


    public void ClearMessages() => _messages.Clear();

    public ChatRequestBuilder SetModel(string model)
    {
        _chatRequest.Model = model;

        return this;
    }

    public ChatRequestBuilder AddMessage(string content, string role)
    {
        _messages.Add(new Message
        {
            Content = content,
            Role = role
        });

        return this;
    }

    public ChatRequestBuilder AddFunctions(Functions functions)
    {
        functions.GetFunctionDetails().ForEach(function => AddTool(this, function));

        return this;
    }

    public ChatRequest Build()
    {
        _chatRequest.Messages = _messages;
        _chatRequest.Tools = _tools;

        return _chatRequest;
    }

    public static ChatRequestBuilder AddTool(ChatRequestBuilder builder, FunctionDetails functionDetails)
    {

        var p = new Property();
        p.Type = "string";
        p.Description = "The parcellation system for urban design";

        builder.AddTool(
            "function",
            functionDetails.Name,
            functionDetails.Description,
            "object",
            new Dictionary<string, Property>()
            {
                { "value", p }
            },
            ["value"]);

        return builder;
    }

    private ChatRequestBuilder AddTool(
        string type,
        string functionName,
        string functionDescription,
        string parameterType,
        Dictionary<string, Property> properties,
        List<string> required)
    {
        _tools.Add(new Tool
        {
            Type = type,
            Function = new Function
            {
                Name = functionName,
                Description = functionDescription,
                Parameters = new Parameters
                {
                    Type = parameterType,
                    Properties = properties,
                    Required = required
                }
            }
        });

        return this;
    }
}