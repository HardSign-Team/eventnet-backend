using X.PagedList;

namespace Eventnet.Api.Helpers;

public static class PagedListExtensions
{
    public static object ToPaginationHeader<T>(this PagedList<T> pagedList,
        Func<int, int, string?> generatePageLink) =>
        new
        {
            previousPageLink = pagedList.HasPreviousPage
                ? generatePageLink(pagedList.PageNumber - 1, pagedList.PageSize)
                : null,
            nextPageLink = pagedList.HasNextPage
                ? generatePageLink(pagedList.PageNumber + 1, pagedList.PageSize)
                : null,
            totalCount = pagedList.TotalItemCount,
            pageSize = pagedList.PageSize,
            currentPage = pagedList.PageNumber,
            totalPages = pagedList.PageCount
        };
}