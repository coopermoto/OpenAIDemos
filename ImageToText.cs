using Microsoft.Extensions.Options;
using OpenAI.Chat;

namespace OpenAIDemos
{
    internal class ImageToText(IOptions<OpenAISettings> settings) : IImageToText
    {
        private readonly OpenAISettings _settings = settings.Value;

        public void DescribeImage()
        {
            ChatClient client = new(_settings.ChatModelName, _settings.APIKey);

            using Stream stream = File.OpenRead("Images/waterfall.png");

            BinaryData imageBytes = BinaryData.FromStream(stream);

            List<ChatMessage> messages = new()
            {
                new UserChatMessage(
                    ChatMessageContentPart.CreateTextPart("Describe the image in detail."),
                    ChatMessageContentPart.CreateImagePart(imageBytes, "image/png"))
            };

            ChatCompletion chatCompletion = client.CompleteChat(messages);

            Console.WriteLine("Image Description:");
            Console.WriteLine(chatCompletion.Content[0].Text);
        }
    }
}
