using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebCityEvents.Data;
using WebCityEvents.Models;
using WebCityEvents.ViewModels;

namespace WebCityEvents.Tests
{
    public class TicketOrdersControllerTests
    {
        private readonly DbContextOptions<EventContext> _options;

        public TicketOrdersControllerTests()
        {
            _options = new DbContextOptionsBuilder<EventContext>()
                .UseInMemoryDatabase(databaseName: "TestTicketOrderDatabase")
                .Options;
        }

        private EventContext CreateContext()
        {
            return new EventContext(_options);
        }

        private void SeedDatabase(EventContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            context.TicketOrders.AddRange(TestDataHelper.GetFakeTicketOrdersList());
            context.SaveChanges();
        }

        [Fact]
        public async Task Details_ReturnsNotFound()
        {
            using var context = CreateContext();
            SeedDatabase(context);
            var controller = new TicketOrdersController(context);

            var result = await controller.Details(999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Create_ReturnsViewResult()
        {
            using var context = CreateContext();
            var controller = new TicketOrdersController(context);

            var result = controller.Create();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Edit_UpdatesTicketOrder()
        {
            using var context = CreateContext();
            SeedDatabase(context);
            var controller = new TicketOrdersController(context);

            var updatedOrder = new TicketOrderViewModel
            {
                OrderID = 1,
                TicketCount = 10
            };

            var result = await controller.Edit(1, updatedOrder);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(controller.Index), redirectToActionResult.ActionName);

            var orderToUpdate = await context.TicketOrders.FindAsync(1);
            Assert.Equal(10, orderToUpdate.TicketCount);
        }

        [Fact]
        public async Task Edit_ReturnsNotFound()
        {
            using var context = CreateContext();
            SeedDatabase(context);
            var controller = new TicketOrdersController(context);

            var updatedOrder = new TicketOrderViewModel
            {
                OrderID = 999,
                TicketCount = 5
            };

            var result = await controller.Edit(999, updatedOrder);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenIdIsNull()
        {
            using var context = CreateContext();
            SeedDatabase(context);
            var controller = new TicketOrdersController(context);

            var result = await controller.Delete(null);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteConfirmed_RemovesTicketOrder()
        {
            using var context = CreateContext();
            SeedDatabase(context);
            var controller = new TicketOrdersController(context);

            var result = await controller.DeleteConfirmed(1);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(controller.Index), redirectToActionResult.ActionName);

            var deletedOrder = context.TicketOrders.Find(1);
            Assert.Null(deletedOrder);
        }

        [Fact]
        public async Task SearchEvents_ReturnsEmptyList_WhenTermIsEmpty()
        {
            using var context = CreateContext();
            SeedDatabase(context);
            var controller = new TicketOrdersController(context);

            var result = await controller.SearchEvents(string.Empty);

            var jsonResult = Assert.IsType<JsonResult>(result);
            var events = Assert.IsAssignableFrom<List<string>>(jsonResult.Value);
            Assert.Empty(events);
        }

        [Fact]
        public void ClearFilters_DeletesCookies()
        {
            using var context = CreateContext();
            var controller = new TicketOrdersController(context);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            controller.HttpContext.Response.Cookies.Append("CustomerName", "TestCustomer");

            var result = controller.ClearFilters();

            Assert.Null(controller.HttpContext.Request.Cookies["CustomerName"]);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(controller.Index), redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Create_ReturnsRedirectToAction_WhenModelStateIsValid()
        {
            using var context = CreateContext();
            SeedDatabase(context);
            var controller = new TicketOrdersController(context);

            var newOrder = new TicketOrderViewModel
            {
                EventID = 1,
                CustomerID = 1,
                OrderDate = DateTime.Now,
                TicketCount = 2
            };

            var result = await controller.Create(newOrder);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(controller.Index), redirectToActionResult.ActionName);

            var createdOrder = await context.TicketOrders
                .FirstOrDefaultAsync(o => o.CustomerID == newOrder.CustomerID && o.EventID == newOrder.EventID);
            Assert.NotNull(createdOrder);
            Assert.Equal(newOrder.TicketCount, createdOrder.TicketCount);
        }

        [Fact]
        public async Task Create_ReturnsViewResult_WhenModelStateIsInvalid()
        {
            using var context = CreateContext();
            var controller = new TicketOrdersController(context);

            controller.ModelState.AddModelError("Error", "Invalid model");
            var newOrder = new TicketOrderViewModel();

            var result = await controller.Create(newOrder);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(newOrder, viewResult.Model);
        }

        [Fact]
        public async Task Edit_ReturnsNotFound_WhenOrderDoesNotExist()
        {
            using var context = CreateContext();
            SeedDatabase(context);
            var controller = new TicketOrdersController(context);

            var result = await controller.Edit(999);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}