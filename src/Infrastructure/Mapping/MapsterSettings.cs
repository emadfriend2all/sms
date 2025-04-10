using Mapster;

namespace Showmatics.Infrastructure.Mapping;

public class MapsterSettings
{
    public static void Configure()
    {
        TypeAdapterConfig.GlobalSettings.Default.MaxDepth(5);

        // here we will define the type conversion / Custom-mapping
        // More details at https://github.com/MapsterMapper/Mapster/wiki/Custom-mapping
    }
}