using ETickets.Data;
using ETickets.Repository;
using Microsoft.EntityFrameworkCore;
using ETickets.Repository.IRepository;
using ETickets.Models;
using Microsoft.AspNetCore.Identity;
using ETickets.Models.ViewModel;
using Stripe;

namespace ETickets
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            #region DB Services
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
            StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

            builder.Services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            #endregion
            #region Models Services
            builder.Services.AddScoped<ICinemaMovieRepository, CinemaMovieRepository>();
            builder.Services.AddScoped<IActorMovieRepository, ActorMovieRepository>();
            builder.Services.AddScoped<IActorRepository, ActorRepository>();
            builder.Services.AddScoped<IMovieRepository, MovieRepository>();
            builder.Services.AddScoped<ICinemaRepository, CinemaRepository>();
            builder.Services.AddScoped<ICategoriesRepository,CategoriesRepository>();
            builder.Services.AddScoped<ITicketsCartRepository, TicketsCartRepository>();
            builder.Services.AddScoped<ITicketsCinemaRepository, TicketsCinemaRepository>();
            //builder.Services.AddScoped<TicketNumber>();
            #endregion

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
