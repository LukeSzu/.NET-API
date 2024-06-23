using API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace API.Controllers
{
    [Route("api/items")]
    [ApiController]
    public class ItemsController : Controller
    {

        private readonly ApplicationDbContext _context;
        public ItemsController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: api/items
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Item>>> GetItems()
        {
            var items = await _context.Items
                .Where(item => item.IsAvailable)
                .Include(item => item.User) // Załaduj właściciela
                .Select(item => new
                {
                    item.Id,
                    item.UserId,
                    item.Title,
                    item.Description,
                    item.Price,
                    SellerUsername = item.User.Username // Użyj nazwy sprzedającego
                })
                .ToListAsync();

            return Ok(items);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Item>>> GetItemById(int id)
        {
            var item = await _context.Items
                .Include(item => item.User) // Załaduj właściciela
                .Select(item => new
                {
                    item.Id,
                    item.UserId,
                    item.Title,
                    item.Description,
                    item.Price,
                    SellerUsername = item.User.Username // Użyj nazwy sprzedającego
                })
                .FirstOrDefaultAsync(item => item.Id == id);

            if(item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }
    }
}
