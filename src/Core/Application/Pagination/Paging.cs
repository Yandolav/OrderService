namespace Domain.Entities.Pagination;

public sealed record Paging(int Limit, long Cursor);