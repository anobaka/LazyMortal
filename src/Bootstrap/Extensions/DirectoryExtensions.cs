using System.IO;
using System.Linq;

namespace Bootstrap.Extensions
{
    public static class DirectoryExtensions
    {
        public static long GetSize(this DirectoryInfo d)
        {
            // Files
            var fis = d.GetFiles();
            var size = fis.Sum(fi => fi.Length);
            // Sub directories
            var dis = d.GetDirectories();
            return dis.Aggregate(size, (current, di) => current + GetSize(di));
        }
    }
}