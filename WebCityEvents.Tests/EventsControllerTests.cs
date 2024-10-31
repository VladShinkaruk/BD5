using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebCityEvents.Data;
using WebCityEvents.ViewModels;

namespace WebCityEvents.Tests
{
    public class EventsControllerTests
    {
        private readonly DbContextOptions<EventContext> _options;

        public EventsControllerTests()
        {
            _options = new DbContextOptionsBuilder<EventContext>()
                .UseInMemoryDatabase(databaseName: "TestEventDatabase")
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

            context.Events.AddRange(TestDataHelper.GetFakeEventsList());
            context.SaveChanges();
        }

        [Fact]
        public async Task Index_FiltersEvents()
        {
            using var context = CreateContext();
            SeedDatabase(context);
            var controller = new EventsController(context);

            var result = await controller.Index(eventName: "Concert") as ViewResult;

            var events = Assert.IsAssignableFrom<List<EventViewModel>>(result.Model);
            Assert.Single(events);
            Assert.Equal("Concert", events.First().EventName);
        }

        [Fact]
        public async Task Index_SortsEvents()
        {
            using var context = CreateContext();
            SeedDatabase(context);
            var controller = new EventsController(context);

            var result = await controller.Index(sortOrder: "Price", sortDirection: "desc") as ViewResult;

            var events = Assert.IsAssignableFrom<List<EventViewModel>>(result.Model);
            Assert.Equal("Concert", events.First().EventName);
            Assert.Equal("Art Exhibition", events.Last().EventName);
        }

        [Fact]
        public async Task Details_ReturnsNotFound()
        {
            using var context = CreateContext();
            SeedDatabase(context);
            var controller = new EventsController(context);

            var result = await controller.Details(999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Create_ReturnsViewResult()
        {
            using var context = CreateContext();
            var controller = new EventsController(context);

            var result = controller.Create();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Create_CreatesEvent()
        {
            using var context = CreateContext();
            var controller = new EventsController(context);

            var newEvent = new EventViewModel
            {
                EventName = "New Event",
                PlaceID = 1,
                EventDate = DateTime.UtcNow,
                TicketPrice = 150,
                TicketAmount = 100,
                OrganizerID = 1
            };

            var result = await controller.Create(newEvent);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(controller.Index), redirectToActionResult.ActionName);

            var createdEvent = context.Events.SingleOrDefault(e => e.EventName == "New Event");
            Assert.NotNull(createdEvent);
            Assert.Equal(150, createdEvent.TicketPrice);
        }

        [Fact]
        public async Task Edit_UpdatesEvent()
        {
            using var context = CreateContext();
            SeedDatabase(context);
            var controller = new EventsController(context);

            var updatedEvent = new EventViewModel
            {
                EventID = 1,
                EventName = "Updated Concert",
                TicketPrice = 100
            };

            var result = await controller.Edit(1, updatedEvent);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(controller.Index), redirectToActionResult.ActionName);

            var eventToUpdate = await context.Events.FindAsync(1);
            Assert.Equal("Updated Concert", eventToUpdate.EventName);
        }

        [Fact]
        public async Task Edit_ReturnsNotFound()
        {
            using var context = CreateContext();
            SeedDatabase(context);
            var controller = new EventsController(context);

            var updatedEvent = new EventViewModel
            {
                EventID = 999,
                EventName = "Non-existent Event"
            };

            var result = await controller.Edit(999, updatedEvent);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_ReturnsEventForEdit()
        {
            using var context = CreateContext();
            SeedDatabase(context);
            var controller = new EventsController(context);

            var result = await controller.Edit(1) as ViewResult;

            var model = Assert.IsType<EventViewModel>(result.Model);
            Assert.Equal("Concert", model.EventName);
        }

        [Fact]
        public async Task Delete_ReturnsEventForDelete()
        {
            using var context = CreateContext();
            SeedDatabase(context);
            var controller = new EventsController(context);

            var result = await controller.Delete(1) as ViewResult;

            var model = Assert.IsType<EventViewModel>(result.Model);
            Assert.Equal("Concert", model.EventName);
        }

        [Fact]
        public async Task DeleteConfirmed_RemovesEvent()
        {
            using var context = CreateContext();
            SeedDatabase(context);
            var controller = new EventsController(context);

            var result = await controller.DeleteConfirmed(1);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(controller.Index), redirectToActionResult.ActionName);

            var deletedEvent = context.Events.Find(1);
            Assert.Null(deletedEvent);
        }

        [Fact]
        public void ClearFilters_DeletesCookies()
        {
            using var context = CreateContext();
            var controller = new EventsController(context);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            controller.HttpContext.Response.Cookies.Append("EventName", "TestEvent");

            var result = controller.ClearFilters();

            Assert.Null(controller.HttpContext.Request.Cookies["EventName"]);
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(controller.Index), redirectToActionResult.ActionName);
        }
    }
}