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
namespace ApiTests
{
    public class ItemsControllerTests
    {
        private readonly ApplicationDbContext _context;
        private readonly ItemsController _controller;
       

        public ItemsControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ApplicationDbContext(options);

            // Seed the in-memory database
            SeedDatabase();

            _controller = new ItemsController(_context);
        }
        private void SeedDatabase()
        {
            _context.Database.EnsureDeleted(); // Upewniamy siê, ¿e baza danych jest usuniêta
            _context.Database.EnsureCreated(); // Tworzymy now¹ bazê danych
            var users = new List<User>
            {
                new User { Id = 1, Username = "test", Password = "$2y$10$Qe/sNVJX5.us2oHGav648ed03TVGFzB1oQwQSv9IC2Nb4uvJlsEcG" },
                new User { Id = 2, Username = "test_solder", Password = "$2y$10$O7tboM8rFIiSWAVR8yXmP.ngSNZZMb5ef18drjTovll0fJG6T7TYy" }
            };

            var items = new List<Item>
            {
                new Item { Id = 1, UserId = 1, Title = "Item1", Description = "Description1", Price = 10.0m, IsAvailable = true },
                new Item { Id = 2, UserId = 2, Title = "Item2", Description = "Description2", Price = 20.0m, IsAvailable = true },
                new Item { Id = 3, UserId = 1, Title = "Item3", Description = "Description3", Price = 30.0m, IsAvailable = false }
            };

            _context.Users.AddRange(users);
            _context.Items.AddRange(items);
            _context.SaveChanges();
        }
        [Fact]
        public async Task GetAllAvailableItemsCount()
        {
            var result = await _controller.GetItems();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var items = Assert.IsAssignableFrom<IEnumerable<ItemDto>>(okResult.Value);

            var itemDtos = items.ToList();
            Assert.Equal(2, itemDtos.Count);

        }
        [Fact]
        public async Task GetAllAvailableItemsContent()
        {
            var result = await _controller.GetItems();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var items = Assert.IsAssignableFrom<IEnumerable<ItemDto>>(okResult.Value);

            var itemDtos = items.ToList();

            Assert.Equal(1, itemDtos[0].Id);
            Assert.Equal(1, itemDtos[0].UserId);
            Assert.Equal("Item1", itemDtos[0].Title);
            Assert.Equal("Description1", itemDtos[0].Description);
            Assert.Equal(10.0m, itemDtos[0].Price);
            Assert.Equal("test", itemDtos[0].SellerUsername);

            Assert.Equal(2, itemDtos[1].Id);
            Assert.Equal(2, itemDtos[1].UserId);
            Assert.Equal("Item2", itemDtos[1].Title);
            Assert.Equal("Description2", itemDtos[1].Description);
            Assert.Equal(20.0m, itemDtos[1].Price);
            Assert.Equal("test_solder", itemDtos[1].SellerUsername);
        }
        [Fact]
        public async Task GetItemCorrect()
        {
            var result = await _controller.GetItemById(1);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var item = Assert.IsAssignableFrom<ItemDto>(okResult.Value);

            Assert.Equal(1, item.Id);
            Assert.Equal(1, item.UserId);
            Assert.Equal("Item1", item.Title);
            Assert.Equal("Description1", item.Description);
            Assert.Equal(10.0m, item.Price);
            Assert.Equal("test", item.SellerUsername);
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

    }
}