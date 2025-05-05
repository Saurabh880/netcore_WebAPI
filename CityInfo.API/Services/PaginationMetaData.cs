namespace CityInfo.API.Services
{
    public class PaginationMetaData
    {
        public PaginationMetaData() { }
        public PaginationMetaData(int totalItemCount, int pageSize, int currentPage)
        {
            TotalItemCount = totalItemCount;
            PageSize = pageSize;
            CurrentPage = currentPage;
            TotalPages = (int)Math.Ceiling(totalItemCount / (double)pageSize); ;
        }

        public int TotalItemCount { get; }
        public int PageSize { get; }
        public int CurrentPage { get; }
        public int TotalPages { get; }
    }
}
