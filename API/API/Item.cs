﻿using System.ComponentModel.DataAnnotations.Schema;

namespace API
{
    [Table("items")]
    public class Item
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("user_id")]
        public int UserId { get; set; }
        [Column("title")]
        public string Title { get; set; }
        [Column("description")]
        public string Description { get; set; }
        [Column("price")]
        public decimal Price { get; set; }
        [Column("is_available")]
        public bool IsAvailable { get; set; }
        [Column("buyer_id")]
        public int? BuyerId { get; set; }

        // Navigation properties
        public User User { get; set; }
        public User Buyer { get; set; }
        public ICollection<AuctionHistory> AuctionHistories { get; set; }
    }
}