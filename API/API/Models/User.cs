using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    [Table("users")]
    public class User
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("username")]
        public string Username { get; set; }
        [Column("password")]
        public string Password { get; set; }

        // Navigation properties
        public ICollection<Item> Items { get; set; }
        public ICollection<AuctionHistory> AuctionHistories { get; set; }
    }
}
