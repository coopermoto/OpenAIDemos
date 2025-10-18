using Microsoft.Extensions.Options;
using OpenAI.Images;

namespace OpenAIDemos;

internal class ImageGeneration(IOptions<OpenAISettings> settings) : IImageGeneration
{
    private readonly OpenAISettings _settings = settings.Value;

    public void GenerateImage()
    {
        ImageClient client = new(_settings.ImageModelName, _settings.APIKey);

        string prompt =
            """
            A cozy and inviting pie shop that blends the warmth of rustic charm with the elegance of modern design, creating a space that feels both nostalgic and contemporary.
            The ambience is centred around a p-alette of warm, earthy tones - thiink rich browns, soft creams and muted greens - complemented by natural wood finishes that exude a sense of comfort and tradition.

            The shop features handcrafted wooden tables with subtle details, surrounded by comfortable seating that encourages customers to linger and enjoy their treats.
            Large windows allow natural light to flood the space, creating an airy and welcoming environment.

            On the walls, you'll find a mix of vintage-inspired artwork and shelves displaying artisanal products and fresh flowers, which add a personal touch and a connection to nature.
            The counter, where pies are displayed, is the focal point, with a clean, minimalist design that highlights the vibrant colors and textures of the baked goods.

            Incorporating soft textiles, such as woven rugs and cushions in natural fabrics, adds warmth and coziness, making the space feel like a home away from home.
            The overall atmosphere invites customers to relax, savor their pie, and enjoy a moment of simple pleasure in a beautifully crafted setting.
            """;

        Console.WriteLine("Generating image...");

        ImageGenerationOptions options = new()
        {
            Quality = GeneratedImageQuality.High,
            Size = GeneratedImageSize.W1792xH1024,
            Style = GeneratedImageStyle.Vivid,
            ResponseFormat = GeneratedImageFormat.Bytes,
        };

        GeneratedImage generatedImage = client.GenerateImage(prompt, options);
        BinaryData bytes = generatedImage.ImageBytes;

        using FileStream stream = File.OpenWrite($"{Guid.NewGuid()}.png");
        bytes.ToStream().CopyTo(stream);

        Console.WriteLine("Image generated and saved.");
    }

    public void GenerateImageVariation()
    {
        ImageClient client = new(_settings.ImageModelName, _settings.APIKey);

        var image = "Images/waterfall.png";

        ImageVariationOptions options = new()
        {
            Size = GeneratedImageSize.W1024xH1024,
            ResponseFormat = GeneratedImageFormat.Bytes,
        };

        GeneratedImage variation = client.GenerateImageVariation(image, options);
        BinaryData bytes = variation.ImageBytes;

        using FileStream stream = File.OpenWrite($"{Guid.NewGuid()}.png");
        bytes.ToStream().CopyTo(stream);

        Console.WriteLine("Image generated and saved.");
    }
}
