using Microsoft.AspNetCore.Mvc;
using WebSiteBanMoHinh.Repository;
using Microsoft.EntityFrameworkCore;

namespace WebSiteBanMoHinh.Controllers
{
    public class SearchController : Controller
    {
        private readonly DataContext _context;  // DbContext của bạn

        public SearchController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Suggestions(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Json(new List<object>());

            var suggestions = await _context.Products
                .Where(p => p.Name.Contains(term))
                .Select(p => new {
                    p.Id,
                    p.Name,
                    p.Price,
                    ImageUrl = p.Image  // mapping rõ tên
                })
                .Take(10)
                .ToListAsync();

            return Json(suggestions);
        }
    }

}