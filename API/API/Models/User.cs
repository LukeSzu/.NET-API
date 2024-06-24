using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    [Table("users")]
    public class User : IdentityUser
    {
        public ICollection<Item> Items { get; set; }
        public ICollection<AuctionHistory> AuctionHistories { get; set; }
    }
}
