using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    [Table("auction_history")]
    public class AuctionHistory
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("item_id")]
        public int ItemId { get; set; }
        [Column("user_id")]
        public string UserId { get; set; }
        [Column("title")]
        public string Title { get; set; }
        [Column("description")]
        public string Description { get; set; }
        [Column("price")]
        public decimal Price { get; set; }
        [Column("sold_date")]
        public DateTime? SoldDate { get; set; }

        // Navigation properties
        public Item Item { get; set; }
        public User User { get; set; }
    }
}
