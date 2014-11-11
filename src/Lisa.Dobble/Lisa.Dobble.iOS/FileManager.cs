using Lisa.Dobble;
using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Labs.Services.IO;

[assembly: Dependency(typeof(Xamarin.Forms.Labs.Services.IO.FileManager))]
namespace Xamarin.Forms.Labs.Services.IO
{
    public class FileManager : Xamarin.Forms.Labs.Services.IO.IFileManager
    {
        public bool DirectoryExists(string path)
        {
            string fullPath = CreateFullPath(path);
            return Directory.Exists(fullPath);
        }

        public void CreateDirectory(string path)
        {
            string fullPath = CreateFullPath(path);
            Directory.CreateDirectory(fullPath);
        }

        public Stream OpenFile(string path, Xamarin.Forms.Labs.Services.IO.FileMode mode, Xamarin.Forms.Labs.Services.IO.FileAccess access)
        {
            string fullPath = CreateFullPath(path);
            return File.Open(fullPath, TranslateFileMode(mode), TranslateFileAccess(access));
        }

        public Stream OpenFile(string path, Xamarin.Forms.Labs.Services.IO.FileMode mode, Xamarin.Forms.Labs.Services.IO.FileAccess access, Xamarin.Forms.Labs.Services.IO.FileShare fileShare)
        {
            string fullPath = CreateFullPath(path);
            return File.Open(fullPath, TranslateFileMode(mode), TranslateFileAccess(access), TranslateFileShare(fileShare));
        }

        public bool FileExists(string path)
        {
            string fullPath = CreateFullPath(path);
            return File.Exists(fullPath);
        }

        public DateTimeOffset GetLastWriteTime(string path)
        {
            string fullPath = CreateFullPath(path);
            return File.GetLastWriteTime(fullPath);
        }

        private string CreateFullPath(string path)
        {
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            return String.Format("{0}/{1}", folder, path);
        }

        private System.IO.FileMode TranslateFileMode(Xamarin.Forms.Labs.Services.IO.FileMode mode)
        {
            switch (mode)
            {
                case Xamarin.Forms.Labs.Services.IO.FileMode.CreateNew: return System.IO.FileMode.CreateNew;
                case Xamarin.Forms.Labs.Services.IO.FileMode.Create: return System.IO.FileMode.Create;
                case Xamarin.Forms.Labs.Services.IO.FileMode.Open: return System.IO.FileMode.Open;
                case Xamarin.Forms.Labs.Services.IO.FileMode.OpenOrCreate: return System.IO.FileMode.OpenOrCreate;
                case Xamarin.Forms.Labs.Services.IO.FileMode.Truncate: return System.IO.FileMode.Truncate;
                case Xamarin.Forms.Labs.Services.IO.FileMode.Append: return System.IO.FileMode.Append;
                default: throw new NotImplementedException();
            }
        }

        private System.IO.FileAccess TranslateFileAccess(Xamarin.Forms.Labs.Services.IO.FileAccess access)
        {
            switch (access)
            {
                case Xamarin.Forms.Labs.Services.IO.FileAccess.Read: return System.IO.FileAccess.Read;
                case Xamarin.Forms.Labs.Services.IO.FileAccess.Write: return System.IO.FileAccess.Write;
                case Xamarin.Forms.Labs.Services.IO.FileAccess.ReadWrite: return System.IO.FileAccess.ReadWrite;
                default: throw new NotImplementedException();
            }
        }
        private System.IO.FileShare TranslateFileShare(Xamarin.Forms.Labs.Services.IO.FileShare share)
        {
            switch (share)
            {
                case Xamarin.Forms.Labs.Services.IO.FileShare.Delete: return System.IO.FileShare.Delete;
                case Xamarin.Forms.Labs.Services.IO.FileShare.Inheritable: return System.IO.FileShare.Inheritable;
                case Xamarin.Forms.Labs.Services.IO.FileShare.None: return System.IO.FileShare.None;
                case Xamarin.Forms.Labs.Services.IO.FileShare.Read: return System.IO.FileShare.Read;
                case Xamarin.Forms.Labs.Services.IO.FileShare.ReadWrite: return System.IO.FileShare.ReadWrite;
                case Xamarin.Forms.Labs.Services.IO.FileShare.Write: return System.IO.FileShare.Write;
                default: throw new NotImplementedException();
            }
        }
    }
}