namespace ProjectD.Models;

public class PageManagerFlights : IPageManager
{
    public int PageNumber { get; set; }
    public int TotalPages { get; set; }
    public int TotalRecords { get; set; }
    public int TotalItems { get; set; }
    public Flight[] Flights { get; set; }
    public PageManagerFlights(int page, int pageSize, int totalItems, Flight[] flights)
    {
        PageNumber = page;
        TotalRecords = flights.Length;
        TotalPages = pageSize;
        TotalItems = totalItems;
        Flights = flights;
    }
}