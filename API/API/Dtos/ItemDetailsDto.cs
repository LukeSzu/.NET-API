namespace API.Dtos
{
    public class ItemDetailsDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string SellerUsername { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }

        public bool isAvailable { get; set; }

        public DateTime AddTime { get; set; }

    }
}
