namespace API.Dtos
{
    public class ItemDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string SellerUsername { get; set; }
    }
}
