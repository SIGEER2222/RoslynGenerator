namespace RoslynGenerator.Build;
public class BuildRepository {
  public static string GenerateClassWithInterface(ClassInfo classInfo, string repName, string interfaceName, string genericTypeName) {
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
}
