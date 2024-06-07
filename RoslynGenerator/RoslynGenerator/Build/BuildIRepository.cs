namespace RoslynGenerator.Build;
public class BuildIRepository {
  public static string GenerateRepositoryInterface(ClassInfo classInfo, string repName, string interfaceName, string genericTypeName) {
    var genericName = SyntaxFactory.GenericName(SyntaxFactory.Identifier(interfaceName))
        .WithTypeArgumentList(
            SyntaxFactory.TypeArgumentList(
                SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                    SyntaxFactory.IdentifierName(genericTypeName))));

    var interfaceSyntax = SyntaxFactory.SimpleBaseType(genericName);

    var classDeclaration = SyntaxFactory.ClassDeclaration(repName)
        .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
        .AddBaseListTypes(interfaceSyntax);

    var namespaceDeclaration = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName(classInfo.Namespace))
        .AddMembers(classDeclaration);

    var compilationUnit = SyntaxFactory.CompilationUnit()
        .AddMembers(namespaceDeclaration)
        .NormalizeWhitespace();

    return compilationUnit.ToFullString();
  }

  public static string GenerateRepository(
            string className,
            string baseClassName,
            string interfaceName,
            string genericTypeName,
            List<string> parameterTypes,
            List<string> parameterNames) {
    var baseClassSyntax = SyntaxFactory.SimpleBaseType(
        SyntaxFactory.ParseTypeName($"{baseClassName}<{genericTypeName}>"));

    var interfaceSyntax = SyntaxFactory.SimpleBaseType(
        SyntaxFactory.ParseTypeName(interfaceName));

    var parameters = SyntaxFactory.ParameterList(
        SyntaxFactory.SeparatedList(
            parameterTypes.Zip(parameterNames, (type, name) =>
                SyntaxFactory.Parameter(SyntaxFactory.Identifier(name))
                    .WithType(SyntaxFactory.ParseTypeName(type)))));

    var constructor = SyntaxFactory.ConstructorDeclaration(className)
        .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
        .WithParameterList(parameters)
        .WithInitializer(
            SyntaxFactory.ConstructorInitializer(SyntaxKind.BaseConstructorInitializer,
                SyntaxFactory.ArgumentList(
                    SyntaxFactory.SeparatedList(
                        parameterNames.Select(name =>
                            SyntaxFactory.Argument(SyntaxFactory.IdentifierName(name)))))))
        .WithBody(SyntaxFactory.Block());

    var classDeclaration = SyntaxFactory.ClassDeclaration(className)
        .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
        .AddBaseListTypes(baseClassSyntax, interfaceSyntax)
        .AddMembers(constructor);

    var namespaceDeclaration = SyntaxFactory.NamespaceDeclaration(
        SyntaxFactory.IdentifierName("TestNamespace"))
        .AddMembers(classDeclaration);

    var compilationUnit = SyntaxFactory.CompilationUnit()
        .AddMembers(namespaceDeclaration)
        .NormalizeWhitespace();

    return compilationUnit.ToFullString();
  }
}
