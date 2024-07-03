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

            _mockUserManager = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var mockUserClaimsPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<User>>();
            var mockOptionsAccessor = new Mock<IOptions<IdentityOptions>>();
            var mockLogger = new Mock<ILogger<SignInManager<User>>>();

            var mockSignInManager = new Mock<SignInManager<User>>(
                _mockUserManager.Object, mockHttpContextAccessor.Object,
                mockUserClaimsPrincipalFactory.Object, mockOptionsAccessor.Object,
                mockLogger.Object, null);
        }
        private ApplicationDbContext GetInMemoryDbContext(string name)
        {
            return new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
               .UseInMemoryDatabase(databaseName: name)
               .Options);
        }
    }
}
