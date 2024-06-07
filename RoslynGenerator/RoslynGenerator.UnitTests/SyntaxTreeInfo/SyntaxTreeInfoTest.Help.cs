namespace RoslynGenerator.UnitTests.SyntaxTreeInfo;
public partial class SyntaxTreeInfoTest {
  ClassInfo classInfo => GetClassInfo();
  ClassInfo GetClassInfo() {
    var file = @"RoslynGenerator\RoslynGenerator.FileHelp\Class\Entity\FabLotTransSetting.cs";
    var syntaxTree = CSharpSyntaxTree.ParseText(ScribanHelp.GetTextByRelativePaths(file));
    SyntaxTreeAnalyzer analyzer = new SyntaxTreeAnalyzer();
    ClassInfo classInfo = analyzer.Analyze(syntaxTree);
    classInfo = analyzer.AddRequird(classInfo);
    classInfo.Namespace = "TestNamespace";
    return classInfo;
  }
}
