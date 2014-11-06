namespace Lisa.Dobble
{
    public interface IPathService
    {
        string CreateDocumentsPath(string fileName);
        string CreateResourcePath(string fileName);
    }
}
