using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiDemo.Controllers;
using WebApiDemo.Data;
using WebApiDemo.Models.Entities;

namespace MoneyFlowTests.InvestmentsTest
{
    public class InvestmentTests
    {
        private AppDbContext GetInMemoryDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task GetPortfolio_NoInvestments_ReturnsEmptyList()
        {
            var context = GetInMemoryDbContext(nameof(GetPortfolio_NoInvestments_ReturnsEmptyList));
            var mockPriceService = new Mock<IStockPriceService>();
            var controller = new InvestmentsController(context, mockPriceService.Object);

            var result = await controller.GetPortfolio(userId: 1, period: "month");

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var data = Assert.IsAssignableFrom<IEnumerable<PortfolioDataPoint>>(okResult.Value);
            Assert.Empty(data);
        }

        [Fact]
        public async Task GetProfitability_NoInvestments_ReturnsEmptyList()
        {
            var context = GetInMemoryDbContext(nameof(GetProfitability_NoInvestments_ReturnsEmptyList));
            var mockPriceService = new Mock<IStockPriceService>();
            var controller = new InvestmentsController(context, mockPriceService.Object);

            var result = await controller.GetProfitabilityData(userId: 1, period: "month");

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var data = Assert.IsAssignableFrom<IEnumerable<ProfitabilityDataPoint>>(okResult.Value);
            Assert.Empty(data);
        }

        [Fact]
        public async Task GetPortfolio_OneInvestment_ReturnsDataPoint()
        {
            var context = GetInMemoryDbContext(nameof(GetPortfolio_OneInvestment_ReturnsDataPoint));
            context.Investments.Add(new Investment
            {
                InvestmentId = 1,
                UserId = 1,
                StockId = 2,
                Quantity = 5,
                TransactionType = "BUY",
                InvestmentType = "Stock",
                InvestmentDate = DateTime.UtcNow.AddDays(-3)
            });
            await context.SaveChangesAsync();

            var mockPriceService = new Mock<IStockPriceService>();
            mockPriceService.Setup(s => s.GetHistoricalPrice(2, It.IsAny<DateTime>()))
                            .ReturnsAsync(100m);

            var controller = new InvestmentsController(context, mockPriceService.Object);

            var result = await controller.GetPortfolio(userId: 1, period: "week");

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var data = Assert.IsAssignableFrom<IEnumerable<PortfolioDataPoint>>(okResult.Value);
            Assert.NotEmpty(data);
        }

        [Fact]
        public async Task GetProfitability_OneInvestment_ReturnsDataPoint()
        {
            var context = GetInMemoryDbContext(nameof(GetProfitability_OneInvestment_ReturnsDataPoint));
            context.Investments.Add(new Investment
            {
                InvestmentId = 1,
                UserId = 1,
                StockId = 2,
                Quantity = 5,
                PurchasePrice = 80,
                TransactionType = "BUY",
                InvestmentType = "Stock", 
                InvestmentDate = DateTime.UtcNow.AddDays(-3)
            });
            await context.SaveChangesAsync();

            var mockPriceService = new Mock<IStockPriceService>();
            mockPriceService.Setup(s => s.GetHistoricalPrice(2, It.IsAny<DateTime>()))
                            .ReturnsAsync(100m);

            var controller = new InvestmentsController(context, mockPriceService.Object);

            var result = await controller.GetProfitabilityData(userId: 1, period: "week");

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var data = Assert.IsAssignableFrom<IEnumerable<ProfitabilityDataPoint>>(okResult.Value);
            Assert.NotEmpty(data);
        }

    }
}
