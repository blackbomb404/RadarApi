using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RadarApi.DbContexts;
using RadarApi.Models;
using RadarApi.Models.DTO;
using RadarApi.Helpers;
using Microsoft.AspNetCore.Cors;

namespace RadarApi.Controllers
{
    [EnableCors("myPolicy")]
    [ApiController]
    [Route("api/[controller]")]
    public class NewsController : ControllerBase
    {
        private readonly RadarContext _context;

        public NewsController(RadarContext context)
        {
            _context = context;
        }

        // GET api/news/topcarousel
        [HttpGet("topcarousel")]
        public async Task<ActionResult<IEnumerable<News>>> GetTopCarouselItems()
        {
            return await _context.News
                .Where(e => e.Type.Equals(NewsType.Entertainment))
                .OrderByDescending(e => e.Id)
                .Take(3)
                .ToListAsync();
        }

        /// <summary>
        /// Returns all the top news.
        /// </summary>
        /// <returns></returns>
        // GET api/news/topnews
        [HttpGet("topnews")]
        public async Task<ActionResult<IEnumerable<TopNews>>> GetTopNews()
        {
            return await _context.News
                .Where(e => e.Content == "-")
                .Take(4)
                .Select(news => new TopNews(news.Title, news.ThumbnailPath, news.PostedAt))
                .ToListAsync();
        }

        /// <summary>
        /// Returns all the music related news.
        /// </summary>
        /// <returns></returns>
        // GET api/news/music
        [HttpGet("music")]
        public async Task<ActionResult<IEnumerable<NewsDTOResponse>>> GetMusicNews()
        {
            return await GetAllWithType(NewsType.Music);
        }

        // POST api/news/music
        [RequestSizeLimit(bytes: 500 * 1024)]
        [HttpPost]
        public async Task<IActionResult> FileUploadAsync([FromForm] NewsDTORequest newsDTO)
        {
            var file = newsDTO.File;

            string fileUrl;
            try
            {
                FileUploadUtil.CheckUploadedFile(file);

                fileUrl = Path.Combine(FileUploadUtil.requestPath, await FileUploadUtil.SaveAsync(file));
                fileUrl = new Uri(new Uri("https://localhost:7074"), fileUrl).ToString();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { ex.Message });
            }

            var news = new News
            {
                Title = newsDTO.Title,
                Content = newsDTO.Content,
                Type = newsDTO.Type,
                AuthorName = newsDTO.AuthorName,
                PostedAt = DateTime.Now,
                ThumbnailPath = fileUrl
            };
            await _context.News.AddAsync(news);
            await _context.SaveChangesAsync();

            return Ok(news);
        }

        /// <summary>
        /// Returns all the entertainment related news.
        /// </summary>
        /// <returns></returns>
        // GET api/entertainment
        [HttpGet("entertainment")]
        public async Task<ActionResult<IEnumerable<NewsDTOResponse>>> GetEntertainmentNews()
        {
            return await GetAllWithType(NewsType.Entertainment);
        }

        /// <summary>
        /// Returns all the lifestyle related news
        /// </summary>
        /// <returns></returns>
        // GET api/lifestyle
        [HttpGet("lifestyle")]
        public async Task<ActionResult<IEnumerable<NewsDTOResponse>>> GetLifestyleNews()
        {
            return await GetAllWithType(NewsType.Lifestyle);
        }

        /// <summary>
        /// Returns all the marks related news.
        /// </summary>
        /// <returns></returns>
        // GET api/mark
        [HttpGet("mark")]
        public async Task<ActionResult<IEnumerable<NewsDTOResponse>>> GetMarksNews()
        {
            return await GetAllWithType(NewsType.Mark);
        }

        /// <summary>
        /// Returns all the opinion related news.
        /// </summary>
        /// <returns></returns>
        // GET api/opinion
        [HttpGet("opinion")]
        public async Task<ActionResult<IEnumerable<TopNews>>> GetOpinionNews()
        {
            //var records = await GetAllWithType(NewsType.Opinion);
            //records.ForEach(rec =>
            //{
            //    rec.ThumbnailPath;
            //});

            //return await GetAllWithType(NewsType.Opinion);
            return await _context.News
                .Where(e => e.Type == NewsType.Opinion)
                .Select(news => new TopNews(news.Title, news.ThumbnailPath, news.PostedAt))
                .ToListAsync();
        }

        /// <summary>
        /// Utility method which filters the result set of NewsDTO by Type. 
        /// </summary>
        /// <param name="newsType">The type whose news should match to.</param>
        /// <returns></returns>
        private async Task<List<NewsDTOResponse>> GetAllWithType(NewsType newsType)
        {
            return await _context.News
                .Where(news => news.Type.Equals(newsType))
                .OrderBy(news => news.Id)
                .Select(news => NewsToDTO(news))
                .Take(6)
                .ToListAsync();
        }

        /// <summary>
        /// Utility method that extracts a NewsDTO instance from <paramref name="news" />.
        /// </summary>
        /// <param name="news">The News instance from where to extract the NewsDTO instance from.</param>
        /// <returns>The extracted NewsDTO instance.</returns>
        /// 
        public static NewsDTOResponse NewsToDTO(News news) => new NewsDTOResponse(
                news.Id,
                news.Title,
                news.Content,
                news.Type,
                news.ThumbnailPath,
                news.Views,
                news.Comments,
                news.AuthorName,
                news.PostedAt
            );

    }
}
