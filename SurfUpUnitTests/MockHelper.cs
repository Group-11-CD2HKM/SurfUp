using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SurfBoardManager.Controllers;
using SurfBoardManager.Data;
using SurfBoardManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace SurfUpUnitTests
{
    internal static class MockHelper
    {
        public static UserManager<SurfUpUser> UserManager { get; private set; }
        public static SurfBoardManagerContext Context { get; private set; }
        public static RoleManager<IdentityRole> RoleManager { get; private set; }
        public static BoardPostsController BoardPostManager { get; private set; }
        public static IHttpContextAccessor HttpContextAccessor { get; private set; }
        public static HttpContext HttpContext { get; private set; }
        /// <summary>
        /// Set up all needed injections objects needed for testing a class.
        /// The following static properties can be used to access the objects the controlelr needs injected.
        /// Use the static properties to access the object, which the controller needs injected.
        /// </summary>
        /// <param name="userEmail">Email of the single test user we will add to the HttpContext. (make it match seeddata for great success!)</param>
        /// <returns>Task</returns>
        public static async Task SetUpTestingVariables(string userEmail)
        {
            // Create SQLite database
            var dbOptionsBuilder = new DbContextOptionsBuilder<SurfBoardManagerContext>();
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            // Do the builder thingie like in the program.cs and such.
            var builder = WebApplication.CreateBuilder();

            // Add the DBContext
            builder.Services.AddDbContext<SurfBoardManagerContext>(options =>
                options.UseSqlite(connection));

            // Dunno what this does, is it needed?
            //using (var ctx = new SurfBoardManagerContext(dbOptionsBuilder.Options))
            //{
            //    ctx.Database.EnsureCreated();
            //}

            // Do the identity stuffs also like the program.cs
            builder.Services.AddIdentity<SurfUpUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<SurfBoardManagerContext>()
                .AddTokenProvider<DataProtectorTokenProvider<SurfUpUser>>(TokenOptions.DefaultProvider);

            // Do the authorize stuffs program.cs.
            builder.Services.AddAuthorization(options => options.AddPolicy("RequiredAdminRole", policy => policy.RequireRole("Admin")));

            builder.Services.AddRazorPages();

            // Controller stuffs
            builder.Services.AddControllersWithViews();

            // HttpContextAccessor (is used in our code for injecting dummy (and real) users).
            builder.Services.AddHttpContextAccessor();

            // Build webapp.
            var app = builder.Build();

            // Pull out controllers, _rolemanager etc. for later injection into the controller classes we are testing.
            Context = new SurfBoardManagerContext(
                app.Services.GetRequiredService<
                    DbContextOptions<SurfBoardManagerContext>>());
            Context.Database.EnsureCreated();
            RoleManager = app.Services
                .GetRequiredService<RoleManager<IdentityRole>>();
            UserManager = app.Services
                            .GetRequiredService<UserManager<SurfUpUser>>();
            HttpContextAccessor = app.Services.GetRequiredService<IHttpContextAccessor>();

            // Controller under test.
            BoardPostManager = new BoardPostsController(Context, RoleManager, UserManager, HttpContextAccessor);
            // Do the context thingie. This is reset by asp.net sometimes, so do it again it before actually testing a method.
            SetHttpContext();

            // Initialize seeddata into SQLite server.
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                await SeedData.Initialize(services);
            }

            // Maybe try loggin in using a login manager?
            var identity = new GenericIdentity(userEmail, "test");
            var contextUser = new ClaimsPrincipal(identity); //add claims as needed

            //...then set user and other required properties on the httpContext as needed
            // THIS IS NOT THE SAME AS ACTUAL AUTHORIZATION!
            // It only gives a plausible user which matches our seddata (so we can pulle info from the test SQLite database).
            HttpContext = new DefaultHttpContext()
            {
                User = contextUser
            };
        }
        /// <summary>
        /// Resets the HttpContext.
        /// Asp.Net apaprently semirandomly resets the HttpContext?
        /// So use this at the start of test methods.
        /// </summary>
        public static void SetHttpContext()
        {
            HttpContextAccessor.HttpContext = HttpContext;
        }
    }
}
