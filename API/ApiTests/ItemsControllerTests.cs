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
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly Mock<ApplicationDbContext> _mockContext;
        private readonly ItemsController _controller;
        private readonly IConfiguration _configuration;

        public ItemsControllerTests()
        {
            _mockUserManager = new Mock<UserManager<User>>(
            Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _mockContext = new Mock<ApplicationDbContext>(options);
            _controller = new ItemsController(_mockContext.Object, _mockUserManager.Object);
        }

        [Fact]
        public async Task GetItems_ReturnsOkResult_WithListOfItems()
        {
            // Arrange
            var items = new List<Item>
            {
                new Item { Id = 1, UserId="1" ,Title = "Item1", Description = "Description1", Price = 10, IsAvailable = true, User = new User { Id="1" ,UserName = "User1" } },
                new Item { Id = 2, UserId="1" ,Title = "Item2", Description = "Description2", Price = 20, IsAvailable = true, User = new User { Id="2", UserName = "User2" } }
            };

            _mockContext.Setup(m => m.Items.Include(i => i.User).Where(i => i.IsAvailable).ToListAsync())
                .ReturnsAsync(items);

            // Act
            var result = await _controller.GetItems();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnItems = Assert.IsType<List<ItemDto>>(okResult.Value);
            Assert.Equal(2, returnItems.Count);
        }

        [Fact]
        public async Task GetItemDetailsById_ReturnsNotFound_WhenItemDoesNotExist()
        {
            // Arrange
            var itemId = 1;
            _mockContext.Setup(m => m.Items.Include(i => i.User).FirstOrDefaultAsync(i => i.Id == itemId))
                .ReturnsAsync((Item)null);

            // Act
            var result = await _controller.GetItemDetailsById(itemId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task AddItem_ReturnsOkResult_WhenItemIsAdded()
        {
            // Arrange
            var addItemDto = new AddItemDto { Title = "NewItem", Description = "NewDescription", Price = 30 };
            var user = new User { Id = "1", UserName = "User1" };
            _mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
            _mockContext.Setup(m => m.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _controller.AddItem(addItemDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Item added successfully", ((dynamic)okResult.Value).Message);
        }

    }
}