using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaviTools.IO
{
    public interface IFileStreamLineReadable<T>
    {
        public T? LineParse(string line);

    }
}
