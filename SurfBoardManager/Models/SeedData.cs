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

        //Metoden her, bliver kørt når programmet starter og lavet 2 checks.
        //først checker den om vi har en registeret admin i vores database og hvis ikke opretter den en default admin.
        //Anden check er om databasen indeholder nogle boardpost, og hvis opretter den nogle.
        public async static Task Initialize(IServiceProvider serviceProvider)
        {
           
            //Henter data'en fra databasen og gemmer dette i "context" variablen.
            using (var context = new SurfBoardManagerContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<SurfBoardManagerContext>>()))
            {
                
                //Sætter "roleManager" variable til typen "RoleManager" med type parameter af "IdentityRole"
                var roleManager = serviceProvider
                .GetRequiredService<RoleManager<IdentityRole>>();
                var roleName = "Admin";
                IdentityResult result;

                //checker om "roleManager" har en existerende "Admin" bruger
                bool roleExist = await roleManager.RoleExistsAsync(roleName);

                if (!roleExist)
                {
                    //Ópretter en ny admin bruger, hvis ingen bruger har en admin rolle. 
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

                // Leder efter boardpost.
                if (context.BoardPost.Any())
                {
                    return;   // DB has been seeded
                }

                //Fylder databasen med 3 default boardpost, hvis ingen var fundet.
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
