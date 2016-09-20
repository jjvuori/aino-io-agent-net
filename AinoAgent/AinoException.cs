using System;

namespace Aino
{
    public class AinoException : Exception
    {
        public AinoException(string message) : base(message)
        {
        }

        public AinoException(string message, Exception e) : base(message, e)
        {
        }
    }
}
