using API.Controllers;
using API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using API.Models;
using API.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Configuration;
namespace ApiTests
{
    public class ItemsControllerTests
    {
        private readonly ApplicationDbContext _context;
        private readonly ItemsController _controller;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;

        public ItemsControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new ApplicationDbContext(options);
            SeedDatabase();

            _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

            var mockUserStore = new Mock<IUserStore<User>>();
            var mockSignInManager = new Mock<SignInManager<User>>(mockUserStore.Object,
                                                                   Mock.Of<IHttpContextAccessor>(),
                                                                   Mock.Of<IUserClaimsPrincipalFactory<User>>(),
                                                                   null, null, null, null)
            {
                CallBase = true
            };

            var mockUserManager = new Mock<UserManager<User>>(mockUserStore.Object, null, null, null, null, null, null, null, null);
            mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                           .ReturnsAsync(() => _context.Users.First(u => u.UserName == "user1")); // Przyk³adowy u¿ytkownik dla testów

            _controller = new ItemsController(_context, mockUserManager.Object);
            _userManager = mockUserManager.Object;
        }
        private void SeedDatabase()
        {
            _context.Database.EnsureDeleted(); // Upewniamy siê, ¿e baza danych jest usuniêta
            _context.Database.EnsureCreated(); // Tworzymy now¹ bazê danych

            var users = new List<User>
            {
                new User { Id = "1", UserName = "user1" },
                new User { Id = "2", UserName = "user2" }
            };

            _context.Users.AddRange(users);
            _context.SaveChanges(); // Save changes to detach tracked entities

            var items = new List<Item>
            {
                new Item { Id = 1, UserId = "1", Title = "Item 1", Description = "Description 1", Price = 100, IsAvailable = true },
                new Item { Id = 2, UserId = "2", Title = "Item 2", Description = "Description 2", Price = 200, IsAvailable = true }
            };

            _context.Items.AddRange(items);

            _context.SaveChanges();
        }
        [Fact]
        public async Task Get_Items_Returns_OkResult()
        {
            // Arrange (setup)
            

            // Act (perform the action)
            var result = await _controller.GetItems();

            // Assert (verify the result)
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var items = Assert.IsAssignableFrom<IEnumerable<ItemDto>>(okResult.Value);
            Assert.Equal(2, items.Count()); // Ensure two items are returned

            // Additional assertions on item properties if needed
            var item1 = items.FirstOrDefault(i => i.Id == 1);
            Assert.NotNull(item1);
            Assert.Equal("Item 1", item1.Title);
            Assert.Equal("Description 1", item1.Description);
            Assert.Equal(100, item1.Price);

            var item2 = items.FirstOrDefault(i => i.Id == 2);
            Assert.NotNull(item2);
            Assert.Equal("Item 2", item2.Title);
            Assert.Equal("Description 2", item2.Description);
            Assert.Equal(200, item2.Price);
        }

        [Fact]
        public async Task Get_Item_Returns_OkResult()
        {
            int itemId = 1;
            var result = await _controller.GetItemById(itemId);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var item = Assert.IsType<ItemDto>(okResult.Value);
            Assert.Equal(itemId, item.Id);
        }
        [Fact]
        public async Task GetItemNotFound()
        {
            var result = await _controller.GetItemById(2137);
            var okResult = Assert.IsType<NotFoundResult>(result.Result);
        }
        [Fact]
        public async Task GetItemBadRequest()
        {
            var result = await _controller.GetItemById(-2);
            var okResult = Assert.IsType<NotFoundResult>(result.Result);
        }
        [Fact]
        public async Task Add_Item_Returns_CreatedResult()
        {
            var newItem = new AddItemDto { Title = "New Item", Description = "New Description", Price = 300 };

            // Utwórz u¿ytkownika i token JWT dla testu
            var user = new User { Id = "1", UserName = "user1" };
            var token = GenerateJwtToken(user);

            // Ustaw nag³ówek Authorization z tokenem JWT
            _controller.HttpContext.Request.Headers["Authorization"] = "Bearer " + token;

            // Wykonaj akcjê dodawania przedmiotu
            var result = await _controller.AddItem(newItem);

            // SprawdŸ czy zwrócono poprawny wynik
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var item = Assert.IsType<Item>(createdResult.Value);
            Assert.Equal(newItem.Title, item.Title);
        }
        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Issuer"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}