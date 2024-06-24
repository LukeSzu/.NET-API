using API.Data;
using API.Dtos;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
namespace API.Controllers
{
    [Route("api/items")]
    [ApiController]
    public class ItemsController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        public ItemsController(ApplicationDbContext context, UserManager<User> userManager)

        {
            _context = context;
            _userManager = userManager;
        }
        private string GetCurrentUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
        // GET: api/items
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ItemDto>>> GetItems()
        {
            var items = await _context.Items
                .Where(item => item.IsAvailable)
                .Include(item => item.User) // Załaduj właściciela
                .Select(item => new ItemDto
                {
                    Id = item.Id,
                    UserId = item.UserId,
                    Title = item.Title,
                    Description = item.Description,
                    Price = item.Price,
                    SellerUsername = item.User.UserName // Użyj nazwy sprzedającego
                })
                .ToListAsync();

            return Ok(items);
        }
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ItemDto>>> GetItemById(int id)
        {
            var item = await _context.Items
                .Include(item => item.User) // Załaduj właściciela
                .Select(item => new ItemDto
                {
                    Id = item.Id,
                    UserId = item.UserId,
                    Title = item.Title,
                    Description = item.Description,
                    Price = item.Price,
                    SellerUsername = item.User.UserName // Użyj nazwy sprzedającego
                })
                .FirstOrDefaultAsync(item => item.Id == id);

            if(item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItem(int id, [FromBody] ItemDto updatedItem)
        {
            var item = await _context.Items.FindAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            // Sprawdź, czy zalogowany użytkownik jest właścicielem przedmiotu
            var currentUser = await _userManager.GetUserAsync(User);
            if (item.UserId != currentUser.Id)
            {
                return Forbid(); // Brak uprawnień do edycji
            }

            // Zaktualizuj właściwości przedmiotu
            item.Title = updatedItem.Title;
            item.Description = updatedItem.Description;
            item.Price = updatedItem.Price;

            _context.Items.Update(item);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Item updated successfully" });
        }
        [HttpPost]
        [Authorize] // Endpoint dodawania nowego itemu wymaga autoryzacji
        public async Task<IActionResult> AddItem([FromBody] AddItemDto itemDto)
        {
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token == null)
            {
                return Unauthorized();
            }

            if (token.StartsWith("Bearer "))
            {
                // Usuń prefiks "Bearer " z tokena
                token = token.Substring(7);
            }

            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            if (jsonToken == null)
            {
                return Unauthorized();
            }
            var username = jsonToken.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Sub)?.Value;

            // Znajdź użytkownika w bazie danych na podstawie nazwy użytkownika
            var user = _userManager.FindByNameAsync(username).Result;

            if (user == null)
            {
                return Unauthorized();
            }

            var item = new Item
            {
                Title = itemDto.Title,
                Description = itemDto.Description,
                Price = itemDto.Price,
                UserId = user.Id,  // Ustaw UserId na Id zalogowanego użytkownika
                IsAvailable = true
            };

            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Item added successfully" });
        }
    }
}
