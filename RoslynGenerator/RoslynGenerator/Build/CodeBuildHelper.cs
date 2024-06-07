using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoslynGenerator.Build;
public static class CodeGenerationHelper {
  public static NamespaceDeclarationSyntax GenerateNamespace(string namespaceName) {
    if (string.IsNullOrWhiteSpace(namespaceName))
      throw new ArgumentNullException(nameof(namespaceName));

    return SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(namespaceName))
        .NormalizeWhitespace();
  }

  public static SyntaxList<UsingDirectiveSyntax> GenerateUsings(List<string> usings) {
    var usingList = SyntaxFactory.List<UsingDirectiveSyntax>();
    foreach (var u in usings) {
      if (!string.IsNullOrWhiteSpace(u)) {
        usingList = usingList.Add(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(u)));
      }
    }
    return usingList;
  }

  public static BaseListSyntax GenerateBaseList(string baseClassName, List<string> interfaceNames) {
    var baseList = SyntaxFactory.BaseList();
    if (baseClassName is not null)
      baseList = baseList.AddTypes(SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(baseClassName)));
    if (interfaceNames is not null)
      foreach (var i in interfaceNames) {
        baseList = baseList.AddTypes(SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(i)));
      }
    return baseList;
  }

  public static SyntaxList<AttributeListSyntax> GenerateAttributes(List<string> attributes) {
    var attributeList = new SyntaxList<AttributeListSyntax>();
    foreach (var attr in attributes) {
      attributeList = attributeList.Add(
          SyntaxFactory.AttributeList(
              SyntaxFactory.SingletonSeparatedList(
                  SyntaxFactory.Attribute(SyntaxFactory.ParseName(attr)))));
    }

    return attributeList;
  }

  public static PropertyDeclarationSyntax GenerateProperty(string type, string name, List<string> attributes, string defaultValue = null) {
    var propertyDeclaration = SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName(type), name)
        .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
        .AddAccessorListAccessors(
            SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
            SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)))
        .AddAttributeLists(GenerateAttributes(attributes).ToArray());

    if (!string.IsNullOrEmpty(defaultValue)) {
      propertyDeclaration = propertyDeclaration.WithInitializer(
          SyntaxFactory.EqualsValueClause(SyntaxFactory.ParseExpression(defaultValue)))
          .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
    }

    return propertyDeclaration;
  }
}
