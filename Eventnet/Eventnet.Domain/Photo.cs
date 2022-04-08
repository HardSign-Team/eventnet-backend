using System.Drawing;
using System.Drawing.Imaging;

namespace Eventnet.Domain;

public class Photo
{
    private readonly byte[] rawData;
    private readonly ImageFormat imageFormat;
    private readonly string extension;
    
    public Photo(byte[] rawData, string contentType)
    {
        this.rawData = rawData;
        (imageFormat, extension) = GetFormatAndExtensionFromContentType(contentType);
    }
    
    private static (ImageFormat, string) GetFormatAndExtensionFromContentType(string contentType)
    {
        return contentType switch
        {
            "image/jpeg" => (ImageFormat.Jpeg, ".jpeg"),
            "image/png"  => (ImageFormat.Png, ".png"),
            "image/bmp"  => (ImageFormat.Bmp, ".bmp"),
            _            => throw new ArgumentOutOfRangeException($"Unknown content type {contentType}")
        };
    }

    public void Save(string path)
    {
        using var ms = new MemoryStream(rawData);
        var photo = Image.FromStream(ms);
        photo.Save(path + extension, imageFormat);
    }
}