namespace workoutAPI.Models.Pagination;

public class Pagination
{
    public static PaginationMetadata CreateMetadata(
        int totalResults,
        int offset,
        int number,
        int returned
    )
    {
        // Validate data
        if (offset < 0) offset = 0;
        if (number < 1) number = 1;
        if (number > 100) number = 100;
        
        var hasMore = offset + returned < totalResults;
        var hasPrevious = offset > 0;
        var currentPage = (offset / number) + 1;
        var totalPages = (int)Math.Ceiling((double)totalResults / number);

        return new PaginationMetadata
        {
            TotalResults = totalResults,
            Offset = offset,
            Number = number,
            Returned = returned,
            HasMore = hasMore,
            HasPrevious = hasPrevious,
            CurrentPage = currentPage,
            TotalPages = totalPages,

            Previous = hasPrevious
                ? new PageParams
                {
                    Offset = Math.Max(0, offset - number),
                    Number = number
                }
                : null,
            Next = hasMore
                ? new PageParams
                {
                    Offset = offset + number,
                    Number = number
                }
                : null,

            First = new PageParams
            {
                Offset = 0,
                Number = number
            },

            Last = new PageParams
            {
                Offset = Math.Max(0, (totalPages - 1) * number),
                Number = number
            }
        };
    }
}