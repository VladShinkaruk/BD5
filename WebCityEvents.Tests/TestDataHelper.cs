using WebCityEvents.Models;
using System.Collections.Generic;
using System.Linq;

namespace WebCityEvents.Tests
{
    internal static class TestDataHelper
    {
        public static List<Customer> GetFakeCustomersList()
        {
            return new List<Customer>
            {
                new Customer { CustomerID = 1, FullName = "John Doe", PassportData = "AB123456" },
                new Customer { CustomerID = 2, FullName = "Jane Doe", PassportData = "CD789012" },
                new Customer { CustomerID = 3, FullName = "Sam Smith", PassportData = "EF345678" }
            };
        }

        public static List<Event> GetFakeEventsList()
        {
            return new List<Event>
            {
                new Event
                {
                    EventID = 1,
                    EventName = "Concert",
                    PlaceID = 1,
                    EventDate = new DateTime(2024, 11, 15),
                    TicketPrice = 100,
                    TicketAmount = 200,
                    OrganizerID = 1,
                    Place = new Place
                    {
                        PlaceID = 1,
                        PlaceName = "Main Hall"
                    },
                    Organizer = new Organizer
                    {
                        OrganizerID = 1,
                        FullName = "John Smith"
                    }
                },
                new Event
                {
                    EventID = 2,
                    EventName = "Art Exhibition",
                    PlaceID = 2,
                    EventDate = new DateTime(2024, 12, 1),
                    TicketPrice = 50,
                    TicketAmount = 150,
                    OrganizerID = 2,
                    Place = new Place
                    {
                        PlaceID = 2,
                        PlaceName = "Gallery"
                    },
                    Organizer = new Organizer
                    {
                        OrganizerID = 2,
                        FullName = "Art Inc."
                    }
                }
            };
        }

        public static List<TicketOrder> GetFakeTicketOrdersList()
        {
            return new List<TicketOrder>
        {
            new TicketOrder
            {
                OrderID = 1,
                EventID = 1,
                CustomerID = 1,
                OrderDate = DateTime.UtcNow.AddDays(-10),
                TicketCount = 2
            },
            new TicketOrder
            {
                OrderID = 2,
                EventID = 1,
                CustomerID = 2,
                OrderDate = DateTime.UtcNow.AddDays(-5),
                TicketCount = 4
            },
            new TicketOrder
            {
                OrderID = 3,
                EventID = 2,
                CustomerID = 1,
                OrderDate = DateTime.UtcNow.AddDays(-1),
                TicketCount = 1
            },
            new TicketOrder
            {
                OrderID = 4,
                EventID = 2,
                CustomerID = 3,
                OrderDate = DateTime.UtcNow.AddDays(-2),
                TicketCount = 3
            },
            new TicketOrder
            {
                OrderID = 5,
                EventID = 3,
                CustomerID = 4,
                OrderDate = DateTime.UtcNow,
                TicketCount = 5
            }
        };
        }
    }
}