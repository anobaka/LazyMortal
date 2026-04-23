using System;
using System.Collections.Generic;
using System.Linq;
using Humanizer;

namespace Bootstrap.Components.Miscellaneous
{
    public static class ConstantsGenerator
    {
        public static string Generate(IEnumerable<Type> types)
        {
            var blocks = new List<string>();
            var nl = Environment.NewLine;

            foreach (var t in types.Where(t => t.IsPublic && t.IsEnum))
            {
                try
                {
                    var values = Enum
                        .GetValues(t)
                        .Cast<object>()
                        .OrderBy(v => (int) v)
                        .ToList();

                    var enumMembers = string.Join("," + nl,
                        values.Select(v => $"  {v} = {(int) v}"));
                    var enumBlock = $"export enum {t.Name} {{{nl}{enumMembers}{nl}}}";

                    var optionsVar = t.Name.Camelize().Pluralize();
                    var optionItems = string.Join("," + nl,
                        values.Select(v => $"  {{ label: '{v}', value: {t.Name}.{v} }}"));
                    var optionsBlock = $"export const {optionsVar} = [{nl}{optionItems}{nl}] as const;";

                    var labelEntries = string.Join("," + nl,
                        values.Select(v => $"  [{t.Name}.{v}]: '{v}'"));
                    var labelBlock = $"export const {t.Name}Label: Record<{t.Name}, string> = {{{nl}{labelEntries}{nl}}};";

                    blocks.Add(string.Join(nl + nl, new[] {enumBlock, optionsBlock, labelBlock}));
                }
                catch
                {
                    // ignore enum generation errors for this type
                }
            }

            var header = string.Join(nl, new[]
            {
                "// AUTO-GENERATED FROM server enums. Do not edit manually.",
                "/* eslint-disable */"
            });

            return header + nl + nl + string.Join(nl + nl, blocks);
        }
    }
}
