using ChatApi.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApi.Infrastructure
{
    public class ChatApiContext : DbContext
    {
        public ChatApiContext(DbContextOptions<ChatApiContext> options) : base(options)
        {
                
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure entity relationships and properties here
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Message>().ToTable("Messages");

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
        }
    }
}
