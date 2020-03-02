using System.IO;
using System.Threading.Tasks;

namespace AdAwayHost.Shared.Wrappers
{
    public class FileWrapper : IFileWrapper
    {
        public async Task WriteAllTextAsync(string pathToFile, string fileContent)
        {
            await File.WriteAllTextAsync(pathToFile, fileContent);
        }

        public async Task<string> ReadAllTextAsync(string pathToFile)
        {
            return await File.ReadAllTextAsync(pathToFile);
        }

        public bool Exists(string pathToFile)
        {
            return File.Exists(pathToFile);
        }

        public void Create(string pathToHostsFile)
        {
            File.Create(pathToHostsFile).Dispose();
        }
    }
}