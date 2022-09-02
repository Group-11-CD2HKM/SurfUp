using Microsoft.AspNetCore.Identity;

namespace SurfBoardManager.Models
{
    public class SurfUpUser : IdentityUser
    {
        public List<BoardPost>? BoardPosts { get; set; }

    }
}
