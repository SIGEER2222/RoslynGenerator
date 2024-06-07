using Dumpify;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using RoslynGenerator.FileHelp;
using Scriban;
using SourceGeneratorQuery;

namespace RoslynGenerator.UnitTests.DDD;
public class RepostoryGenerator {

  [Fact]
  public async Task SourceFileTest() {
    var file = @"RoslynGenerator\RoslynGenerator.FileHelp\Class\Entity\FabLotTransSetting.cs";
    var compilation = CSharpCompilation.Create("MyCompilation",
        options: new CSharpCompilationOptions(OutputKind.ConsoleApplication,
                optimizationLevel: OptimizationLevel.Release,
                assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default),
        syntaxTrees: new[] { CSharpSyntaxTree.ParseText(ScribanHelp.GetTextByRelativePaths(file)) },
        references: new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });
    var sourceFiles= compilation.SyntaxTrees.Select(x => new SourceFile(x, string
        .Empty));

    var namespaces = sourceFiles.Select(x=>x.Namespaces).ToList();
    var usings = sourceFiles.Select(x=>x.Usings).ToList();
    var syntextTrees = sourceFiles.Select(x => x.SyntaxTree).ToList();
    var classs = sourceFiles.Select(x => x.GetAllClasses()).ToList();
  }

  [Fact]
  public async Task GenerateRepository() {
    var model = new {
      Namespace = "TestNamespace",
      tool_name = "RecastCSharp",
      tool_version = "1.0.0",
      Records = new List<object>
     {
        new
        {
          Name = "LogLevel",
          Properties = new List<object>
          {
            new { JsonName = "Default", Type = "string", Name = "Default" },
            new { JsonName = "System", Type = "string", Name = "System" },
            new { JsonName = "Microsoft", Type = "string", Name = "Microsoft" }
          }
        }
      }
    };

    var templateContent = ScribanHelp.GetTextByRelativePaths(@"RoslynGenerator\RoslynGenerator.FileHelp\Scriban\JsonToRecord\JsonToRecord.scriban");
    var template = Template.Parse(templateContent);
    await Verify(async () => await template.RenderAsync(model));

    var expectedCode = @"using ManuTalent.Mom.Semi.FrontEnd.EntityFrameworkCore.Models;
using Microsoft.EntityFrameworkCore;

namespace ManuTalent.Mom.Semi.FrontEnd.EntityFrameworkCore.Repositories;

public class FabLotTransSettingRepository : Repository<FabLotTransSetting>, IFabLotTransSettingRepository
{
    public FabLotTransSettingRepository(DbContext dbContext) : base(dbContext)
    {
    }
}";
  }
}

internal class Entity {
  private string v;

  public Entity(string v) {
    this.v = v;
  }
}
