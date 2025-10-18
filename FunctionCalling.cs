using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;
using System.Text.Json;

namespace OpenAIDemos
{
    internal class FunctionCalling(IOptions<OpenAISettings> settings) : IFunctionCalling
    {
        private readonly OpenAISettings _settings = settings.Value;

        private static readonly ChatTool getClosestPieShopTool =
            ChatTool.CreateFunctionTool(
                functionName: nameof(GetClosestPieShop),
                functionDescription: "Get the closest pie shop based on the user location."
            );

        private static readonly ChatTool getWeatherAtPieShopTool =
            ChatTool.CreateFunctionTool(
                functionName: nameof(GetWeatherAtPieShop),
                functionDescription: "Get the weather at the given pie shop.",
                functionParameters: BinaryData.FromString("""
                    {
                        "type": "object",
                        "properties": {
                            "location": {
                                "type": "string",
                                "description": "The city and country"
                            },
                            "unit": {
                                "type": "string",
                                "enum": [ "celsius", "fahrenheit" ],
                                "description": "The temperature unit to use"
                            }
                        },
                        "required": [ "location" ]
                    }
                    """)
            );

        private static string GetClosestPieShop()
        {
            // In a real implementation, you would look up the closest pie shop based on the user location.
            return "Horncastle, UK";
        }

        private static string GetWeatherAtPieShop(string location, string unit = "celsius")
        {
            // In a real implementation, you would call a weather API to get the weather at the given location.
            return unit.ToLower() == "fahrenheit"
                ? "The weather at Horncastle, UK is 55°F and sunny."
                : "The weather at Horncastle, UK is 13°C and sunny.";
        }

        public void SimpleFunctionCalling()
        {
            bool requiresAction = true;

            OpenAIClient openAIClient = new(_settings.APIKey);

            ChatClient chatClient = openAIClient.GetChatClient(_settings.ChatModelName);

            List<ChatMessage> messages =
            [
                new UserChatMessage("I want to go to the nearest Pie Shop. What clothes should I wear?")
            ];

            ChatCompletionOptions options = new()
            {
                Tools =
                {
                    getClosestPieShopTool,
                    getWeatherAtPieShopTool
                },
            };

            while (requiresAction)
            {
                requiresAction = false;
                ChatCompletion response = chatClient.CompleteChat(messages, options);

                switch (response.FinishReason)
                {
                    case ChatFinishReason.ToolCalls:
                    {
                        messages.Add(new AssistantChatMessage(response));

                        foreach (ChatToolCall toolCall in response.ToolCalls)
                        {
                            switch (toolCall.FunctionName)
                            {
                                case nameof(GetClosestPieShop):
                                {
                                    var toolResult = GetClosestPieShop();
                                    messages.Add(new ToolChatMessage(toolCall.Id, toolResult));
                                    break;
                                }

                                case nameof(GetWeatherAtPieShop):
                                {
                                    using JsonDocument argumentsJson = JsonDocument.Parse(toolCall.FunctionArguments);
                                    bool hasLocation = argumentsJson.RootElement.TryGetProperty("location", out JsonElement location);
                                    bool hasUnit = argumentsJson.RootElement.TryGetProperty("unit", out JsonElement unit);
                                    var toolResult = hasLocation
                                        ? GetWeatherAtPieShop(location.GetString()!, hasUnit ? unit.GetString()! : "celsius")
                                        : "Error: location argument is required.";
                                    messages.Add(new ToolChatMessage(toolCall.Id, toolResult));
                                    break;
                                }
                            }
                        }

                        requiresAction = true;
                        break;
                    }

                    case ChatFinishReason.Stop:
                    {
                        messages.Add(new AssistantChatMessage(response));
                        break;
                    }

                    case ChatFinishReason.Length:
                    {
                        throw new InvalidOperationException("The response was cut off due to length limits.");
                    }

                    case ChatFinishReason.ContentFilter:
                    {
                        throw new InvalidOperationException("The response was blocked by the content filter.");
                    }

                    case ChatFinishReason.FunctionCall:
                    {
                        throw new InvalidOperationException("The response was blocked by the function call.");
                    }
                }

                foreach (ChatMessage message in messages)
                {
                    if (message.Content.Count > 0)
                    {
                        Console.WriteLine(message.Content[0].Text);
                    }
                }
            }
        }
    }
}