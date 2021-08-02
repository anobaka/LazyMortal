using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bootstrap.Components.Mobiles.Android.Infrastructures;
using Bootstrap.Components.Mobiles.Android.Models;
using Bootstrap.Extensions;

namespace Bootstrap.Components.Mobiles.Android.Wrappers.Shell
{
    public class AdbPm : AdbWrapper
    {
        public AdbPm(AdbShellWrapper prev) : base(prev, "pm")
        {
        }

        public async Task<List<AdbPackage>> ListPackages(string arguments = null)
        {
            var output = await Execute($"list packages {arguments}");
            var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries).Where(a => a.IsNotEmpty())
                .Select(a => a.Trim()["package:".Length..]).Select(a => a.Split('='));
            var packages = lines.Select(a => new AdbPackage
            {
                AssociatedFile = a.Length > 1 ? a[0] : null,
                Name = a.Length > 1 ? a[1] : a[0]
            }).ToList();
            return packages;
        }
    }
}