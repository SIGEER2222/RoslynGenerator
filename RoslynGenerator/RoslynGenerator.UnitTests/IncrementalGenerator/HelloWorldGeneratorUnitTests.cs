using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Xunit;
using VerifyCS = RoslynGenerator.UnitTests.Verifiers.CSharpSourceGeneratorVerifier<RoslynGenerator.HelloWorldGenerator>;

namespace RoslynGenerator.UnitTests
{
	public class HelloWorldGeneratorUnitTests
	{
		private const string Attribute = @"// <auto-generated/>
#nullable enable

namespace Roslyn.Generated
{
	[global::System.CodeDom.Compiler.GeneratedCodeAttribute(""RoslynGenerator"", ""1.0.0.0"")]
	[global::System.AttributeUsage(global::System.AttributeTargets.Method, AllowMultiple = false)]
	internal sealed class HelloWorldAttribute : global::System.Attribute
	{
	}
}
";

		[Fact]
		public async Task Generator_NoCandidates_AddAttributeUnconditionally()
		{
			string code = @"
using System;
using Roslyn.Generated;

namespace Tests
{
	internal static partial class Greeter
	{
		public static partial string {|#0:GetHelloWorld_0|}();

		[Placeholder]
		public static partial string {|#1:GetHelloWorld_1|}();

		[HelloWorld]
		public static partial string {|#2:GetHelloWorld_2|}(object obj);

		[HelloWorld]
		public static string {|#3:GetHelloWorld_3|}();

		[HelloWorld]
		public static partial void {|#4:GetHelloWorld_4|}();
	}

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	internal sealed class PlaceholderAttribute : Attribute
	{
	}
}
";

			DiagnosticResult cs0501 = VerifyCS.Diagnostic("CS0501", DiagnosticSeverity.Error);
			DiagnosticResult cs8795 = VerifyCS.Diagnostic("CS8795", DiagnosticSeverity.Error);

			DiagnosticResult[] diagnostics =
			{
				cs8795.WithLocation(0).WithArguments("Tests.Greeter.GetHelloWorld_0()"),
				cs8795.WithLocation(1).WithArguments("Tests.Greeter.GetHelloWorld_1()"),
				cs8795.WithLocation(2).WithArguments("Tests.Greeter.GetHelloWorld_2(object)"),
				cs0501.WithLocation(3).WithArguments("Tests.Greeter.GetHelloWorld_3()"),
				cs8795.WithLocation(4).WithArguments("Tests.Greeter.GetHelloWorld_4()"),
			};

			await VerifyCS.VerifyGeneratorAsync(code, diagnostics, ("HelloWorldAttribute.g.cs", Attribute));
		}

		[Fact]
		public async Task Generator_WithCandidates_AddPartialMethods()
		{
			string code = @"
using System;
using Roslyn.Generated;

namespace @Tests
{
	public static partial class @Greeter
	{
		[@HelloWorldAttribute]
		public static partial string @GetHelloWorld();
	}

	public static partial class Greeter
	{
		[HelloWorld]
		public static partial string GetAdditionalHelloWorld();
	}

	internal static partial class AdditionalGreeter
	{
		[Obsolete]
		[HelloWorldAttribute()]
		public static partial string GetHelloWorld();

		[Obsolete, Roslyn.Generated.HelloWorld()]
		public static partial string GetAdditionalHelloWorld();

		[Roslyn.Generated.HelloWorldAttribute(), Obsolete]
		public static partial string Get();

		[global::Roslyn.Generated.HelloWorldAttribute()]
		[Obsolete]
		public static partial string HelloWorld();

		[HelloWorld]
		internal static partial string GetText();
	}

	public partial class NonStatic
	{
		[HelloWorld]
		public partial String Instance();

		[HelloWorld]
		public static partial String Static();
	}

	public sealed partial class Sealed
	{
		[HelloWorld]
		public partial String Public();

		[HelloWorld]
		internal partial String Internal();
	}
}
";

			string greeter = @"// <auto-generated/>
#nullable enable

namespace Tests
{
	partial class Greeter
	{
		public static partial string GetHelloWorld() => ""Hello, World!"";
		public static partial string GetAdditionalHelloWorld() => ""Hello, World!"";
	}
}
";

			string additionalGreeter = @"// <auto-generated/>
#nullable enable

namespace Tests
{
	partial class AdditionalGreeter
	{
		public static partial string GetHelloWorld() => ""Hello, World!"";
		public static partial string GetAdditionalHelloWorld() => ""Hello, World!"";
		public static partial string Get() => ""Hello, World!"";
		public static partial string HelloWorld() => ""Hello, World!"";
		internal static partial string GetText() => ""Hello, World!"";
	}
}
";

			string nonStatic = @"// <auto-generated/>
#nullable enable

namespace Tests
{
	partial class NonStatic
	{
		public partial string Instance() => ""Hello, World!"";
		public static partial string Static() => ""Hello, World!"";
	}
}
";

			string @sealed = @"// <auto-generated/>
#nullable enable

namespace Tests
{
	partial class Sealed
	{
		public partial string Public() => ""Hello, World!"";
		internal partial string Internal() => ""Hello, World!"";
	}
}
";

			await VerifyCS.VerifyGeneratorAsync(code, ("HelloWorldAttribute.g.cs", Attribute),
				("Greeter.HelloWorld.g.cs", greeter),
				("AdditionalGreeter.HelloWorld.g.cs", additionalGreeter),
				("NonStatic.HelloWorld.g.cs", nonStatic),
				("Sealed.HelloWorld.g.cs", @sealed));
		}

		[Fact]
		public async Task Generator_WithCandidatesAlias_AddPartialMethod()
		{
			string code = @"
using System;
using Greet = Roslyn.Generated.HelloWorldAttribute;

namespace Tests
{
	internal static partial class Greeter
	{
		[Greet]
		public static partial string GetAliasHelloWorld();

		[HelloWorld]
		public static partial string {|#0:GetDifferentHelloWorld|}();
	}

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	internal sealed class HelloWorldAttribute : Attribute
	{
	}
}
";

			string generated = @"// <auto-generated/>
#nullable enable

namespace Tests
{
	partial class Greeter
	{
		public static partial string GetAliasHelloWorld() => ""Hello, World!"";
	}
}
";

			DiagnosticResult diagnostic = VerifyCS.Diagnostic("CS8795", DiagnosticSeverity.Error)
				.WithLocation(0)
				.WithArguments("Tests.Greeter.GetDifferentHelloWorld()");

			await VerifyCS.VerifyGeneratorAsync(code, diagnostic, ("HelloWorldAttribute.g.cs", Attribute), ("Greeter.HelloWorld.g.cs", generated));
		}

		[Fact]
		public async Task Generator_WithNamespaces_AddPartialMethods()
		{
			string code = @"
using System;
using Roslyn.Generated;

internal static partial class GlobalNamespaceGreeter
{
	[HelloWorld]
	public static partial string GetHelloWorld();
}

namespace Nested.Namespace
{
	internal static partial class NestedNamespaceGreeter
	{
		[HelloWorld]
		public static partial string GetHelloWorld();
	}
}

namespace Lexically
{
	namespace Nested
	{
		namespace Namespace
		{
			internal static partial class LexicallyNestedNamespaceGreeter
			{
				[HelloWorld]
				public static partial string GetHelloWorld();
			}
		}
	}
}
";

			string global = @"// <auto-generated/>
#nullable enable

partial class GlobalNamespaceGreeter
{
	public static partial string GetHelloWorld() => ""Hello, World!"";
}
";

			string nested = @"// <auto-generated/>
#nullable enable

namespace Nested.Namespace
{
	partial class NestedNamespaceGreeter
	{
		public static partial string GetHelloWorld() => ""Hello, World!"";
	}
}
";

			string lexical = @"// <auto-generated/>
#nullable enable

namespace Lexically.Nested.Namespace
{
	partial class LexicallyNestedNamespaceGreeter
	{
		public static partial string GetHelloWorld() => ""Hello, World!"";
	}
}
";

			await VerifyCS.VerifyGeneratorAsync(code, ("HelloWorldAttribute.g.cs", Attribute),
				("GlobalNamespaceGreeter.HelloWorld.g.cs", global),
				("NestedNamespaceGreeter.HelloWorld.g.cs", nested),
				("LexicallyNestedNamespaceGreeter.HelloWorld.g.cs", lexical));
		}
	}
}
