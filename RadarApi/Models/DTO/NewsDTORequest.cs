namespace RadarApi.Models.DTO
{
    public record NewsDTORequest(string Title, string Content, NewsType Type, string AuthorName, IFormFile File);
}
