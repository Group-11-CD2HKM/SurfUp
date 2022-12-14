

using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using SharedModels;

namespace SurfUpLibary
{
    public class SurfUpUser : IdentityUser
    {
        // Extension of the surfupuser, so it can store a list of rented boardposts.
        // The list is nullable, since a user is not required to rent a board.
        [ForeignKey("SurfUpUserId")]
        public List<BoardPost>? BoardPosts { get; set; }
        
        public bool IsAnonymous { get; set; } = false;

        public bool IsRenter { get; set; }

        public UserAddress? Address { get; set; }
        
        virtual public List<BoardPost>? CreatedBoards { get; set; }
    }
}
