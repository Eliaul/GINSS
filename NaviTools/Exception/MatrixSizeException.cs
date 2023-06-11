using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaviTools
{
    [Serializable]
    public class MatrixSizeException : Exception
    {
        public MatrixSizeException() : base("Matrix's size is invalid!") { }

        public MatrixSizeException(string? message) : base(message) { }

        public MatrixSizeException(string? message, Exception? innerException)
            : base(message, innerException) { }

        public MatrixSizeException(string? paraName, int row, int col)
            : base($"Matrix {paraName}'s size(size = {row}×{col}) is invalid!")
        { }
    }
}
