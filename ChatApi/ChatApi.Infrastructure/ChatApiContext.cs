using ChatApi.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatApi.Infrastructure
{
    public class ChatApiContext : DbContext
    {

        //dotnet ef migrations add AddConnectionEntity --project ChatApi.Infrastructure --startup-project ChatApi.WebApi

        public ChatApiContext(DbContextOptions<ChatApiContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Connection> Connections { get; set; }  // Add this line

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure entity relationships and properties here
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Message>().ToTable("Messages");
            modelBuilder.Entity<Connection>().ToTable("Connections");  // Add this line

            modelBuilder.Entity<Message>()
                .Property(m => m.MessageId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Recipient)
                .WithMany()
                .HasForeignKey(m => m.RecipientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Connection>()  // Add this block to configure the relationship
                .HasOne(c => c.User)
                .WithMany(u => u.Connections)
                .HasForeignKey(c => c.UserId);
        }
    }
}
