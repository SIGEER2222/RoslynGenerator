using RoslynGenerator.Build;

namespace RoslynGenerator.UnitTests.SyntaxTreeInfo {
  public partial class SyntaxTreeInfoTest {
    private readonly ITestOutputHelper output;

    public SyntaxTreeInfoTest(ITestOutputHelper output) {
      this.output = output;
    }

    [Fact]
    public async Task GenerateRepositoryCodeTest() {
      // var buildRepository = BuildRepository.GenerateRepositoryCode(classInfo);
      // await Verify(buildRepository);
    }

    [Fact]
    public async Task GenerateInterfaceCodeTest() {
      var classInfo = this.classInfo;
      classInfo.ImplementedInterfaces = new List<string> { "ISemiFrontEndEntity" };
      var buildInterface = BuildInterface.GenerateInterfaceCode(classInfo);
      await Verify(buildInterface);
    }

    [Fact]
    public async Task GenerateClassCodeTest() {
      var classInfo = this.classInfo;
      classInfo.BaseClass = "SemiFrontEndEntity";
      classInfo.ImplementedInterfaces = new List<string> { "I" + classInfo.ClassName };
      var buildClass = BuildClass.GenerateClassCode(classInfo);
      await Verify(buildClass);
    }

    [Fact]
    public async Task Run() {
      await Verify(classInfo);
    }
  }
}
