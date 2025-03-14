﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebCityEvents.Data;
using WebCityEvents.Models;
using WebCityEvents.ViewModels;

namespace WebCityEvents.Controllers
{
    public class EventsController : Controller
    {
        private readonly EventContext _context;
        private const int PageSize = 20;

        public EventsController(EventContext context)
        {
            _context = context;
        }

        // GET: Events
        public async Task<IActionResult> Index(int page = 1, string eventName = "", string placeName = "", string organizerName = "", string sortOrder = "Date", string sortDirection = "asc")
        {
            var eventsQuery = _context.Events
                .Include(e => e.Organizer)
                .Include(e => e.Place)
                .AsQueryable();

            if (!string.IsNullOrEmpty(eventName)) eventsQuery = eventsQuery.Where(e => e.EventName.Contains(eventName));
            if (!string.IsNullOrEmpty(placeName)) eventsQuery = eventsQuery.Where(e => e.Place.PlaceName.Contains(placeName));
            if (!string.IsNullOrEmpty(organizerName)) eventsQuery = eventsQuery.Where(e => e.Organizer.FullName.Contains(organizerName));

            eventsQuery = sortOrder switch
            {
                "Tickets" => sortDirection == "asc" ? eventsQuery.OrderBy(e => e.TicketAmount) : eventsQuery.OrderByDescending(e => e.TicketAmount),
                "Price" => sortDirection == "asc" ? eventsQuery.OrderBy(e => e.TicketPrice) : eventsQuery.OrderByDescending(e => e.TicketPrice),
                _ => sortDirection == "asc" ? eventsQuery.OrderBy(e => e.EventDate) : eventsQuery.OrderByDescending(e => e.EventDate),
            };

            var totalEvents = await eventsQuery.CountAsync();
            var totalPages = (int)Math.Ceiling(totalEvents / (double)PageSize);

            var events = await eventsQuery
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .Select(e => new EventViewModel
                {
                    EventID = e.EventID,
                    EventName = e.EventName,
                    PlaceName = e.Place.PlaceName,
                    OrganizerName = e.Organizer.FullName,
                    EventDate = e.EventDate,
                    TicketPrice = e.TicketPrice,
                    TicketAmount = e.TicketAmount
                })
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.EventName = eventName;
            ViewBag.PlaceName = placeName;
            ViewBag.OrganizerName = organizerName;
            ViewBag.SortOrder = sortOrder;
            ViewBag.SortDirection = sortDirection;

            return View(events);
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventDetails = await _context.Events
                .Include(e => e.Organizer)
                .Include(e => e.Place)
                .Where(e => e.EventID == id)
                .Select(e => new EventViewModel
                {
                    EventID = e.EventID,
                    EventName = e.EventName,
                    PlaceName = e.Place.PlaceName,
                    OrganizerName = e.Organizer.FullName,
                    EventDate = e.EventDate,
                    TicketPrice = e.TicketPrice,
                    TicketAmount = e.TicketAmount
                })
                .FirstOrDefaultAsync();

            if (eventDetails == null)
            {
                return NotFound();
            }

            return View(eventDetails);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            ViewData["OrganizerName"] = new SelectList(_context.Organizers, "OrganizerID", "FullName");
            ViewData["PlaceName"] = new SelectList(_context.Places, "PlaceID", "PlaceName");
            return View();
        }

        // POST: Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EventViewModel model)
        {
            if (ModelState.IsValid)
            {
                var newEvent = new Event
                {
                    EventName = model.EventName,
                    PlaceID = model.PlaceID,
                    EventDate = model.EventDate,
                    TicketPrice = model.TicketPrice,
                    TicketAmount = model.TicketAmount,
                    OrganizerID = model.OrganizerID
                };
                _context.Add(newEvent);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["OrganizerName"] = new SelectList(_context.Organizers, "OrganizerID", "FullName", model.OrganizerID);
            ViewData["PlaceName"] = new SelectList(_context.Places, "PlaceID", "PlaceName", model.PlaceID);
            return View(model);
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventDetails = await _context.Events
                .Include(e => e.Organizer)
                .Include(e => e.Place)
                .Where(e => e.EventID == id)
                .Select(e => new EventViewModel
                {
                    EventID = e.EventID,
                    EventName = e.EventName,
                    PlaceID = e.PlaceID,
                    PlaceName = e.Place.PlaceName,
                    OrganizerID = e.OrganizerID,
                    OrganizerName = e.Organizer.FullName,
                    EventDate = e.EventDate,
                    TicketPrice = e.TicketPrice,
                    TicketAmount = e.TicketAmount
                })
                .FirstOrDefaultAsync();

            if (eventDetails == null)
            {
                return NotFound();
            }

            ViewData["OrganizerID"] = new SelectList(_context.Organizers, "OrganizerID", "FullName", eventDetails.OrganizerID);
            ViewData["PlaceID"] = new SelectList(_context.Places, "PlaceID", "PlaceName", eventDetails.PlaceID);

            return View(eventDetails);
        }

        // POST: Events/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EventViewModel model)
        {
            if (id != model.EventID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var eventToUpdate = await _context.Events.FindAsync(id);
                    if (eventToUpdate == null)
                    {
                        return NotFound();
                    }

                    eventToUpdate.EventName = model.EventName;
                    eventToUpdate.PlaceID = model.PlaceID;
                    eventToUpdate.EventDate = model.EventDate;
                    eventToUpdate.TicketPrice = model.TicketPrice;
                    eventToUpdate.TicketAmount = model.TicketAmount;
                    eventToUpdate.OrganizerID = model.OrganizerID;

                    _context.Update(eventToUpdate);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {

                }
            }

            ViewData["OrganizerID"] = new SelectList(_context.Organizers, "OrganizerID", "FullName", model.OrganizerID);
            ViewData["PlaceID"] = new SelectList(_context.Places, "PlaceID", "PlaceName", model.PlaceID);
            return View(model);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventToDelete = await _context.Events
                .Include(e => e.Organizer)
                .Include(e => e.Place)
                .FirstOrDefaultAsync(m => m.EventID == id);

            if (eventToDelete == null)
            {
                return NotFound();
            }

            var eventViewModel = new EventViewModel
            {
                EventID = eventToDelete.EventID,
                EventName = eventToDelete.EventName,
                PlaceName = eventToDelete.Place.PlaceName,
                OrganizerName = eventToDelete.Organizer.FullName,
                EventDate = eventToDelete.EventDate,
                TicketPrice = eventToDelete.TicketPrice,
                TicketAmount = eventToDelete.TicketAmount
            };

            return View(eventViewModel);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Events.FindAsync(id);
            if (@event != null)
            {
                _context.Events.Remove(@event);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.EventID == id);
        }

        public IActionResult ClearFilters()
        {
            Response.Cookies.Delete("EventName");
            Response.Cookies.Delete("PlaceName");
            Response.Cookies.Delete("OrganizerName");

            return RedirectToAction(nameof(Index));
        }
    }
}