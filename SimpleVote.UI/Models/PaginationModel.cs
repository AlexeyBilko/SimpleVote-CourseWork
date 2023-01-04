namespace SimpleVote.UI.Models
{
    public class PaginationModel
    {
        public int TotalPagesInOneContainer { get; set; } = 2;
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int QuantityPages { get; set; }
        public int QuantityPaginationContainers { get; set; }

        public void CalculateQuantityPages()
        {
            if (TotalCount % PageSize == 0)
            {
                QuantityPages= TotalCount / PageSize;
            }
            else QuantityPages= (TotalCount / PageSize)+1;
        }
        public void CalculateQuantityPaginationContainers()
        {
            if (QuantityPages % TotalPagesInOneContainer == 0)
            {
                QuantityPaginationContainers = QuantityPages / TotalPagesInOneContainer;
            }
            else QuantityPaginationContainers = (QuantityPages / TotalPagesInOneContainer)+1;
        }
    }
}
