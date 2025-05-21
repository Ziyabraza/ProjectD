namespace ProjectD.Models;
public class PageManager
{
    public int PageNumber { get; set; }
    public int TotalPages { get; set; }
    public int TotalTouchpointRecords { get; set; }
    public Touchpoint[] Touchpoints { get; set; }
    public PageManager(int page, int pageSize, Touchpoint[] touchpoints)
    {
        PageNumber = page;
        TotalTouchpointRecords = touchpoints.Length;
        TotalPages = pageSize;
        Touchpoints = touchpoints;
    }
}