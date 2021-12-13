using System;

namespace Bootstrap.Models.Exceptions
{
    public class NotInitializedException : Exception
    {
        public string Type { get; }

        public NotInitializedException(string type, string message = null) : base($"[{type}] not initialized. {message}".Trim())
        {
            Type = type;
        }
    }
}