using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;
using System.Text;

namespace OpenAIDemos;

internal class ChatCompletions(IOptions<OpenAISettings> settings) : IChatCompletions
{
    private readonly OpenAISettings _settings = settings.Value;

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

    public void CreatePieDescription()
    {
        OpenAIClient openAIClient = new(_settings.APIKey);

        ChatClient chatClient = openAIClient.GetChatClient(_settings.ChatModelName);

        var systemPrompt = "You are a helpful assistant who creates descriptions for products in an online store.";

        Console.WriteLine("Enter a product:");
        var userPrompt = Console.ReadLine() ?? string.Empty;
        Console.WriteLine("Working on it...");

        ChatCompletion completion = chatClient.CompleteChat(
        [
            new SystemChatMessage(systemPrompt),
            new AssistantChatMessage("I can help with creating product descriptions. What can I do for you?."),
            new UserChatMessage(userPrompt)
        ]);

        Console.WriteLine($"\n[{completion.Role}]: {completion.Content[0].Text}");
    }

    public void CreateBetterPieName()
    {
        OpenAIClient openAIClient = new(_settings.APIKey);

        ChatClient chatClient = openAIClient.GetChatClient(_settings.ChatModelName);

        var systemPrompt = "You are a helpful assistant who creates better names for products in an online store. You will return just one single name suggestion please.";

        var userPromptSample =
            """
            Product description: A cheese cake
            Seed words: creamy, flavour, heavy
            Product names: DreamCake, Creamy-Dreamy-Cheesy Cake

            Product description: A cherry pie
            Seed words: red, summer
            Product names: Cherry Dream, Red Summer Cherry Pie
            """;

        Console.WriteLine("Enter a product:");
        var userPrompt = Console.ReadLine() ?? string.Empty;
        Console.WriteLine("Working on it...");

        ChatCompletion completion = chatClient.CompleteChat(
        [
            new SystemChatMessage(systemPrompt),
            new UserChatMessage($"Here are sample product descriptions and names: {userPromptSample}"),
            new AssistantChatMessage("I can help with creating product descriptions. What can I do for you?."),
            new UserChatMessage(userPrompt)
        ]);

        Console.WriteLine($"\n[{completion.Role}]: {completion.Content[0].Text}");
    }

    public void CreatePoshAndFancyPieDescription()
    {
        OpenAIClient openAIClient = new(_settings.APIKey);

        ChatClient chatClient = openAIClient.GetChatClient(_settings.ChatModelName);

        var systemPrompt =
            """
            You are a helpful assistant who creates descriptions for products in an online store.
            You are using a very posh and fancy language tone of voice for this.
            Include emojis in the response where applicable.
            Please use UK English spelling.
            """;


        Console.WriteLine("Enter a product:");
        var userPrompt = Console.ReadLine() ?? string.Empty;
        Console.WriteLine("Working on it...");

        ChatCompletion completion = chatClient.CompleteChat(
        [
            new SystemChatMessage(systemPrompt),
            new AssistantChatMessage("I can help with creating product descriptions. What can I do for you?."),
            new UserChatMessage(userPrompt)
        ]);

        Console.WriteLine($"\n[{completion.Role}]: {completion.Content[0].Text}");
    }

    public void CreateIngredientTable()
    {
        OpenAIClient openAIClient = new(_settings.APIKey);

        ChatClient chatClient = openAIClient.GetChatClient(_settings.ChatModelName);

        var systemPrompt =
            """
            You are a helpful assistant who creates ingredients for pies.
            You should return data as a 3 column spreadsheet in markdown format with the columns being Ingredient Name, Quantity, Description.
            Make sure to use the mettric system for quantities.
            Please use UK English spelling.
            """;


        Console.WriteLine("Enter a product:");
        var userPrompt = Console.ReadLine() ?? string.Empty;
        Console.WriteLine("Working on it...");

        ChatCompletion completion = chatClient.CompleteChat(
        [
            new SystemChatMessage(systemPrompt),
            new AssistantChatMessage("I can help with creating a list of pie ingredients. What can I do for you?."),
            new UserChatMessage(userPrompt)
        ]);

        Console.WriteLine($"\n[{completion.Role}]: {completion.Content[0].Text}");
    }

    public void CreatePieDescriptionWithOptions()
    {
        var completionOptions = new ChatCompletionOptions
        {
            MaxOutputTokenCount = 300,
            Temperature = 0.5f,
            FrequencyPenalty = 0.0f,
            PresencePenalty = 0.0f,
            ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat(),
        };

        OpenAIClient openAIClient = new(_settings.APIKey);

        ChatClient chatClient = openAIClient.GetChatClient(_settings.ChatModelName);

        var systemPrompt =
            """
            You are a helpful assistant who creates descriptions for products in an online store.
            You should return the reponse in JSON format please.
            """;

        Console.WriteLine("Enter a product:");
        var userPrompt = Console.ReadLine() ?? string.Empty;
        Console.WriteLine("Working on it...");

        ChatCompletion completion = chatClient.CompleteChat(
        [
            new SystemChatMessage(systemPrompt),
            new AssistantChatMessage("I can help with creating product descriptions. What can I do for you?."),
            new UserChatMessage(userPrompt)
        ], completionOptions);

        Console.WriteLine($"\n[{completion.Role}]: {completion.Content[0].Text}");
    }

    public async Task OpenEndedChatAsync()
    {
        OpenAIClient openAIClient = new(_settings.APIKey);

        ChatClient chatClient = openAIClient.GetChatClient(_settings.ChatModelName);

        List<ChatMessage> messages =
        [
            new SystemChatMessage("You are a helpful assistant who is very knowledgeable in the food space."),
            new AssistantChatMessage("I know a lot about food. What can I help with you today?")
        ];

        Console.WriteLine($"\n[ASSISTANT]: {messages[^1].Content[0].Text}\n");

        while (true)
        {
            var stringBuilder = new StringBuilder();

            Console.Write("[USER]: ");

            var input = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(input))
            {
                break;
            }

            messages.Add(new UserChatMessage(input));

            Console.Write("[ASSISTANT]: ");

            await foreach (var update in chatClient.CompleteChatStreamingAsync(messages))
            {
                foreach (ChatMessageContentPart contentPart in update.ContentUpdate)
                {
                    stringBuilder.Append(contentPart.Text);
                    Console.Write(contentPart.Text);
                }
            }

            Console.WriteLine();

            messages.Add(new AssistantChatMessage(stringBuilder.ToString()));
        }
    }
}