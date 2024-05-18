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
        //commands i need to remember:
        //dotnet ef migrations add AddConnectionEntity --project ChatApi.Infrastructure --startup-project ChatApi.WebApi
        //dotnet ef database update --project ChatApi.Infrastructure --startup-project ChatApi.WebApi
        public ChatApiContext(DbContextOptions<ChatApiContext> options) : base(options)
        {
                
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Message>().ToTable("Messages");

            modelBuilder.Entity<Message>()
                .Property(m => m.MessageId)
                .ValueGeneratedOnAdd();
        }
    }
}
