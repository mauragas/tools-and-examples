using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;

namespace AdAwayHost.Tests
{
  public class TestHelpers
  {
    private static readonly EmbeddedFileProvider EmbeddedFileProvider =
      new(Assembly.GetExecutingAssembly());

    public static async Task<string> GetEmbeddedFileAsync(string relativePathToFile)
    {
      var fileContent = string.Empty;
      using (var stream = EmbeddedFileProvider.GetFileInfo(relativePathToFile).CreateReadStream())
      using (var reader = new StreamReader(stream))
      {
        fileContent = await reader.ReadToEndAsync();
      }
      return fileContent;
    }
  }
}
