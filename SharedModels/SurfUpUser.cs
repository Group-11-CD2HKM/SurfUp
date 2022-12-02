

using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace SurfUpLibary
{
    public class SurfUpUser : IdentityUser
    {
        // Extension of the surfupuser, so it can store a list of rented boardposts.
        // The list is nullable, since a user is not required to rent a board.
        [ForeignKey("SurfUpUserId")]
        public List<BoardPost>? BoardPosts { get; set; }
        
        public bool IsAnonymous { get; set; } = false;
        virtual public List<BoardPost>? CreatedBoards { get; set; }
    }
}
