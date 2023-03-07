namespace HotelListing.Data
{
    public class RequestParams
    {
        const int maxPageSize = 50;
        int pageSize = 10;
        public int PageNumber { get; set; } = 1;
        public int PageSize { 
            get
            {
                return pageSize;
            }
            set
            {
                pageSize = value <= maxPageSize ? value : maxPageSize;
            }
        }
    }
}
