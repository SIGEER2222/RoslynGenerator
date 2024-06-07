using Roslyn.Generated;
using Xunit;

namespace RoslynGenerator.IntegrationTests
{
	public class HelloWorldGeneratorIntegrationTests
	{
		[Fact]
		public void Generated_HelloWorld()
		{
			string greeting = Greeter.GetHelloWorld();

			Assert.Equal("Hello, World!", greeting);
		}
	}

	internal static partial class Greeter
	{
		[HelloWorld]
		public static partial string GetHelloWorld();
	}
}
