namespace CesiZen_Backend.Dtos.CategoryDtos
{
    public record CreateCategoryRequestDto(
        string Name,
        string IconLink
    );

    public record UpdateCategoryRequestDto(
        string Name,
        string IconLink
    );
}
