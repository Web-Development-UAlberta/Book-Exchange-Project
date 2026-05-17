namespace Book_Exchange.Models.DTOs.ExchangeRequest
{
    public class ExchangeListingViewModel
    {
        public Guid Id { get; set; }
        public string Isbn { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public BookCondition Condition { get; set; }
        public decimal Price { get; set; }
    }
}
