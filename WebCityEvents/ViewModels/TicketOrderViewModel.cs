namespace WebCityEvents.ViewModels
{
    public class TicketOrderViewModel
    {
        public int OrderID { get; set; }
        public int EventID { get; set; }
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string EventName { get; set; }
        public DateTime OrderDate { get; set; }
        public int TicketCount { get; set; }
    }
}