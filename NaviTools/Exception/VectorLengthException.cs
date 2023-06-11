using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaviTools
{
    [Serializable]
    public class VectorLengthException : Exception
    {
        public VectorLengthException() : base("Vector's length is invalid!") { }

        public VectorLengthException(string? message) : base(message) { }

        public VectorLengthException(string? message, Exception? innerException)
            : base(message, innerException) { }

        public VectorLengthException(string? paraName, int errorLen)
            : base($"Vector {paraName}'s length(length = {errorLen}) is invalid!")
        {

        }

    }
}
