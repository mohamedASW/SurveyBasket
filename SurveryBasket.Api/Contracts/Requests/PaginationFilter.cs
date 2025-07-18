namespace SurveryBasket.Api.Contracts.Requests;

public record PaginationFilter
(
    int PageSize = 10,
    int PageNumber =1,
    string? SearchKey =null,
    string? SortBy= null,
    string SortDirection = "ASC"
);
