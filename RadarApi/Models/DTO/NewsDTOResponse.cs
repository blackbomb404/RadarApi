namespace RadarApi.Models.DTO
{
    public record NewsDTOResponse(
        long Id, string Title, string Content, NewsType Type, string ThumbnailPath,
        int Views, int Comments, string AuthorName, DateTime PostedAt);
}
