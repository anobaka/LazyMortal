using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Bootstrap.Components.Storage
{
    public class FileUtils
    {
        public static async Task Save(string fullname, Stream stream, FileMode mode = FileMode.Create,
            Encoding encoding = null)
        {

            if (string.IsNullOrEmpty(fullname))
            {
                throw new ArgumentNullException(nameof(fullname));
            }

            var dir = Path.GetDirectoryName(fullname);
            if (string.IsNullOrEmpty(dir))
            {
                throw new ArgumentException();
            }

            Directory.CreateDirectory(dir);
            await using var fs = new FileStream(fullname, mode, FileAccess.Write, FileShare.ReadWrite);
            await stream.CopyToAsync(fs);
        }

        public static async Task Save(string fullname, byte[] data, FileMode mode = FileMode.Create,
            Encoding encoding = null)
        {
            if (string.IsNullOrEmpty(fullname))
            {
                throw new ArgumentNullException(nameof(fullname));
            }

            var dir = Path.GetDirectoryName(fullname);
            if (string.IsNullOrEmpty(dir))
            {
                throw new ArgumentException();
            }

            Directory.CreateDirectory(dir);
            await using var fs = new FileStream(fullname, mode, FileAccess.Write, FileShare.ReadWrite);
            await fs.WriteAsync(data);
        }

        public static async Task Save(string fullname, string content, FileMode mode = FileMode.Create,
            Encoding encoding = null)
        {
            if (string.IsNullOrEmpty(fullname))
            {
                throw new ArgumentNullException(nameof(fullname));
            }

            var dir = Path.GetDirectoryName(fullname);
            if (string.IsNullOrEmpty(dir))
            {
                throw new ArgumentException();
            }

            Directory.CreateDirectory(dir);
            await using var fs = new FileStream(fullname, mode, FileAccess.Write, FileShare.ReadWrite);
            await using var sw = new StreamWriter(fs, encoding ?? Encoding.UTF8);

            await sw.WriteAsync(content);
        }

        public static async Task<string> ReadAsync(string fullname)
        {
            await using var fs = new FileStream(fullname, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var sr = new StreamReader(fs);
            return await sr.ReadToEndAsync();
        }

        public static async Task CopyAsync(string sourcePath, string destinationPath)
        {
            await using Stream source = File.OpenRead(sourcePath);
            await using Stream destination = File.Create(destinationPath);
            await source.CopyToAsync(destination);
        }
    }
}