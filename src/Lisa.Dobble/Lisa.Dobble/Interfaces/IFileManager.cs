using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamarin.Forms.Labs.Services.IO
{
    public interface IFileManager
    {
        bool DirectoryExists(string path);

        void CreateDirectory(string path);

        Stream OpenFile(string path, FileMode mode, FileAccess access);

        bool FileExists(string path);

        DateTimeOffset GetLastWriteTime(string path);
    }

    public enum FileMode
    {
        CreateNew = 1,
        Create = 2,
        Open = 3,
        OpenOrCreate = 4,
        Truncate = 5,
        Append = 6,
    }

    public enum FileAccess
    {
        Read = 1,
        Write = 2,
        ReadWrite = 3,
    }
}