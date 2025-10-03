namespace workoutAPI.Models.Pagination;

public class PaginationMetadata
{
    public int TotalResults { get; set; }
    public int Offset { get; set; }
    public int Number { get; set; }  
    public int Returned { get; set; }
    public bool HasMore { get; set; }
    public bool HasPrevious { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    
    public PageParams? Previous { get; set; }
    public PageParams? Next { get; set; }
    public PageParams? First { get; set; }
    public PageParams? Last { get; set; }
}

