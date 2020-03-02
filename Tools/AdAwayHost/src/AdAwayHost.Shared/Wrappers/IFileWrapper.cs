using System.Threading.Tasks;

namespace AdAwayHost.Shared.Wrappers
{
    public interface IFileWrapper
    {
        Task WriteAllTextAsync(string pathToFile, string fileContent);
        Task<string> ReadAllTextAsync(string pathToFile);
        bool Exists(string pathToFile);
        void Create(string pathToHostsFile);
    }
}