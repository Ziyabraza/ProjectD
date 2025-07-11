namespace ProjectD.Models;

public class PageManagerTouchpoints : IPageManager
{
    public int PageNumber { get; set; }
    public int TotalPages { get; set; }
    public int TotalRecords { get; set; }
    public Touchpoint[] Touchpoints { get; set; }
    public PageManagerTouchpoints(int page, int pageSize, Touchpoint[] touchpoints)
    {
        PageNumber = page;
        TotalRecords = touchpoints.Length;
        TotalPages = pageSize;
        Touchpoints = touchpoints;
    }
}

