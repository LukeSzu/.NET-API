using API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace API.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet properties go here
        public DbSet<Item> Items { get; set; }
        new public DbSet<User> Users { get; set; }
        public DbSet<AuctionHistory> AuctionHistories { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)

        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdentityUserLogin<string>>().HasKey(x => new { x.LoginProvider, x.ProviderKey });
            modelBuilder.Entity<IdentityUserRole<string>>().HasKey(x => new { x.UserId, x.RoleId });
            modelBuilder.Entity<IdentityUserToken<string>>().HasKey(x => new { x.UserId, x.LoginProvider, x.Name });
            // Relacja między Item a User (właściciel)
            modelBuilder.Entity<Item>()
                .HasOne(item => item.User)
                .WithMany(user => user.Items)
                .HasForeignKey(item => item.UserId)
                .OnDelete(DeleteBehavior.Restrict); // lub .Cascade dla zachowania referencyjnego

            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .ValueGeneratedOnAdd(); // Możesz dodać to, jeśli Id jest generowane automatycznie

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