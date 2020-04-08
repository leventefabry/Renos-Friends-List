using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using RenosFriendsList.API.Services.PropertyMapping;

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

        public static IQueryable<T> ApplySort<T>(this IQueryable<T> source, string orderBy,
            Dictionary<string, PropertyMappingValue> mappingDictionary)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (mappingDictionary == null)
            {
                throw new ArgumentNullException(nameof(mappingDictionary));
            }

            if (string.IsNullOrWhiteSpace(orderBy))
            {
                return source;
            }

            // the orderBy string is separated by ",", so we split it.
            var orderByAfterSplit = orderBy.Split(',');

            // future orderBy string
            var orderByQueryList = new List<string>();

            foreach (var orderByClause in orderByAfterSplit)
            {
                // trim the orderBy clause, as it might contain leading
                // or trailing spaces. Can't trim the var in foreach,
                // so use another var.
                var trimmedOrderByClause = orderByClause.Trim();

                // if the sort option ends with with " desc", we order
                // descending, ortherwise ascending
                var orderDescending = trimmedOrderByClause.EndsWith(" desc");

                // remove " asc" or " desc" from the orderBy clause, so we 
                // get the property name to look for in the mapping dictionary
                var indexOfFirstSpace = trimmedOrderByClause.IndexOf(" ", StringComparison.Ordinal);
                var propertyName = indexOfFirstSpace == -1 ?
                    trimmedOrderByClause : trimmedOrderByClause.Remove(indexOfFirstSpace);

                // find the matching property
                if (!mappingDictionary.ContainsKey(propertyName))
                {
                    throw new ArgumentException($"Key mapping for {propertyName} is missing");
                }

                // get the PropertyMappingValue
                var propertyMappingValue = mappingDictionary[propertyName];

                if (propertyMappingValue == null)
                {
                    throw new ArgumentNullException("propertyMappingValue");
                }

                // Run through the property names
                foreach (var destinationProperty in
                    propertyMappingValue.DestinationProperties)
                {
                    // revert sort order if necessary
                    if (propertyMappingValue.Revert)
                    {
                        orderDescending = !orderDescending;
                    }
                    orderByQueryList.Add(destinationProperty +
                        (orderDescending ? " descending" : " ascending"));
                }
            }

            var orderByQuery = string.Join(",", orderByQueryList);
            source = source.OrderBy(orderByQuery);
            return source;
        }
    }
}
