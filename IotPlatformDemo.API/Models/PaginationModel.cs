namespace IotPlatformDemo.API.Models;

public record PaginationModel
{
    public int PageSize { get; set; }
    public int Page { get; set; }
}