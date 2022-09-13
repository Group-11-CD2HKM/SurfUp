using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SurfBoardManager.Controllers;
using SurfBoardManager.Data;
using SurfBoardManager.Models;

namespace SurfUpUnitTests
{
    [TestClass]
    public class Krav4
    {
        UserManager<SurfUpUser> _userManager;
        SurfBoardManagerContext _context;
        RoleManager<IdentityRole> _roleManager;
        BoardPostsController _boardPostsController;

        private List<SurfUpUser> _users = new List<SurfUpUser>
         {
              new SurfUpUser() { Email = "user1@test.com", UserName = "user1@test.com"},
              new SurfUpUser() { Email = "user2@test.com", UserName = "user1@test.com"}
         };

        private List<IdentityRole> _roles = new List<IdentityRole>
         {
            new IdentityRole("Admin")
         };

        [TestInitialize]
        public void Init()
        {
            var _contextOptions = new DbContextOptionsBuilder<SurfBoardManagerContext>()
                .UseSqlServer("Server=10.56.8.36;Database=DB42;User Id=STUDENT42;Password=OPENDB_42;Trusted_Connection=False;MultipleActiveResultSets=true")
                .Options;
            _context = new SurfBoardManagerContext(_contextOptions);
            _userManager = MockHelper.MockUserManager<SurfUpUser>(_users).Object;
            _roleManager = MockHelper.MockRoleManager<IdentityRole>(_roles).Object;
            _boardPostsController = new BoardPostsController(_context,_roleManager,_userManager);
        }
        [TestMethod]
        public async Task RentIdBoard()
        {
            var testView1 = (await _boardPostsController.Rent(1) as ViewResult);
            
            Assert.AreEqual("FishyDaniel", (testView1.Model as RentalViewModel).BoardPost.Name);
        }
    }
}