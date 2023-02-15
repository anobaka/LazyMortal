using Humanizer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Bootstrap.Components.Configuration.Abstractions;
using Bootstrap.Extensions;

namespace Bootstrap.Components.Configuration.Helpers
{
    public class ConfigurationUtils
    {
        private static readonly string[] BadTrailingSet = new[]
        {
            "option",
            "config",
            "configuration",
        }.SelectMany(a => new[] { a.Pluralize(), a, a.Titleize(), a.Titleize().Pluralize() }).ToHashSet().ToArray();

        public static OptionsDescriber GetOptionsDescriber<TOptions>(string rootPath)
        {
            var type = SpecificTypeUtils<TOptions>.Type;
            return GetOptionsDescriber(type, rootPath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="rootPath">
        /// </param>
        /// <returns></returns>
        public static OptionsDescriber GetOptionsDescriber(Type type, string rootPath)
        {
            var attr = type.GetCustomAttribute<OptionsAttribute>();
            var fileKey = attr?.FileKey;
            var optionsKey = attr?.OptionsKey ?? type.Name!.TrimEnd(BadTrailingSet);
            if (string.IsNullOrEmpty(fileKey))
            {
                fileKey = string.Join('-', optionsKey.Humanize(LetterCasing.LowerCase).Split(' '));
            }

            return new OptionsDescriber
            {
                FileKey = fileKey,
                OptionsKey = optionsKey,
                OptionsType = type,
                FilePath = Path.GetFullPath(Path.Combine(rootPath, $"{fileKey}{OptionsConstraints.FileExt}"))
            };
        }
    }
}