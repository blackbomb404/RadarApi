using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RadarApi.DbContexts;
using RadarApi.Helpers;
using RadarApi.Models;
using RadarApi.Models.DTO;

namespace RadarApi.Controllers
{
    [EnableCors("myPolicy")]
    [ApiController]
    [Route("api/[controller]")]
    public class SideNewsController : ControllerBase
    {
        private readonly RadarContext _context;

        public SideNewsController(RadarContext context)
        {
            _context = context;
        }

        // GET api/sidenews
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SideNews>>> GetSideNews()
        {
            return await _context.SideNews.ToListAsync();
        }

        // POST api/sidenews
        [RequestSizeLimit(bytes: 500 * 1024)]
        [HttpPost]
        public async Task<IActionResult> FileUploadAsync([FromForm] SideNewsDTO sideNewsDTO)
        {
            var file = sideNewsDTO.File;

            string fileUrl;
            try
            {
                FileUploadUtil.CheckUploadedFile(file);

                fileUrl = Path.Combine(FileUploadUtil.requestPath, await FileUploadUtil.SaveAsync(file));
                fileUrl = new Uri(new Uri("https://localhost:7074"), fileUrl).ToString();
            } catch (ArgumentException ex)
            {
                return BadRequest(new { ex.Message });
            }

            var sideNews = new SideNews
            {
                Title = sideNewsDTO.Title,
                AuthorName = sideNewsDTO.AuthorName,
                ThumbnailPath = fileUrl
            };
            await _context.SideNews.AddAsync(sideNews);
            await _context.SaveChangesAsync();

            return Ok(sideNews);
        }
    }
}
