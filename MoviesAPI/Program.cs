
using System.Security.Cryptography.Xml;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MoviesAPI.Data;

namespace MoviesAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            var ConnectionString = builder.Configuration.GetConnectionString(name: "DefaultConnection")
                                              ?? throw new InvalidCastException("No Connections String was Found");
            builder.Services.AddDbContext<ApplicationDbContext>(options => 
            options.UseSqlServer(ConnectionString));
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Movies",
                Contact = new OpenApiContact
                {
                    Name = "Ahmed Mohamed",
                    Email = "ahmedhamouda18hhh@gmail.com",
                    Url = new Uri("https://www.linkedin.com/in/ahmed-m-youssif-170838290")
                }
            });

            options.AddSecurityDefinition(name: "Bearer", securityScheme:new OpenApiSecurityScheme
            {
                Name = "Ahuothrization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "jwt",
                In = ParameterLocation.Header,
                Description = "Enter your jwt key"

            });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                  {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Name = "Bearer",
                        In = ParameterLocation.Header

                    },
                    new List<string>()
                  }
                });

         });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
