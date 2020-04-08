namespace RenosFriendsList.API.ResourceParameters
{
    public abstract class BaseResourceParameters
    {
        // Pagination
        private const int maxPageSize = 20;
        public int PageNumber { get; set; } = 1;
        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > maxPageSize) ? maxPageSize : value;
        }

        // Order By
        public string OrderBy { get; set; }

        // Shaping data
        public string Fields { get; set; }
    }
}
