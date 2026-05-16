using AutoMapper;
using CartService.Application.Mappings;
using System.Diagnostics.CodeAnalysis;

namespace CartService.UnitTests;

[ExcludeFromCodeCoverage]
internal class TestBase
{
    protected Fixture Fixture => new Fixture();

    protected IMapper Mapper { get; private set; } = new MapperConfiguration(cfg =>
        cfg.AddProfile<CartMappingProfile>()).CreateMapper();
}
