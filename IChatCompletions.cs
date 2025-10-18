namespace OpenAIDemos;

public interface IChatCompletions
{
    void SimpleChat();
    Task SimpleChatAsync();
    Task SimpleChatStreamingAsync();
    void SimpleChatUsingOpenAIClient();
    Task SimpleChatUsingOpenAIClientWithMessagesAsync();
}
