using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiDemo.Data;
using WebApiDemo.Models.Entities;
using WebApiDemo.Controllers;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging.Abstractions;
using static MoneyFlowClient.Client.Pages.AdminPanel;
using User = WebApiDemo.Models.Entities.User;

namespace MoneyFlowTests.AdminPanel
{
    public class AdminPanelTests
    {
        [Fact]
        public void GetAdminStats_ReturnsZeroStats_WhenNoDataExists()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb3")
                .Options;

            using var context = new AppDbContext(options);

            var logger = NullLogger<UserAccountController>.Instance;
            var controller = new UserAccountController(context, logger);

            var result = controller.GetAdminStats() as OkObjectResult;

            var json = System.Text.Json.JsonSerializer.Serialize(result.Value);
            var stats = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, System.Text.Json.JsonElement>>(json);

            Assert.Equal(0, stats["TotalUsers"].GetInt32());
            Assert.Equal(0, stats["TotalTransactions"].GetInt32());
            Assert.Equal(0m, stats["TotalBalance"].GetDecimal());
            Assert.Equal(0m, stats["TotalInvestmentsAmount"].GetDecimal());

        }


        [Fact]
        public void GetAdminStats_ReturnsCorrectStats_WhenDataExists()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb2")
                .Options;

            using var context = new AppDbContext(options);

            context.Users.Add(new User { Name = "Nikita", Email = "test@mail.com", RoleId = 1 });
            context.Transactions.Add(new Transaction { Amount = 1000, TransactionType = "Покупка", TransactionDate = DateTime.Now, Comment = "Test" });
            context.Transactions.Add(new Transaction { Amount = 500, TransactionType = "Перевод", TransactionDate = DateTime.Now, Comment = "Test" });
            context.Budgets.Add(new Budget { Amount = 3000 });

            // Исправленная строка - добавлен InvestmentType
            context.Investments.Add(new Investment
            {
                Amount = 1000,
                TransactionType = "BUY",
                TransactionDate = DateTime.Now,
                InvestmentType = "STOCK", 
                UserId = 1, 
                StockId = 1 
            });

            context.SaveChanges();

            var logger = NullLogger<UserAccountController>.Instance;
            var controller = new UserAccountController(context, logger);

            var result = controller.GetAdminStats() as OkObjectResult;

            var json = System.Text.Json.JsonSerializer.Serialize(result.Value);
            var stats = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, System.Text.Json.JsonElement>>(json);

            Assert.Equal(1, stats["TotalUsers"].GetInt32());
            Assert.Equal(2, stats["TotalTransactions"].GetInt32());
            Assert.Equal(3000m, stats["TotalBalance"].GetDecimal());
            Assert.Equal(1000m, stats["TotalInvestmentsAmount"].GetDecimal());
        }




        [Fact]
        public void GetUserByEmail_ReturnsUser_WhenUserExists()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb3")
                .Options;

            using var context = new AppDbContext(options);
            context.Users.Add(new User { Name = "Admin", Email = "admin@mail.com", RoleId = 1 });
            context.SaveChanges();

            var logger = NullLogger<UserAccountController>.Instance;
            var controller = new UserAccountController(context, logger);
            var result = controller.GetUserByEmail("admin@mail.com") as OkObjectResult;
            var user = result.Value as User;

            Assert.NotNull(user);
            Assert.Equal("Admin", user.Name);
        }

        [Fact]
        public void GetRecentUsers_ReturnsEmptyList_WhenNoTransactions()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb4")
                .Options;

            using var context = new AppDbContext(options);
            var logger = NullLogger<UserAccountController>.Instance;
            var controller = new UserAccountController(context, logger);

            var result = controller.GetRecentUsers(1) as OkObjectResult;
            var users = result.Value as List<User>;

            Assert.Empty(users);
        }
    }
}
