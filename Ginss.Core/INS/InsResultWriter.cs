using NaviTools.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ginss.Core.INS
{
    public class InsResultWriter : FileStreamWriter
    {

        public StateOfEpoch? State { get; set; }

        public InsResultWriter(string filePath) : base(filePath)
        { }

        public void WriteLine(StateOfEpoch state)
        {
            State = state;
            WriteLine();
        }

        protected override string Line()
        {
            if (State == null)
            {
                return "";
            }
            else
            {
                return string.Format("{0:degval}", State);
            }
        }
    }
}
