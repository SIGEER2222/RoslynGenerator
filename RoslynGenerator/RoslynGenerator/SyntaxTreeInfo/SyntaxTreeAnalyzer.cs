using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace RoslynGenerator.SyntaxTreeInfo;

public class SyntaxTreeAnalyzer {
  public ClassInfo AddRequird(ClassInfo info) {
    foreach (var item in info.Members) {
      if (item.DefaultValue?.Contains("null!") is true) {
        item.Attributes.Add("Required");
      }
    }
    return info;
  }
  public ClassInfo Analyze(SyntaxTree syntaxTree) {
    CompilationUnitSyntax root = syntaxTree.GetCompilationUnitRoot();

    string @namespace = root.DescendantNodes().OfType<NamespaceDeclarationSyntax>().FirstOrDefault()?.Name.ToString();

    List<string> usings = root.DescendantNodes().OfType<UsingDirectiveSyntax>().Select(u => u.Name.ToString()).ToList();

    List<string> classAttributes = root.DescendantNodes().OfType<ClassDeclarationSyntax>()
        .SelectMany(c => c.AttributeLists.SelectMany(a => a.Attributes.Select(attr => attr.ToString())))
        .ToList();

    string className = root.DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault()?.Identifier.ToString();

    List<ClassMember> members = root.DescendantNodes().OfType<ClassDeclarationSyntax>()
        .SelectMany(c => c.Members.Select(m => ExtractClassMemberInfo(m)))
        .ToList();

    ClassInfo classInfo = new ClassInfo {
      Namespace = @namespace,
      Usings = usings,
      Attributes = classAttributes,
      ClassName = className,
      Members = members
    };

    return classInfo;
  }

  private ClassMember ExtractClassMemberInfo(MemberDeclarationSyntax memberSyntax) {
    string memberName = memberSyntax switch {
      PropertyDeclarationSyntax propertySyntax => propertySyntax.Identifier.ToString(),
      _ => string.Empty
    };
    string memberType = memberSyntax switch {
      FieldDeclarationSyntax fieldSyntax => fieldSyntax.Declaration.Type.ToString(),
      PropertyDeclarationSyntax propertySyntax => propertySyntax.Type.ToString(),
      MethodDeclarationSyntax methodSyntax => methodSyntax.ReturnType.ToString(),
      _ => throw new NotSupportedException($"Unsupported member type: {memberSyntax.Kind()}")
    };

    List<string> memberAttributes = memberSyntax.AttributeLists
        .SelectMany(a => a.Attributes.Select(attr => attr.ToString()))
        .ToList();

    string defaultValue = memberSyntax switch {
      FieldDeclarationSyntax fieldSyntax => fieldSyntax.Declaration.Variables.FirstOrDefault()?.Initializer?.Value?.ToString(),
      PropertyDeclarationSyntax propertySyntax => propertySyntax.Initializer?.Value?.ToString(),
      _ => null
    };

    ClassMember classMember = new ClassMember {
      Name = memberName,
      Type = memberType,
      Attributes = memberAttributes,
      DefaultValue = defaultValue
    };

    return classMember;
  }
}
