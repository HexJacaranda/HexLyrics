using System;
using System.Collections.Generic;
using System.Text;

namespace HexLyrics.Interfaces
{

    [Serializable]
    public class IllegalContentException : Exception
    {
        public IllegalContentException() { }
        public IllegalContentException(string message) : base(message) { 

        }
        public IllegalContentException(string message, Exception inner) : base(message, inner) { 
        }
        protected IllegalContentException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
