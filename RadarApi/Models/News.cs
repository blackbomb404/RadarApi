using System.ComponentModel.DataAnnotations.Schema;

namespace RadarApi.Models
{
    public class News
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public NewsType Type { get; set; }
        public string ThumbnailPath { get; set; }

        public int Views { get; set; }
        public int Comments { get; set; }

        public string AuthorName { get; set; }

        [Column(TypeName = "TIMESTAMP")]
        public DateTime PostedAt { get; set; }

        public News() { }

        public News(long id, string title, string content, NewsType type, string thumbnailPath, int views, int comments, string authorName, DateTime postedAt)
        {
            Id = id;
            Title = title;
            Content = content;
            Type = type;
            ThumbnailPath = thumbnailPath;
            Views = views;
            Comments = comments;
            AuthorName = authorName;
            PostedAt = postedAt;
        }
    }
}