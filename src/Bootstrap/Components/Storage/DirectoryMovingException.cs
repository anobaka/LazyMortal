using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace Bootstrap.Components.Storage;

public class DirectoryMovingException : Exception
{
    public string[]? MissingFiles { get; init; } = null;
    public string[]? ConflictFiles { get; init; } = null;

    public override string Message
    {
        get
        {
            var sb = new StringBuilder();
            const int sampleCount = 10;
            if (MissingFiles?.Any() == true)
            {
                sb.AppendLine("Missing files:");
                foreach (var file in MissingFiles.Take(sampleCount))
                {
                    sb.AppendLine(file);
                }
            }

            if (ConflictFiles?.Any() == true)
            {
                sb.AppendLine("Conflict files:");
                foreach (var file in ConflictFiles.Take(sampleCount))
                {
                    sb.AppendLine(file);
                }
            }

            return sb.ToString();
        }
    }
}