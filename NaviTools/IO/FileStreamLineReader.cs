using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaviTools.IO
{
    public abstract class FileStreamLineReader<T> : IFileStreamLineReadable<T>
    {

        protected string FilePath;

        protected int FileHead;

        protected FileStreamLineReader(string filePath, int fileHead)
        {
            FilePath = filePath;
            FileHead = fileHead;
        }

        public IEnumerable<T> Read()
        {
            using FileStream fileStream = new(FilePath, FileMode.Open, FileAccess.Read);
            using StreamReader reader = new(fileStream);
            for (int i = 0; i < FileHead; i++)
            {
                if (!reader.EndOfStream)
                    reader.ReadLine();
            }
            while (!reader.EndOfStream)
            {
                string? line = reader.ReadLine();
                if (line == null)
                    continue;
                T? res = LineParse(line);
                if (res == null)
                    continue;
                yield return res;
            }
            yield break;
        }

        public abstract T? LineParse(string line);
    }
}
