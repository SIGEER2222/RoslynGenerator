using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynGenerator.FileHelp;

public static partial class ScribanHelp {
  public static string GetTextByRelativePaths(string file) => File.ReadAllText(RoslynGeneratorFileHelp.GetFilePathByRelativePaths(file));
}

public static partial class RoslynGeneratorFileHelp {
  public static string RootPath => GetFilePath.GetRootPath("RoslynGenerator");
  public static string GetFilePathByRelativePaths(string file) => Path.Combine(RootPath, file);
}

internal partial class GetFilePath {
  public static string GetRootPath(string slnName) {
    string folderPath = AppDomain.CurrentDomain.BaseDirectory.Split(slnName).First();
    string searchPattern = slnName + ".sln";
    DirectoryInfo dirInfo = new DirectoryInfo(folderPath);

    var foundPath = dirInfo.GetFiles(searchPattern, SearchOption.AllDirectories).FirstOrDefault();

    string directoryPath = Path.GetDirectoryName(foundPath.FullName);
    string parentDirectoryPath = Path.GetDirectoryName(directoryPath);

    return parentDirectoryPath;
  }
}
