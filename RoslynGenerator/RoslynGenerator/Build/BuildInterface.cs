namespace RoslynGenerator.Build;
public class BuildInterface {
  public static string GenerateInterfaceCode(ClassInfo classInfo) {
    var namespaceDeclaration = CodeGenerationHelper.GenerateNamespace(classInfo.Namespace);
    var usings = CodeGenerationHelper.GenerateUsings(classInfo.Usings);

    var interfaceDeclaration = SyntaxFactory.InterfaceDeclaration("I" + classInfo.ClassName)
        .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

    var baseList = CodeGenerationHelper.GenerateBaseList(classInfo.BaseClass, classInfo.ImplementedInterfaces);
    interfaceDeclaration = interfaceDeclaration.WithBaseList(baseList);

    foreach (var member in classInfo.Members) {
      if (string.IsNullOrWhiteSpace(member.Name) || string.IsNullOrWhiteSpace(member.Type))
        continue;

      var propertyDeclaration = SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName(member.Type), member.Name)
          .AddAccessorListAccessors(
              SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
              SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)));

      interfaceDeclaration = interfaceDeclaration.AddMembers(propertyDeclaration);
    }

    namespaceDeclaration = namespaceDeclaration.AddMembers(interfaceDeclaration);

    var compilationUnit = SyntaxFactory.CompilationUnit()
        .AddUsings(usings.ToArray())
        .AddMembers(namespaceDeclaration)
        .NormalizeWhitespace();

    return compilationUnit.ToFullString();
  }
}
