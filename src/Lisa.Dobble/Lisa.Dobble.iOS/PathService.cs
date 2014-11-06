using MonoTouch.Foundation;
using System;
using System.IO;
using Xamarin.Forms;

[assembly: Dependency(typeof(Lisa.Dobble.iOS.PathService))]
namespace Lisa.Dobble.iOS
{

    public class PathService : IPathService
    {
        public string CreatePersonalPath(string fileName)
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var filePath = Path.Combine(documents, fileName);
            return filePath;
        }

        public string CreateResourcePath(string fileName)
        {
            return fileName;
        }
    }
}
