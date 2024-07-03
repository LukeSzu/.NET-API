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
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Runtime.Intrinsics.X86;

namespace ApiTests
{
    public class ItemsControllerTests
    {
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly IConfiguration _configuration;

        public ItemsControllerTests()
        {
            _mockUserManager = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();
            _configuration = configurationBuilder.Build();

        }
        private ApplicationDbContext GetInMemoryDbContext(string name)
        {
            return new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
               .UseInMemoryDatabase(databaseName: name)
               .Options);
        }

        [Fact]
        public async Task GetItems_ReturnsOkResult()
        {
            using (var context = GetInMemoryDbContext("test1"))
            {
                // Arrange
                var items = new List<Item>
                {
                new Item { Id = 1, UserId = "1", Title = "Item1", Description = "Description1", Price = 10, IsAvailable = true, AddTime = DateTime.Now, User = new User { Id = "1", UserName = "User1", City="City", Address="address" } },
                new Item { Id = 2, UserId = "2", Title = "Item2", Description = "Description2", Price = 20, IsAvailable = true, AddTime = DateTime.Now, User = new User { Id = "2", UserName = "User2", City="City", Address="address" } }
                };

                await context.Items.AddRangeAsync(items);
                await context.SaveChangesAsync();

                var controller = new ItemsController(context, _mockUserManager.Object);

                // Act
                var result = await controller.GetItems();

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var returnItems = Assert.IsType<List<ItemDto>>(okResult.Value);
                Assert.Equal(2, returnItems.Count);
            }
        }

        [Fact]
        public async Task GetItemById_ReturnsOkResult()
        {
            using (var context = GetInMemoryDbContext("test2"))
            {
                // Arrange
                var item = new Item { Id = 1, UserId = "1", Title = "Item1", Description = "Description1", Price = 10, IsAvailable = true, AddTime = DateTime.Now, User = new User { Id = "1", UserName = "User1", City = "City", Address = "Address" } };
                await context.Items.AddAsync(item);
                await context.SaveChangesAsync();

                var controller = new ItemsController(context, _mockUserManager.Object);

                // Act
                var result = await controller.GetItemById(1);

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var returnItem = Assert.IsType<ItemDto>(okResult.Value);
                Assert.Equal(1, returnItem.Id);
            }
        }
        [Fact]
        public async Task GetItemById_ReturnsNotFoundResult_notExistingId()
        {
            using (var context = GetInMemoryDbContext("test3"))
            {
                // Arrange
                var item = new Item { Id = 1, UserId = "1", Title = "Item1", Description = "Description1", Price = 10, IsAvailable = true, AddTime = DateTime.Now, User = new User { Id = "1", UserName = "User1", City = "City", Address = "Address" } };
                await context.Items.AddAsync(item);
                await context.SaveChangesAsync();

                var controller = new ItemsController(context, _mockUserManager.Object);

                // Act
                var result = await controller.GetItemById(2);

                // Assert
                var notFoundResult = Assert.IsType<NotFoundResult>(result.Result);
            }
        }
        [Fact]
        public async Task GetItemDetailsById_ReturnsOkResult()
        {
            using (var context = GetInMemoryDbContext("test4"))
            {
                // Arrange
                var item = new Item { Id = 1, UserId = "1", Title = "Item1", Description = "Description1", Price = 10, IsAvailable = true, AddTime = DateTime.Now, User = new User { Id = "1", UserName = "User1", City = "City", Address = "Address", PhoneNumber = "123456789" } };
                await context.Items.AddAsync(item);
                await context.SaveChangesAsync();

                var controller = new ItemsController(context, _mockUserManager.Object);

                // Act
                var result = await controller.GetItemDetailsById(1);

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var returnItemDetails = Assert.IsType<ItemDetailsDto>(okResult.Value);
                Assert.Equal(1, returnItemDetails.Id);
                Assert.Equal("123456789", returnItemDetails.PhoneNumber);
                Assert.Equal("City", returnItemDetails.City);
                Assert.Equal("Address", returnItemDetails.Address);
            }
        }
        [Fact]
        public async Task GetItemDetailsById_ReturnsNotFoundResult()
        {
            using (var context = GetInMemoryDbContext("test5"))
            {
                // Arrange
                var item = new Item { Id = 1, UserId = "1", Title = "Item1", Description = "Description1", Price = 10, IsAvailable = true, AddTime = DateTime.Now, User = new User { Id = "1", UserName = "User1", City = "City", Address = "Address", PhoneNumber = "123456789" } };
                await context.Items.AddAsync(item);
                await context.SaveChangesAsync();

                var controller = new ItemsController(context, _mockUserManager.Object);

                // Act
                var result = await controller.GetItemDetailsById(5);

                // Assert
                var okResult = Assert.IsType<NotFoundResult>(result.Result);
            }
        }

        [Fact]
        public async Task AddItem_ReturnsOkResult()
        {
            // Arrange
            var user = new User { Id = "1", UserName = "User1", City = "City", Address = "Address", PhoneNumber = "123456789" };

            _mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
            _mockUserManager.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(user.Id);
            _mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);

            using (var context = GetInMemoryDbContext("test6"))
            {
                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();

                
                var itemDto = new AddItemDto { Title = "NewItem", Description = "NewDescription", Price = 100 };

                var controller = new ItemsController(context, _mockUserManager.Object)
                {
                    ControllerContext = new ControllerContext
                    {
                        HttpContext = new DefaultHttpContext
                        {
                            User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, user.Id)
                        }, "jwt"))
                        }
                    }
                };
                var token = GenerateJwtToken(user);
                controller.ControllerContext.HttpContext.Request.Headers.Authorization = $"Bearer {token}";

                // Act
                var result = await controller.AddItem(itemDto);

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result);
                var response = okResult.Value.GetType().GetProperty("Message").GetValue(okResult.Value, null);
                Assert.Equal("Item added successfully", response);
            }
        }

        [Fact]
        public async Task AddItem_ReturnsUnauthorizedResult_NoUser()
        {
            // Arrange
            var user = new User { Id = "1", UserName = "User1", City = "City", Address = "Address", PhoneNumber = "123456789" };

            _mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
            _mockUserManager.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(user.Id);
            _mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);

            using (var context = GetInMemoryDbContext("test7"))
            {
                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();

                var itemDto = new AddItemDto { Title = "NewItem", Description = "NewDescription", Price = 100 };

                var controller = new ItemsController(context, _mockUserManager.Object)
                {
                    ControllerContext = new ControllerContext
                    {
                        HttpContext = new DefaultHttpContext()
                    }
                };

                // Act
                var result = await controller.AddItem(itemDto);

                // Assert
                var okResult = Assert.IsType<UnauthorizedResult>(result);
            }
        }
        [Fact]
        public async Task AddItem_ReturnsBadRequestResult_ShortTittleAndDescription()
        {
            // Arrange
            var user = new User { Id = "1", UserName = "User1", City = "City", Address = "Address", PhoneNumber = "123456789" };

            _mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
            _mockUserManager.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(user.Id);
            _mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);

            using (var context = GetInMemoryDbContext("test8"))
            {
                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();

                var itemDto = new AddItemDto { Title = "", Description = "", Price = 100 };

                var controller = new ItemsController(context, _mockUserManager.Object)
                {
                    ControllerContext = new ControllerContext
                    {
                        HttpContext = new DefaultHttpContext
                        {
                            User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                            {
                                new Claim(ClaimTypes.NameIdentifier, user.Id)
                            }, "jwt"))
                        }
                    }
                };

                // Act
                var result = await controller.AddItem(itemDto);

                // Assert
                var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
                var errors = Assert.IsType<List<IdentityError>>(badRequestResult.Value);

                Assert.Equal(2, errors.Count);
                Assert.Contains(errors, e => e.Code == "shortTittle" && e.Description == "Tittle must have at least 5 letters.");
                Assert.Contains(errors, e => e.Code == "shortDescription" && e.Description == "Description must have at least 5 letters.");
            }
        }

        [Fact]
        public async Task DeleteItemById_ReturnsUnauthorizedResult()
        {
            using (var context = GetInMemoryDbContext("test9"))
            {
                var controller = new ItemsController(context, _mockUserManager.Object)
                {
                    ControllerContext = new ControllerContext
                    {
                        HttpContext = new DefaultHttpContext()
                    }
                };

                // Act
                var result = await controller.DeleteItemById(1);

                // Assert
                Assert.IsType<UnauthorizedResult>(result);

            }
        }
        [Fact]
        public async Task DeleteItemById_ReturnsNotFoundResult()
        {
            // Arrange
            var user = new User { Id = "1", UserName = "User1", City = "City", Address = "Address", PhoneNumber = "123456789" };

            _mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
            _mockUserManager.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(user.Id);
            _mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);

            using (var context = GetInMemoryDbContext("test10"))
            {
                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();

                var controller = new ItemsController(context, _mockUserManager.Object)
                {
                    ControllerContext = new ControllerContext
                    {
                        HttpContext = new DefaultHttpContext
                        {
                            User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                            {
                                new Claim(ClaimTypes.NameIdentifier, user.Id)
                            }, "jwt"))
                        }
                    }
                };
                var token = GenerateJwtToken(user);
                controller.ControllerContext.HttpContext.Request.Headers.Authorization = $"Bearer {token}";

                // Act
                var result = await controller.DeleteItemById(1);

                // Assert
                Assert.IsType<NotFoundResult>(result);
            }
        }

        [Fact]
        public async Task DeleteItemById_ReturnsForbidResult()
        {
            // Arrange
            var user1 = new User { Id = "1", UserName = "User1", City = "City", Address = "Address", PhoneNumber = "123456789" };
            var user2 = new User { Id = "2", UserName = "User2", City = "City", Address = "Address", PhoneNumber = "123456789" };

            _mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user1);
            _mockUserManager.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(user1.Id);

            using (var context = GetInMemoryDbContext("test11"))
            {
                await context.Users.AddAsync(user1);
                await context.Users.AddAsync(user2);
                await context.Items.AddAsync(new Item { Id = 1, UserId = "2", Title = "Item1", Description = "Description1", Price = 10, IsAvailable = true, AddTime = DateTime.Now, User = user2 });
                await context.SaveChangesAsync();

                var controller = new ItemsController(context, _mockUserManager.Object)
                {
                    ControllerContext = new ControllerContext
                    {
                        HttpContext = new DefaultHttpContext
                        {
                            User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                            {
                                new Claim(ClaimTypes.NameIdentifier, user1.Id)
                            }, "jwt"))
                        }
                    }
                };
                var token = GenerateJwtToken(user1);
                controller.ControllerContext.HttpContext.Request.Headers.Authorization = $"Bearer {token}";

                // Act
                var result = await controller.DeleteItemById(1);

                // Assert
                Assert.IsType<ForbidResult>(result);
            }
        }
        [Fact]
        public async Task DeleteItemById_ReturnsNoContent()
        {
            // Arrange
            var user1 = new User { Id = "1", UserName = "User1", City = "City", Address = "Address", PhoneNumber = "123456789" };

            _mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user1);
            _mockUserManager.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(user1.Id);

            using (var context = GetInMemoryDbContext("test12"))
            {
                await context.Users.AddAsync(user1);
                await context.Items.AddAsync(new Item { Id = 1, UserId = "1", Title = "Item1", Description = "Description1", Price = 10, IsAvailable = true, AddTime = DateTime.Now, User = user1 });
                await context.SaveChangesAsync();

                var controller = new ItemsController(context, _mockUserManager.Object)
                {
                    ControllerContext = new ControllerContext
                    {
                        HttpContext = new DefaultHttpContext
                        {
                            User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                            {
                                new Claim(ClaimTypes.NameIdentifier, user1.Id)
                            }, "jwt"))
                        }
                    }
                };
                var token = GenerateJwtToken(user1);
                controller.ControllerContext.HttpContext.Request.Headers.Authorization = $"Bearer {token}";

                // Act
                var result = await controller.DeleteItemById(1);

                // Assert
                Assert.IsType<NoContentResult>(result);
            }
        }

        [Fact]
        public async Task GetMyItems_ReturnOkResult()
        {
            // Arrange
            var user1 = new User { Id = "1", UserName = "User1", City = "City", Address = "Address", PhoneNumber = "123456789" };
            var user2 = new User { Id = "2", UserName = "User2", City = "City", Address = "Address", PhoneNumber = "123456789" };

            _mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user1);
            _mockUserManager.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(user1.Id);

            using (var context = GetInMemoryDbContext("test13"))
            {
                await context.Users.AddAsync(user1);
                await context.Users.AddAsync(user2);
                await context.Items.AddAsync(new Item { Id = 1, UserId = "2", Title = "Item1", Description = "Description1", Price = 10, IsAvailable = true, AddTime = DateTime.Now, User = user2 });
                await context.Items.AddAsync(new Item { Id = 2, UserId = "2", Title = "Item2", Description = "Description2", Price = 10, IsAvailable = true, AddTime = DateTime.Now, User = user2 });
                await context.Items.AddAsync(new Item { Id = 3, UserId = "1", Title = "Item3", Description = "Description3", Price = 10, IsAvailable = true, AddTime = DateTime.Now, User = user1 });
                await context.Items.AddAsync(new Item { Id = 4, UserId = "1", Title = "Item4", Description = "Description4", Price = 10, IsAvailable = true, AddTime = DateTime.Now, User = user1 });
                await context.SaveChangesAsync();

                var controller = new ItemsController(context, _mockUserManager.Object)
                {
                    ControllerContext = new ControllerContext
                    {
                        HttpContext = new DefaultHttpContext
                        {
                            User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                            {
                                new Claim(ClaimTypes.NameIdentifier, user1.Id)
                            }, "jwt"))
                        }
                    }
                };
                var token = GenerateJwtToken(user1);
                controller.ControllerContext.HttpContext.Request.Headers.Authorization = $"Bearer {token}";

                // Act
                var result = await controller.GetUserItems();

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var returnItems = Assert.IsType<List<ItemDto>>(okResult.Value);
                Assert.Equal(2, returnItems.Count);
            }
        }

        [Fact]
        public async Task GetMyItems_ReturnUnauthorizedResult()
        {
            // Arrange
            var user1 = new User { Id = "1", UserName = "User1", City = "City", Address = "Address", PhoneNumber = "123456789" };
            var user2 = new User { Id = "2", UserName = "User2", City = "City", Address = "Address", PhoneNumber = "123456789" };

            _mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user1);
            _mockUserManager.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(user1.Id);

            using (var context = GetInMemoryDbContext("test14"))
            {
                await context.Users.AddAsync(user1);
                await context.Users.AddAsync(user2);
                await context.Items.AddAsync(new Item { Id = 1, UserId = "2", Title = "Item1", Description = "Description1", Price = 10, IsAvailable = true, AddTime = DateTime.Now, User = user2 });
                await context.Items.AddAsync(new Item { Id = 2, UserId = "2", Title = "Item2", Description = "Description2", Price = 10, IsAvailable = true, AddTime = DateTime.Now, User = user2 });
                await context.Items.AddAsync(new Item { Id = 3, UserId = "1", Title = "Item3", Description = "Description3", Price = 10, IsAvailable = true, AddTime = DateTime.Now, User = user1 });
                await context.Items.AddAsync(new Item { Id = 4, UserId = "1", Title = "Item4", Description = "Description4", Price = 10, IsAvailable = true, AddTime = DateTime.Now, User = user1 });
                await context.SaveChangesAsync();

                var controller = new ItemsController(context, _mockUserManager.Object)
                {
                    ControllerContext = new ControllerContext
                    {
                        HttpContext = new DefaultHttpContext()
                    }
                };

                // Act
                var result = await controller.GetUserItems();

                // Assert
                var okResult = Assert.IsType<UnauthorizedResult>(result.Result);
            }
        }

        [Fact]
        public async Task UpdateItemById_ReturnsUnauthorizedResult()
        {
            var user = new User { Id = "1", UserName = "User1", City = "City", Address = "Address", PhoneNumber = "123456789" };

            _mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
            _mockUserManager.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(user.Id);
            _mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);

            using (var context = GetInMemoryDbContext("test15"))
            {
                await context.Users.AddAsync(user);
                await context.Items.AddAsync(new Item { Id = 1, UserId = "1", Title = "Item1", Description = "Description1", Price = 10, IsAvailable = true, AddTime = DateTime.Now, User = user });
                await context.SaveChangesAsync();

                var controller = new ItemsController(context, _mockUserManager.Object)
                {
                    ControllerContext = new ControllerContext
                    {
                        HttpContext = new DefaultHttpContext()
                    }
                };
                var edit = new EditItemDto { Title = "item2", Description = "Description2", IsAvailable = false, Price = 20 };

                // Act
                var result = await controller.UpdateItem(1, edit);

                // Assert
                Assert.IsType<UnauthorizedResult>(result);

            }
        }

        [Fact]
        public async Task UpdateItem_ReturnsNotFoundResult()
        {
            // Arrange
            var user = new User { Id = "1", UserName = "User1", City = "City", Address = "Address", PhoneNumber = "123456789" };

            _mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
            _mockUserManager.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(user.Id);
            _mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);

            using (var context = GetInMemoryDbContext("test16"))
            {
                await context.Users.AddAsync(user);

                await context.Items.AddAsync(new Item { Id = 1, UserId = "1", Title = "Item1", Description = "Description1", Price = 10, IsAvailable = true, AddTime = DateTime.Now, User = user });
                await context.SaveChangesAsync();

                var controller = new ItemsController(context, _mockUserManager.Object)
                {
                    ControllerContext = new ControllerContext
                    {
                        HttpContext = new DefaultHttpContext
                        {
                            User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                            {
                                new Claim(ClaimTypes.NameIdentifier, user.Id)
                            }, "jwt"))
                        }
                    }
                };
                var token = GenerateJwtToken(user);
                controller.ControllerContext.HttpContext.Request.Headers.Authorization = $"Bearer {token}";

                var edit = new EditItemDto { Title = "item2", Description = "Description2", IsAvailable = false, Price = 20 };

                // Act
                var result = await controller.UpdateItem(2, edit);

                // Assert
                Assert.IsType<NotFoundResult>(result);
            }
        }

        [Fact]
        public async Task UpdateItem_ReturnsForbidResult()
        {
            // Arrange
            var user1 = new User { Id = "1", UserName = "User1", City = "City", Address = "Address", PhoneNumber = "123456789" };
            var user2 = new User { Id = "2", UserName = "User2", City = "City", Address = "Address", PhoneNumber = "123456789" };

            _mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user1);
            _mockUserManager.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(user1.Id);

            using (var context = GetInMemoryDbContext("test17"))
            {
                await context.Users.AddAsync(user1);
                await context.Users.AddAsync(user2);
                await context.Items.AddAsync(new Item { Id = 1, UserId = "2", Title = "Item1", Description = "Description1", Price = 10, IsAvailable = true, AddTime = DateTime.Now, User = user2 });
                await context.SaveChangesAsync();

                var controller = new ItemsController(context, _mockUserManager.Object)
                {
                    ControllerContext = new ControllerContext
                    {
                        HttpContext = new DefaultHttpContext
                        {
                            User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                            {
                                new Claim(ClaimTypes.NameIdentifier, user1.Id)
                            }, "jwt"))
                        }
                    }
                };
                var token = GenerateJwtToken(user1);
                controller.ControllerContext.HttpContext.Request.Headers.Authorization = $"Bearer {token}";

                var edit = new EditItemDto { Title = "item2", Description = "Description2", IsAvailable = false, Price = 20 };

                // Act
                var result = await controller.UpdateItem(1, edit);

                // Assert
                Assert.IsType<ForbidResult>(result);
            }
        }


        [Fact]
        public async Task UpdateItemBy_ReturnsOk()
        {
            // Arrange
            var user1 = new User { Id = "1", UserName = "User1", City = "City", Address = "Address", PhoneNumber = "123456789" };

            _mockUserManager.Setup(um => um.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user1);
            _mockUserManager.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>())).Returns(user1.Id);

            using (var context = GetInMemoryDbContext("test18"))
            {
                await context.Users.AddAsync(user1);
                await context.Items.AddAsync(new Item { Id = 1, UserId = "1", Title = "Item1", Description = "Description1", Price = 10, IsAvailable = true, AddTime = DateTime.Now, User = user1 });
                await context.SaveChangesAsync();

                var controller = new ItemsController(context, _mockUserManager.Object)
                {
                    ControllerContext = new ControllerContext
                    {
                        HttpContext = new DefaultHttpContext
                        {
                            User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                            {
                                new Claim(ClaimTypes.NameIdentifier, user1.Id)
                            }, "jwt"))
                        }
                    }
                };
                var token = GenerateJwtToken(user1);
                controller.ControllerContext.HttpContext.Request.Headers.Authorization = $"Bearer {token}";

                var edit = new EditItemDto { Title = "item2", Description = "Description2", IsAvailable = false, Price = 20 };

                // Act
                var result = await controller.UpdateItem(1, edit);

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result);
                var response = okResult.Value.GetType().GetProperty("Message").GetValue(okResult.Value, null);
                Assert.Equal("Item updated successfully", response);
            }
        }





        private string GenerateJwtToken(User user)
        {

            var jwtSettings = _configuration.GetSection("Jwt");
            var secretKey = jwtSettings["Key"];
            var issuer = jwtSettings["Issuer"];

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: issuer,
                claims: claims,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}
