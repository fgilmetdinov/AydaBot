using Microsoft.EntityFrameworkCore;

namespace AydaBot.SqliteStorage
{
    public class DataContext : DbContext
    {
        public DbSet<Serial> Serials { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<SerialMessage> SerialMessages { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=ayda.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Subscription>()
                .HasKey(c => new { c.SerialId, c.UserId });
        }
    }
}
