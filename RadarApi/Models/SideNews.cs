using System.ComponentModel.DataAnnotations.Schema;

namespace RadarApi.Models
{
    public class SideNews
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string ThumbnailPath { get; set; }
        public string AuthorName { get; set; }
        public DateTime PostedAt { get; set; }
    }
}
