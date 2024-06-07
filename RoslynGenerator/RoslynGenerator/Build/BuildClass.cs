namespace RoslynGenerator.Build;


public static class BuildClass {
  public static string GenerateClassCode(ClassInfo classInfo) {
    var namespaceDeclaration = CodeGenerationHelper.GenerateNamespace(classInfo.Namespace);
    var usings = CodeGenerationHelper.GenerateUsings(classInfo.Usings);

    var classAttributes = CodeGenerationHelper.GenerateAttributes(classInfo.Attributes);

    var classDeclaration = SyntaxFactory.ClassDeclaration(classInfo.ClassName)
        .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
        .AddAttributeLists(classAttributes.ToArray());

    var baseList = CodeGenerationHelper.GenerateBaseList(classInfo.BaseClass, classInfo.ImplementedInterfaces);
    classDeclaration = classDeclaration.WithBaseList(baseList);

    foreach (var member in classInfo.Members) {
      var propertyDeclaration = CodeGenerationHelper.GenerateProperty(member.Type, member.Name, member.Attributes, member.DefaultValue);
      classDeclaration = classDeclaration.AddMembers(propertyDeclaration);
    }

    namespaceDeclaration = namespaceDeclaration.AddMembers(classDeclaration);

    var compilationUnit = SyntaxFactory.CompilationUnit()
        .AddUsings(usings.ToArray())
        .AddMembers(namespaceDeclaration)
        .NormalizeWhitespace();

    return compilationUnit.ToFullString();
  }
}
