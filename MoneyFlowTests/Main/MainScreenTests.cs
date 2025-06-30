using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiDemo.Controllers;
using WebApiDemo.Data;
using WebApiDemo.Models.Entities;

namespace MoneyFlowTests.Main
{
    public class MainScreenTests
    {
        [Fact]
        public void GenerateBalanceHistory_OneExpenseTransaction_UpdatesBalanceCorrectly()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("TestDb1")
                .Options;

            using var context = new AppDbContext(options);
            context.Budgets.Add(new Budget { UserId = 1, CategoryId = 1, Amount = 1000 });
            context.SaveChanges();

            var controller = new BalanceController(context, NullLogger<TransactionController>.Instance);

            var transactions = new List<BalanceController.TransactionDto>
    {
        new BalanceController.TransactionDto
        {
            TransactionDate = DateTime.UtcNow,
            UserId = 1,
            Amount = -200
        }
    };

            var history = controller.GenerateBalanceHistory(1, transactions, DateTime.UtcNow.AddDays(-1));

            Assert.Equal(2, history.Count); 
            Assert.Equal(800, history.Last().Balance);
        }

        [Fact]
        public void GenerateBalanceHistory_IncomeFromRelatedUser_IncreasesBalance()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("TestDb3")
                .Options;

            using var context = new AppDbContext(options);
            context.Budgets.Add(new Budget { UserId = 3, CategoryId = 1, Amount = 1000 });
            context.SaveChanges();

            var controller = new BalanceController(context, NullLogger<TransactionController>.Instance);

            var transactions = new List<BalanceController.TransactionDto>
    {
        new BalanceController.TransactionDto
        {
            TransactionDate = DateTime.UtcNow,
            RelatedUserId = 3,
            Amount = 300
        }
    };

            var history = controller.GenerateBalanceHistory(3, transactions, DateTime.UtcNow.AddDays(-1));

            Assert.Equal(2, history.Count);
            Assert.Equal(1300, history.Last().Balance);
        }
        [Fact]
        public void GenerateBalanceHistory_NoTransactions_ReturnsOnlyStartPoint()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("TestDb2")
                .Options;

            using var context = new AppDbContext(options);
            context.Budgets.Add(new Budget { UserId = 2, CategoryId = 1, Amount = 500 });
            context.SaveChanges();

            var controller = new BalanceController(context, NullLogger<TransactionController>.Instance);

            var transactions = new List<BalanceController.TransactionDto>();

            var history = controller.GenerateBalanceHistory(2, transactions, DateTime.UtcNow.AddDays(-1));

            Assert.Single(history);
            Assert.Equal(500, history.First().Balance);
        }

    }
}
