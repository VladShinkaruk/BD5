namespace WebCityEvents.ViewModels
{
    public class EventViewModel
    {
        public int EventID { get; set; }
        public string EventName { get; set; }
        public int PlaceID { get; set; }
        public int OrganizerID { get; set; }
        public string PlaceName { get; set; }
        public string OrganizerName { get; set; }
        public DateTime EventDate { get; set; }
        public float TicketPrice { get; set; }
        public int TicketAmount { get; set; }
    }
}
