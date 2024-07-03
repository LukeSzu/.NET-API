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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Formats.Asn1;


namespace ApiTests
{
    public class AuthControllerTests
    {
        private readonly IConfiguration _configuration;
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly Mock<SignInManager<User>> _mockSignInManager;

        public AuthControllerTests()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();
            _configuration = configurationBuilder.Build();

            var userStoreMock = new Mock<IUserStore<User>>();
            _mockUserManager = new Mock<UserManager<User>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

            var contextAccessorMock = new Mock<IHttpContextAccessor>();
            var userPrincipalFactoryMock = new Mock<IUserClaimsPrincipalFactory<User>>();
            _mockSignInManager = new Mock<SignInManager<User>>(
                _mockUserManager.Object,
                contextAccessorMock.Object,
                userPrincipalFactoryMock.Object,
                null,
                null,
                null,
                null);
        }
        private ApplicationDbContext GetInMemoryDbContext(string name)
        {
            return new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
               .UseInMemoryDatabase(databaseName: name)
               .Options);
        }

        [Fact]
        public async Task Login_ValidCredentials()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Username = "testuser",
                Password = "Password123!"
            };

            var user = new User
            {
                UserName = loginDto.Username,
                Email = "testuser@example.com"
            };

            _mockUserManager.Setup(x => x.FindByNameAsync(loginDto.Username))
                .ReturnsAsync(user);

            _mockSignInManager.Setup(x => x.PasswordSignInAsync(loginDto.Username, loginDto.Password, false, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            var _controller = new AuthController(_mockUserManager.Object, _mockSignInManager.Object, _configuration);

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value;

            // Użycie refleksji do uzyskania wartości właściwości anonimowego typu
            var token = response.GetType().GetProperty("Token").GetValue(response, null) as string;
            var username = response.GetType().GetProperty("Username").GetValue(response, null) as string;

            Assert.NotNull(token);
            Assert.Equal("testuser", username);
        }
        [Fact]
        public async Task Login_InvalidCredentials()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Username = "testuser",
                Password = "Password123!"
            };

            var user = new User
            {
                UserName = loginDto.Username,
                Email = "testuser@example.com"
            };

            _mockUserManager.Setup(x => x.FindByNameAsync(loginDto.Username))
                .ReturnsAsync(user);

            _mockSignInManager.Setup(x => x.PasswordSignInAsync(loginDto.Username, loginDto.Password, false, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

            var _controller = new AuthController(_mockUserManager.Object, _mockSignInManager.Object, _configuration);

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var okResult = Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task Register_ReturnsOk()
        {
            var registerDto = new RegisterDto
            {
                Username = "testuser",
                Password = "Password123!",
                Email = "testuser@example.com",
                PhoneNumber = "123456789",
                Address = "123 Test St",
                City = "Test City"
            };

            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var _controller = new AuthController(_mockUserManager.Object, _mockSignInManager.Object, _configuration);

            var result = await _controller.Register(registerDto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value.GetType().GetProperty("Message").GetValue(okResult.Value, null);
            Assert.Equal("User registered successfully", response);
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_ErrorsBeforeTryToRegister()
        {
            var registerDto = new RegisterDto
            {
                Username = "test",
                Password = "Password123!",
                Email = "testuser",
                PhoneNumber = "12345sd9",
                Address = "",
                City = ""
            };

            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var _controller = new AuthController(_mockUserManager.Object, _mockSignInManager.Object, _configuration);

            var result = await _controller.Register(registerDto);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errors = Assert.IsType<List<IdentityError>>(badRequestResult.Value);

            Assert.Equal(5, errors.Count);
            Assert.Contains(errors, e => e.Code == "shortUsername" && e.Description == "Username must have at least 5 letters.");
            Assert.Contains(errors, e => e.Code == "BadNumber" && e.Description == "Phone number isn't in 123456789 format.");
            Assert.Contains(errors, e => e.Code == "BadEmail" && e.Description == "Check your email adress.");
            Assert.Contains(errors, e => e.Code == "EmptyCity" && e.Description == "City cannot be empty.");
            Assert.Contains(errors, e => e.Code == "EmptyAddress" && e.Description == "Address cannot be empty.");
        }
        [Fact]
        public async Task Register_ReturnsBadRequest_ErrorsAfterTryToRegister()
        {
            var registerDto = new RegisterDto
            {
                Username = "testuser",
                Password = "pas",
                Email = "testuser@test.user",
                PhoneNumber = "123456789",
                Address = "test address",
                City = "test city"
            };
            var failedErrors = new List<IdentityError>
            {
                new IdentityError { Code = "usernameExists", Description = "Username is already registered." },
                new IdentityError { Code = "PasswordTooShort", Description = "Passwords must be at least 6 characters." },
                new IdentityError { Code = "PasswordRequiresNonAlphanumeric", Description = "Passwords must have at least one non alphanumeric character." },
                new IdentityError { Code = "PasswordRequiresDigit", Description = "Passwords must have at least one digit ('0'-'9')." },
                new IdentityError { Code = "PasswordRequiresUpper", Description = "Passwords must have at least one uppercase ('A'-'Z')." }
            };
            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(failedErrors.ToArray()));

            var _controller = new AuthController(_mockUserManager.Object, _mockSignInManager.Object, _configuration);

            var result = await _controller.Register(registerDto);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errors = Assert.IsType<List<IdentityError>>(badRequestResult.Value);

            Assert.Equal(5, errors.Count);
            Assert.Contains(errors, e => e.Code == "usernameExists" && e.Description == "Username is already registered.");
            Assert.Contains(errors, e => e.Code == "PasswordTooShort" && e.Description == "Passwords must be at least 6 characters.");
            Assert.Contains(errors, e => e.Code == "PasswordRequiresNonAlphanumeric" && e.Description == "Passwords must have at least one non alphanumeric character.");
            Assert.Contains(errors, e => e.Code == "PasswordRequiresDigit" && e.Description == "Passwords must have at least one digit ('0'-'9').");
            Assert.Contains(errors, e => e.Code == "PasswordRequiresUpper" && e.Description == "Passwords must have at least one uppercase ('A'-'Z').");
        }
    }
}
