namespace OpenAIDemos;

public interface IChatCompletions
{
    void SimpleChat();
    Task SimpleChatAsync();
    Task SimpleChatStreamingAsync();
    void SimpleChatUsingOpenAIClient();
    Task SimpleChatUsingOpenAIClientWithMessagesAsync();
    void CreatePieDescription();
    void CreateBetterPieName();
    void CreatePoshAndFancyPieDescription();
    void CreateIngredientTable();
    void CreatePieDescriptionWithOptions();
    Task OpenEndedChatAsync();
}
