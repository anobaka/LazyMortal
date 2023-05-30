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
            var codes = new List<string>();
            Types.Where(a => a.IsPublic).ToList().ForEach(t =>
            {
                try
                {
                    var values = Enum.GetValues(t);
                    var keyToValue = string.Join(", ", values.Cast<object>().Select(a => $"{a} = {(int) a}"));
                    var ktvDefinition = $"export enum {t.Name} {{{keyToValue}}}";
                    codes.Add(ktvDefinition);
                    // var valueToKey = string.Join(", ", values.Cast<object>().Select(a => $"{(int) a}: \"{a}\""));
                    // var kvtDefinition = $"export const {t.Name}Label = {{{valueToKey}}};";
                    // codes.Add(kvtDefinition);
                    var listDefinition =
                        $"export const {t.Name.Camelize().Pluralize()} = Object.keys({t.Name}).filter(k => typeof {t.Name}[k] === 'number').map(t => ({{label: t, value: {t.Name}[t]}}));";
                    codes.Add(listDefinition);
                }
                catch (Exception e)
                {

                }
            });
            return string.Join(Environment.NewLine, codes);
        }
    }
}