using SixLabors.ImageSharp;

namespace Eventnet.Domain;

// Disable only windows-supported functions warnings
#pragma warning disable CA1416
public class Photo
{
    private readonly byte[] rawData;
    private readonly string extension;

    public Photo(byte[] rawData, string contentType)
    {
        this.rawData = rawData;
        extension = GetFormatAndExtensionFromContentType(contentType);
    }

    public void Save(string path)
    {
        using var ms = new MemoryStream(rawData);
        var photo = Image.Load(ms);
        photo.Save(path + extension);
    }

    private static  string GetFormatAndExtensionFromContentType(string contentType)
    {
        return contentType switch
        {
            "image/jpeg" =>  ".jpeg",
            "image/png"  => ".png",
            "image/bmp"  => ".bmp",
            _            => throw new ArgumentOutOfRangeException($"Unknown content type {contentType}")
        };
    }
}