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
            Types.ForEach(t =>
            {
                var values = Enum.GetValues(t);
                var keyToValue = string.Join(", ", values.Cast<object>().Select(a => $"{a}: {(int) a}"));
                var ktvDefinition = $"export const {t.Name} = {{{keyToValue}}};";
                var valueToKey = string.Join(", ", values.Cast<object>().Select(a => $"{(int) a}: \"{a}\""));
                var kvtDefinition = $"export const {t.Name}Label = {{{valueToKey}}};";
                var listDefinition =
                    $"export const {t.Name.Camelize().Pluralize()} = Object.keys({t.Name}).map(t => ({{label: t, value: {t.Name}[t]}}));";
                codes.Add(ktvDefinition);
                codes.Add(kvtDefinition);
                codes.Add(listDefinition);
            });
            return string.Join(Environment.NewLine, codes);
        }
    }
}