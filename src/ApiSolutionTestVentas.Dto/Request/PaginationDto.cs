namespace ApiSolutionTestVentas.Dto.Request;

public class PaginationDto
{
    public int Page { get; set; } = 1;
    private int _pageSize { get; set; } = 10;
    private int _maxPageSize { get; set; } = 100;

    public int PageSize
    {
        get { return _pageSize; }
        set { _pageSize = (value > _maxPageSize) ? _maxPageSize : value; }
    }
}