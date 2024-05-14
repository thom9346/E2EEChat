
using ChatApi.Core.Entities;
using ChatApi.Core.Interfaces;
using ChatApi.Infrastructure;
using ChatApi.Infrastructure.Repositories;
using ChatApi.WebApi.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ChatApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin", policy =>
                    policy.WithOrigins("http://localhost:4200") // URL of the Angular app
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials()); // Include this only if your frontend needs to send credentials like cookies or auth headers
            });

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddAutoMapper(typeof(MappingProfile));

            builder.Services.AddDbContext<ChatApiContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<IRepository<Message>, MessageRepository>();
            builder.Services.AddScoped<IRepository<User>, UserRepository>();

            builder.Services.AddSignalR();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseCors("AllowSpecificOrigin");

            app.MapControllers();
            app.MapHub<ChatHub>("/chatHub");

            app.Run();
        }
    }
}
