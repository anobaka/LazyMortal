using Bootstrap.Extensions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace Bootstrap.Components.Miscellaneous
{
    public class ImageUtils
    {
        public static byte[] Crop(byte[] originalImage, Rectangle rect)
        {
            using var image = Image.Load(originalImage, out var format);
            image.Mutate(t => t.Crop(rect));
            return image.Save(format);
        }

        public static byte[] Compress(byte[] originalImage, int quality)
        {
            using var image = Image.Load(originalImage);
            return image.Compress(quality);
        }

        public static void Compress(string filename, int quality, string destFilename)
        {
            using var image = Image.Load(filename);
            image.Save(destFilename, new JpegEncoder {Quality = quality});
        }
    }
}