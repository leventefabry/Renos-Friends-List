using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace RenosFriendsList.API.Helpers
{
    public static class Extensions
    {
        public static void AddPagination(this HttpResponse response,
            int totalCount, int pageSize, int currentPage, int totalPages,
            string previousPageLink, string nextPageLink)
        {
            var paginationMetadata = new
            {
                totalCount,
                pageSize,
                currentPage,
                totalPages,
                previousPageLink,
                nextPageLink
            };

            response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetadata));
        }
    }
}
