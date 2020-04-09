using System.Collections.Generic;

namespace RenosFriendsList.API.Models.Dog
{
    public class DogsLinkedCollectionResource
    {
        public DogsLinkedCollectionResource(IEnumerable<IDictionary<string, object>> value, IEnumerable<LinkDto> links)
        {
            Value = value;
            Links = links;
        }

        public IEnumerable<IDictionary<string, object>> Value { get; set; }
        public IEnumerable<LinkDto> Links { get; set; }
    }
}
