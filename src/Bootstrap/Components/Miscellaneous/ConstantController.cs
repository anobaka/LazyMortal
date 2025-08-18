using System;
using System.Collections.Generic;
using System.Linq;
using Humanizer;
using Microsoft.AspNetCore.Mvc;

namespace Bootstrap.Components.Miscellaneous
{
    [Route("~/api/constant")]
    public abstract class ConstantController : Controller
    {
        protected abstract List<Type> Types { get; }

        [HttpGet]
        public string GetAll()
        {
            var blocks = new List<string>();
            var nl = Environment.NewLine;

            Types.Where(t => t.IsPublic && t.IsEnum).ToList().ForEach(t =>
            {
                try
                {
                    var values = Enum
                        .GetValues(t)
                        .Cast<object>()
                        .OrderBy(v => (int)v)
                        .ToList();

                    // enum block
                    var enumMembers = string.Join("," + nl,
                        values.Select(v => $"  {v} = {(int)v}"));
                    var enumBlock = $"export enum {t.Name} {{{nl}{enumMembers}{nl}}}";

                    // options array (typed, stable order)
                    var optionsVar = t.Name.Camelize().Pluralize();
                    var optionItems = string.Join("," + nl,
                        values.Select(v => $"  {{ label: '{v}', value: {t.Name}.{v} }}"));
                    var optionsBlock = $"export const {optionsVar} = [{nl}{optionItems}{nl}] as const;";

                    // label map with type safety
                    var labelEntries = string.Join("," + nl,
                        values.Select(v => $"  [{t.Name}.{v}]: '{v}'"));
                    var labelBlock = $"export const {t.Name}Label: Record<{t.Name}, string> = {{{nl}{labelEntries}{nl}}};";

                    // compose block per enum, separated by a blank line
                    blocks.Add(string.Join(nl + nl, new[] { enumBlock, optionsBlock, labelBlock }));
                }
                catch
                {
                    // ignore enum generation errors for this type
                }
            });

            // header + all blocks
            var header = string.Join(nl, new[]
            {
                "// AUTO-GENERATED FROM server enums. Do not edit manually.",
                "/* eslint-disable */"
            });

            return header + nl + nl + string.Join(nl + nl, blocks);
        }
    }
}