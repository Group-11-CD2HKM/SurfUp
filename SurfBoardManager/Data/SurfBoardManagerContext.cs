using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SurfBoardManager.Models;

namespace SurfBoardManager.Data
{
    public class SurfBoardManagerContext : DbContext
    {
        public SurfBoardManagerContext (DbContextOptions<SurfBoardManagerContext> options)
            : base(options)
        {
        }

        public DbSet<SurfBoardManager.Models.BoardPost> BoardPost { get; set; } = default!;
    }
}
