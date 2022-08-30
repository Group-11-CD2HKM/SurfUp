using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SurfBoardManager.Data;
using System;
using System.Linq;

namespace SurfBoardManager.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new SurfBoardManagerContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<SurfBoardManagerContext>>()))
            {
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
                        Price = 842
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
                        Price = 564
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
                        Price = 562
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
                        Price = 426
                    }
                ); ;
                context.SaveChanges();
            }
        }
    }
}
