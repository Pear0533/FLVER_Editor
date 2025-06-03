using System;

namespace SoulsFormats.Exceptions
{
    /// <summary>
    /// Throw when the Oodle library could not be found.
    /// </summary>
    public class NoOodleFoundException : Exception
    {
        /// <summary>
        /// Creates a new <see cref="NoOodleFoundException"/>.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public NoOodleFoundException(string message) : base(message) { }
    }
}
