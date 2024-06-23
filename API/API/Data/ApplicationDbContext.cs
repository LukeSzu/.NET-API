using API.Models;
using Microsoft.EntityFrameworkCore;


namespace API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet properties go here
        public DbSet<Item> Items { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<AuctionHistory> AuctionHistories { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)

        {
            // Relacja między Item a User (właściciel)
            modelBuilder.Entity<Item>()
                .HasOne(item => item.User)
                .WithMany(user => user.Items)
                .HasForeignKey(item => item.UserId)
                .OnDelete(DeleteBehavior.Restrict); // lub .Cascade dla zachowania referencyjnego

            // Relacja między Item a Buyer (kupujący)
            modelBuilder.Entity<Item>()
                .HasOne(item => item.Buyer)
                .WithMany()
                .HasForeignKey(item => item.BuyerId)
                .OnDelete(DeleteBehavior.SetNull); // Kupujący może być null, jeśli nie jest ustawiony

            // Relacja między AuctionHistory a Item
            modelBuilder.Entity<AuctionHistory>()
                .HasOne(history => history.Item)
                .WithMany(item => item.AuctionHistories)
                .HasForeignKey(history => history.ItemId)
                .OnDelete(DeleteBehavior.Cascade); // Usunięcie historii aukcji, gdy element jest usunięty
        }
    }
}