using System.IO;
using System.Threading.Tasks;

namespace AdAwayHost.Shared.Wrappers
{
  public class FileWrapper : IFileWrapper
  {
    public async Task WriteAllTextAsync(string pathToFile, string fileContent) =>
      await File.WriteAllTextAsync(pathToFile, fileContent);

    public async Task<string> ReadAllTextAsync(string pathToFile) =>
      await File.ReadAllTextAsync(pathToFile);

    public bool Exists(string pathToFile) => File.Exists(pathToFile);

    public void Create(string pathToHostsFile) =>
      File.Create(pathToHostsFile).Dispose();
  }
}
