using System;

namespace Predictly_Api.Helpers
{
    public class HumanErrorException : Exception
    {
        public HumanErrorException(string message, string details = "") : base(message)
        {
            Details = details;
        }

        public object Details { get; set; } = string.Empty;
    }
}
