namespace RoslynGenerator.SyntaxTreeInfo;

public class ClassMember {
  public string Name { get; set; } = null!;
  public string Type { get; set; }
  public List<string> Attributes { get; set; }
  public string DefaultValue { get; set; }
}

public class ClassInfo {
  public string Namespace { get; set; }
  public List<string> Usings { get; set; }
  public List<string> Attributes { get; set; }
  public string ClassName { get; set; }
  public List<ClassMember> Members { get; set; }
  public string BaseClass { get; set; }
  public List<string> ImplementedInterfaces { get; set; }
}
