using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Tiff;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Bootstrap.Extensions
{
    public static class ImageSharpExtensions
    {
        public static List<Rectangle> FindPositions(this Image<Rgba32> main, Image<Rgba32> sub,
            double similarity = 1, int firstN = int.MaxValue)
        {
            // rectangle - miss count
            var candidates = new Dictionary<Rectangle, int>();
            var results = new List<Rectangle>();
            var subW = sub.Width;
            var subH = sub.Height;
            var mainW = main.Width;
            var mainH = main.Height;
            // 误差点默认为误差颜色值/4
            var maxMissCount = subW * subH * (1 - similarity) / 4;
            // var maxMissCount = 0;
            if (subW <= mainW && subH <= mainH)
            {
                var leftTopColor = sub[0, 0];
                for (var x = 0; x <= mainW - subW; x++)
                {
                    for (var y = 0; y <= mainH - subH; y++)
                    {
                        var color = main[x, y];
                        foreach (var (r, missCount) in candidates.ToArray())
                        {
                            var subX = x - r.X;
                            var subY = y - r.Y;
                            if (subY < 0 || subX >= subW || subY >= subH)
                            {
                                if (subX >= subW && subY >= subH)
                                {
                                    results.Add(r);
                                    candidates.Remove(r);
                                    if (results.Count >= firstN)
                                    {
                                        return results;
                                    }
                                }

                                continue;
                            }

                            var subColor = sub[subX, subY];
                            if (!subColor.IsSimilarTo(color, similarity))
                            {
                                if (missCount > maxMissCount)
                                {
                                    candidates.Remove(r);
                                }
                                else
                                {
                                    candidates[r]++;
                                }
                            }
                        }

                        if (color.IsSimilarTo(leftTopColor, similarity))
                        {
                            candidates.Add(new Rectangle(x, y, subW, subH), 0);
                        }
                    }
                }
            }

            return results;
        }

        public static bool IsSimilarTo(this Rgba32 a, Rgba32 b, double similarity = 1)
        {
            var comparisons = new List<(int, int)>
            {
                (a.R, b.R),
                (a.G, b.G),
                (a.B, b.B)
            };
            return comparisons.All(t => Math.Abs(t.Item1 - t.Item2) <= 256 * (1 - similarity));
        }

        public static Rectangle? FindPosition(this Image<Rgba32> parent, Image<Rgba32> target, double similarity = 1)
        {
            var ps = parent.FindPositions(target, similarity, 1);
            return ps.Any() ? ps.FirstOrDefault() : (Rectangle?) null;
        }

        public static bool Equals(this Image<Rgba32> a, Image<Rgba32> b)
        {
            if (a.Width == b.Width && a.Height == b.Height)
            {
                for (var x = 0; x < a.Width; x++)
                {
                    for (var y = 0; y < a.Height; y++)
                    {
                        if (!a[x, y].Equals(b[x, y]))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public static Rectangle CropByGrid(this Rectangle rectangle, int rowCount, int columnCount, int firstRowIndex,
            int firstColumnIndex, int? lastRowIndex = null, int? lastColumnIndex = null)
        {
            lastRowIndex ??= firstRowIndex;
            lastColumnIndex ??= firstColumnIndex;
            var (x, y, width, height) = rectangle;
            var uw = (decimal) width / columnCount;
            var uh = (decimal) height / rowCount;
            var x1 = (int) (uw * firstColumnIndex);
            var y1 = (int) (uh * firstRowIndex);
            var x2 = lastColumnIndex.Value == columnCount - 1
                ? rectangle.Width
                : (int) (uw * (lastColumnIndex.Value + 1));
            var y2 = lastRowIndex.Value == rowCount - 1
                ? rectangle.Height
                : (int) (uh * (lastRowIndex.Value + 1));
            var offset = new Size(rectangle.Location);
            return new Rectangle(new Point(x1, y1) + offset, new Size(x2 - x1 + 1, y2 - y1 + 1));
        }

        public static Point BottomRight(this Rectangle rectangle) => new Point(rectangle.Right, rectangle.Bottom);
        public static Point BottomLeft(this Rectangle rectangle) => new Point(rectangle.Left, rectangle.Bottom);

        public static Point Center(this Rectangle rectangle) => new Point(rectangle.Left + rectangle.Width / 2,
            rectangle.Top + rectangle.Height / 2);

        public static bool In(this Rectangle rectangle, Rectangle largerRectangle) =>
            rectangle.Left >= largerRectangle.Left && rectangle.Right <= largerRectangle.Right &&
            rectangle.Top >= largerRectangle.Top && rectangle.Bottom <= largerRectangle.Bottom;

        public static Rectangle ToRectangle(this Point point) => new Rectangle(0, 0, point.X, point.Y);

        public static bool All<TPixel>(this Image<TPixel> image, Func<TPixel, bool> func)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            for (var x = 0; x < image.Width; x++)
            {
                for (var y = 0; y < image.Height; y++)
                {
                    if (!func(image[x, y]))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static Rectangle GetCoreRegion<TPixel>(this Image<TPixel> image, Func<TPixel, bool> isValid)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            var left = 0;
            var right = 0;
            var top = 0;
            var bottom = 0;
            for (var i = 0; i < image.Width; i++)
            {
                var shouldRemove = true;
                for (var j = 0; j < image.Height; j++)
                {
                    if (isValid(image[i, j]))
                    {
                        shouldRemove = false;
                    }
                }

                if (!shouldRemove)
                {
                    left = i;
                    break;
                }
            }

            for (var i = image.Width - 1; i >= 0; i--)
            {
                var shouldRemove = true;
                for (var j = 0; j < image.Height; j++)
                {
                    if (isValid(image[i, image.Height - j - 1]))
                    {
                        shouldRemove = false;
                    }
                }

                if (!shouldRemove)
                {
                    right = i;
                    break;
                }
            }

            for (var i = 0; i < image.Height; i++)
            {
                var shouldRemove = true;
                for (var j = 0; j < image.Width; j++)
                {
                    if (isValid(image[j, i]))
                    {
                        shouldRemove = false;
                    }
                }

                if (!shouldRemove)
                {
                    top = i;
                    break;
                }
            }

            for (var i = image.Height - 1; i >= 0; i--)
            {
                var shouldRemove = true;
                for (var j = 0; j < image.Width; j++)
                {
                    if (isValid(image[j, i]))
                    {
                        shouldRemove = false;
                    }
                }

                if (!shouldRemove)
                {
                    bottom = i;
                    break;
                }
            }

            return new Rectangle(left, top, right - left + 1, bottom - top + 1);
        }

        public static byte[] Compress(this Image image, int quality) =>
            image.Save(new JpegEncoder
            {
                Quality = quality
            });

        public static byte[] Save(this Image image, IImageFormat format)
        {
            using var ms = new MemoryStream();
            image.Save(ms, format);
            ms.Seek(0, SeekOrigin.Begin);
            return ms.ToArray();
        }

        public static byte[] Save(this Image image, IImageEncoder encoder)
        {
            using var ms = new MemoryStream();
            image.Save(ms, encoder);
            ms.Seek(0, SeekOrigin.Begin);
            return ms.ToArray();
        }

        public static async Task<byte[]> SaveAsync(this Image image, IImageFormat format)
        {
            await using var ms = new MemoryStream();
            await image.SaveAsync(ms, format);
            ms.Seek(0, SeekOrigin.Begin);
            return ms.ToArray();
        }

        public static async Task<byte[]> SaveAsync(this Image image, IImageEncoder encoder)
        {
            await using var ms = new MemoryStream();
            await image.SaveAsync(ms, encoder);
            ms.Seek(0, SeekOrigin.Begin);
            return ms.ToArray();
        }

        public static async Task<byte[]> ToPngDataAsync(this Image image)
        {
            await using var ms = new MemoryStream();
            await image.SaveAsPngAsync(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return ms.ToArray();
        }

        public static async Task<byte[]> ToTiffDataAsync(this Image image)
        {
            await using var ms = new MemoryStream();
            await image.SaveAsTiffAsync(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return ms.ToArray();
        }

        public static async Task<byte[]> ToTiffDataAsync(this Image image, TiffEncoder tiffEncoder)
        {
            await using var ms = new MemoryStream();
            await image.SaveAsTiffAsync(ms, tiffEncoder);
            ms.Seek(0, SeekOrigin.Begin);
            return ms.ToArray();
        }
    }
}