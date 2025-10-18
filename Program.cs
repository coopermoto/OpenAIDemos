using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenAIDemos;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.Configure<OpenAISettings>(builder.Configuration.GetSection("OpenAI"));
builder.Services.AddScoped<IChatCompletions, ChatCompletions>();
builder.Services.AddScoped<IImageGeneration, ImageGeneration>();
var host = builder.Build();
using var scope = host.Services.CreateScope();

var chatCompletions = scope.ServiceProvider.GetRequiredService<IChatCompletions>();

//chatCompletions.SimpleChat();
//chatCompletions.SimpleChatUsingOpenAIClient();
//await chatCompletions.SimpleChatAsync();
//await chatCompletions.SimpleChatStreamingAsync();
//await chatCompletions.SimpleChatUsingOpenAIClientWithMessagesAsync();
//chatCompletions.CreatePieDescription();
//chatCompletions.CreateBetterPieName();
//chatCompletions.CreatePoshAndFancyPieDescription();
//chatCompletions.CreateIngredientTable();
//chatCompletions.CreatePieDescriptionWithOptions();
//await chatCompletions.OpenEndedChatAsync();

var imageGeneration = scope.ServiceProvider.GetRequiredService<IImageGeneration>();

//imageGeneration.GenerateImage();
imageGeneration.GenerateImageVariation();
