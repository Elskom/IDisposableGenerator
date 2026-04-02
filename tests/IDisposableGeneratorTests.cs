namespace IDisposableGenerator.Tests;

public partial class IDisposableGeneratorTests
{
    [Fact]
    public async Task TestGeneratingNoInput()
        => await SGAssert.RunTest<CSGeneratorTest>(string.Empty, string.Empty);
}
