using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using WebApiDemo.Controllers;
using WebApiDemo.Data;
using WebApiDemo.Models;
using WebApiDemo.Models.Entities;

namespace MoneyFlowTests.Controllers
{
    public class UserAccountControllerTests
    {
        private (AppDbContext, Mock<IPasswordHasher<User>>) GetDependencies()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid())
                .Options;

            var context = new AppDbContext(options);
            var hasherMock = new Mock<IPasswordHasher<User>>();

            hasherMock.Setup(h => h.HashPassword(It.IsAny<User>(), It.IsAny<string>()))
                .Returns("hashed_password");

            hasherMock.Setup(h => h.VerifyHashedPassword(
                It.IsAny<User>(),
                "hashed_password",
                It.IsAny<string>()))
                .Returns(PasswordVerificationResult.Success);

            // Добавляем тестового пользователя
            context.Users.Add(new User
            {
                Email = "test@mail.com",
                Password = "hashed_password",
                Name = "Test User"
            });
            context.SaveChanges();

            return (context, hasherMock);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsOk()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid())
                .Options;

            await using var context = new AppDbContext(options);

            var hasher = new PasswordHasher<User>();
            var user = new User
            {
                Email = "test@mail.com",
                Name = "Test User",
                Password = hasher.HashPassword(null, "123456")
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            var controller = new UserAccountController(context);

            var result = await controller.Login(new UserLoginModel
            {
                Email = "test@mail.com",
                Password = "123456"
            });

            Assert.NotNull(result);
            Assert.IsAssignableFrom<IActionResult>(result);
        }


        [Fact]
        public async Task Register_ExistingEmail_ReturnsConflict()
        {
            var (context, hasherMock) = GetDependencies();
            var controller = new UserAccountController(context);

            var result = await controller.Create(new UserRegisterModel
            {
                Email = "test@mail.com",
                Password = "any",
                Name = "Test User"
            });

            var conflict = Assert.IsType<ConflictObjectResult>(result);
            Assert.Equal("Пользователь с таким email уже существует", conflict.Value);
        }

    }
}