using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;

namespace OpenAIDemos;

internal class ChatCompletions(IOptions<OpenAISettings> settings) : IChatCompletions
{
    private readonly OpenAISettings _settings = settings.Value;
    private const string ModelName = "ApenAI:ModelName";
    private const string OpenAIAPIKey = "OpenAI:APIKey";

    public void SimpleChat()
    {
        ChatClient client = new(_settings.ChatModelName, _settings.APIKey);

        ChatCompletion completion = client.CompleteChat("Say 'Hello AI world'");

        Console.WriteLine($"[ASSISTANT]: {completion.Content[0].Text}");
    }

    public void SimpleChatUsingOpenAIClient()
    {
        OpenAIClient openAIClient = new(_settings.APIKey);

        ChatClient chatClient = openAIClient.GetChatClient(_settings.ChatModelName);

        ChatCompletion completion = chatClient.CompleteChat("Say 'Hello AI world'");

        Console.WriteLine($"[ASSISTANT]: {completion.Content[0].Text}");
    }

    public async Task SimpleChatAsync()
    {
        ChatClient client = new(_settings.ChatModelName, _settings.APIKey);

        ChatCompletion completion = await client.CompleteChatAsync("Say 'Hello AI world'");

        Console.WriteLine($"[ASSISTANT]: {completion.Content[0].Text}");
    }

    public async Task SimpleChatStreamingAsync()
    {
        ChatClient client = new(_settings.ChatModelName, _settings.APIKey);

        AsyncCollectionResult<StreamingChatCompletionUpdate> completionUpdates = client.CompleteChatStreamingAsync("Say 'Hello AI world' 20 times");

        await foreach (StreamingChatCompletionUpdate completionUpdate in completionUpdates)
        {
            foreach (ChatMessageContentPart contentPart in completionUpdate.ContentUpdate)
            {
                Console.Write(contentPart.Text);
            }
        }
    }

    public async Task SimpleChatUsingOpenAIClientWithMessagesAsync()
    {
        OpenAIClient openAIClient = new(_settings.APIKey);

        ChatClient chatClient = openAIClient.GetChatClient(_settings.ChatModelName);

        ChatMessage[] chatMessages =
        [
            new SystemChatMessage("You are a helpful assistant who is very knowledgeable in the food space."),
            new UserChatMessage("Hi, can you help me?"),
            new AssistantChatMessage("Of course, dear aspiring foodie! What can I do for you?"),
            new UserChatMessage("Can you give me a list of 10 of the most loved sweet dessert pies around the world please?")
        ];

        AsyncCollectionResult<StreamingChatCompletionUpdate> completionUpdates = chatClient.CompleteChatStreamingAsync(chatMessages);

        await foreach (StreamingChatCompletionUpdate completionUpdate in completionUpdates)
        {
            foreach (ChatMessageContentPart contentPart in completionUpdate.ContentUpdate)
            {
                Console.Write(contentPart.Text);
            }
        }
    }
}
