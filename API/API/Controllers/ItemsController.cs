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

        // GET: api/items
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ItemDto>>> GetItems()
        {
            var items = await _context.Items
                .Where(item => item.IsAvailable)
                .Include(item => item.User) 
                .Select(item => new ItemDto
                {
                    Id = item.Id,
                    Title = item.Title,
                    Description = item.Description,
                    Price = item.Price,
                    SellerUsername = item.User.UserName,
                    isAvailable = item.IsAvailable,
                    AddTime = item.AddTime
                })
                .ToListAsync();

            return Ok(items);
        }

        // GET: api/items/id/details
        [HttpGet("{id}/details")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ItemDetailsDto>>> GetItemDetailsById(int id)
        {
            var item = await _context.Items
                .Include(item => item.User)
                .Select(item => new ItemDetailsDto
                {
                    Id = item.Id,
                    Title = item.Title,
                    Description = item.Description,
                    Price = item.Price,
                    SellerUsername = item.User.UserName,
                    City = item.User.City,
                    PhoneNumber = item.User.PhoneNumber,
                    Address = item.User.Address,
                    isAvailable = item.IsAvailable,
                    AddTime = item.AddTime

                })
                .FirstOrDefaultAsync(item => item.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }
        // GET: api/items/id
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ItemDto>>> GetItemById(int id)
        {
            var item = await _context.Items
                .Include(item => item.User)
                .Select(item => new ItemDto
                {
                    Id = item.Id,
                    Title = item.Title,
                    Description = item.Description,
                    Price = item.Price,
                    SellerUsername = item.User.UserName,
                    isAvailable = item.IsAvailable,
                    AddTime = item.AddTime
                })
                .FirstOrDefaultAsync(item => item.Id == id);

            if(item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }


        // PUT: api/items/id
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItem(int id, [FromBody] EditItemDto updatedItem)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Unauthorized();
            }

            var item = await _context.Items.FindAsync(id);

            if (item == null)
            {
                return NotFound();
            }
            if (item.UserId != user.Id)
            {
                return Forbid(); 
            }
            item.Title = updatedItem.Title;
            item.Description = updatedItem.Description;
            item.Price = updatedItem.Price;
            item.IsAvailable = updatedItem.IsAvailable;

            _context.Items.Update(item);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Item updated successfully" });
        }


        // GET: api/items/myitems
        [Authorize]
        [HttpGet("myitems")]
        public async Task<IActionResult> GetUserItems()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Unauthorized();
            }

            var items = _context.Items.Where(i => i.UserId == userId).ToList();

            var itemsDto = items.Select(i => new ItemDto
            {
                Id = i.Id,
                Title = i.Title,
                Description = i.Description,
                Price = i.Price,
                SellerUsername = user.UserName,
                AddTime = i.AddTime
            }).ToList();

            return Ok(itemsDto);
        }


        // POST: api/items
        [HttpPost]
        [Authorize] 
        public async Task<IActionResult> AddItem([FromBody] AddItemDto itemDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Unauthorized();
            }

            var item = new Item
            {
                Title = itemDto.Title,
                Description = itemDto.Description,
                Price = itemDto.Price,
                UserId = user.Id,  
                IsAvailable = true,
                AddTime = itemDto.AddTime
            };
            var errors = new List<IdentityError>();
            if (item.Title.Length < 5 )
            {
                errors.Add(new IdentityError { Code = "shortTittle", Description = "Tittle must have at least 5 letters." });
            }
            if (item.Description.Length < 5)
            {
                errors.Add(new IdentityError { Code = "shortDescription", Description = "Description must have at least 5 letters." });
            }
            if (errors.Count() > 0)
            {
                return BadRequest(errors);
            }

            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Item added successfully" });
        }


        // DELETE: api/items/id
        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteItemById(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Unauthorized();
            }

            var item = await _context.Items.FindAsync(id);

            if (item == null)
            {
                return NotFound();
            }
            if (item.UserId != user.Id)
            {
                return Forbid();
            }

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
