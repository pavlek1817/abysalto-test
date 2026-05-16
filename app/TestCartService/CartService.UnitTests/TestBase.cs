using System.Diagnostics.CodeAnalysis;

namespace CartService.UnitTests;

[ExcludeFromCodeCoverage]
internal class TestBase
{
    protected Fixture Fixture => new Fixture();
}