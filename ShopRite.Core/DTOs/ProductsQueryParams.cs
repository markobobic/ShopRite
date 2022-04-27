namespace ShopRite.Core.DTOs
{
    public record ProductQueryParams(string SortOrder, bool SortAscending, string Search, string Filter, int? PageNumber, int? Limit, int? AmountFrom, int? AmountTo);
}
