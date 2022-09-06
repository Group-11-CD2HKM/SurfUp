using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SurfBoardManager.Data;
using System;
using System.Linq;

namespace SurfBoardManager.Models
{
    public static class SeedData
    {
        public async static Task Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new SurfBoardManagerContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<SurfBoardManagerContext>>()))
            {
                

                var roleManager = serviceProvider
                .GetRequiredService<RoleManager<IdentityRole>>();
                var roleName = "Admin";
                IdentityResult result;

                bool roleExist = await roleManager.RoleExistsAsync(roleName);

                if (!roleExist)
                {
                    result = await roleManager
                    .CreateAsync(new IdentityRole(roleName));
                    if (result.Succeeded)
                    {
                        var userManager = serviceProvider
                            .GetRequiredService<UserManager<SurfUpUser>>();
                        var config = serviceProvider
                            .GetRequiredService<IConfiguration>();
                        var admin = await userManager
                            .FindByEmailAsync(config["AdminCredentials:Email"]);

                        if (admin == null)
                        {
                            admin = new SurfUpUser()
                            {
                                UserName = config["AdminCredentials:Email"],
                                Email = config["AdminCredentials:Email"],
                                EmailConfirmed = true
                            };
                            result = await userManager
                                .CreateAsync(admin, config["AdminCredentials:Password"]);
                            if (result.Succeeded)
                            {
                                result = await userManager
                                    .AddToRoleAsync(admin, roleName);
                                if (!result.Succeeded)
                                {
                                    // todo: process errors
                                }
                            }
                        }
                    }
                }

                // Look for any movies.
                if (context.BoardPost.Any())
                {
                    return;   // DB has been seeded
                }

                context.BoardPost.AddRange(
                    new BoardPost
                    {
                        Name = "FishyDaniel",
                        Width = 40,
                        Length = 60,
                        Thickness = 74,
                        Volume = 600,
                        BoardType = BoardPost.Type.Fish,
                        Equipment = "Paddle",
                        Price = 842,
                        BoardImage = "https://surf-ski.dk/media/catalog/product/cache/7/thumbnail/256x/9df78eab33525d08d6e5fb8d27136e95/b/i/bic_5_10x.jpg"
                    },

                    new BoardPost
                    {
                        Name = "FunnyBone",
                        Width = 57,
                        Length = 65,
                        Thickness = 89,
                        Volume = 400,
                        BoardType = BoardPost.Type.Funboard,
                        Equipment = "Paddle, Leash",
                        Price = 564,
                        BoardImage = "https://surf-ski.dk/media/catalog/product/cache/7/thumbnail/256x/9df78eab33525d08d6e5fb8d27136e95/b/i/bic_5_10x.jpg"
                    },

                    new BoardPost
                    {
                        Name = "SuhhDude",
                        Width = 46,
                        Length = 76,
                        Thickness = 96,
                        Volume = 859,
                        BoardType = BoardPost.Type.SUP,
                        Equipment = "Paddle, Bone",
                        Price = 562,
                        BoardImage = "https://surf-ski.dk/media/catalog/product/cache/7/thumbnail/256x/9df78eab33525d08d6e5fb8d27136e95/b/i/bic_5_10x.jpg"
                    },

                    new BoardPost
                    {
                        Name = "ShortyBoard",
                        Width = 76,
                        Length = 21,
                        Thickness = 56,
                        Volume = 259,
                        BoardType = BoardPost.Type.Shortboard,
                        Equipment = "Midget",
                        Price = 426,
                        BoardImage = "https://surf-ski.dk/media/catalog/product/cache/7/thumbnail/256x/9df78eab33525d08d6e5fb8d27136e95/b/i/bic_5_10x.jpg"
                    }
                );
                context.SaveChanges();
            }
        }
    }
}
