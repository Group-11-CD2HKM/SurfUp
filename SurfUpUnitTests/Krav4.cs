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
              new SurfUpUser() { Email = "user2@test.com", UserName = "user1@test.com"},
              new SurfUpUser() { Email = "admin@admin.com", UserName = "Admin"}
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
            _userManager.AddToRoleAsync(_users.Last(), "Admin");
            _boardPostsController = new BoardPostsController(_context,_roleManager,_userManager);

            var resetTestModel = _context.BoardPost.Find(1);
            resetTestModel.RentalDateEnd = null;
            resetTestModel.IsRented = false;
        }
        [TestMethod]
        public async Task RentIdBoardGet()
        {
            var view = (await _boardPostsController.Rent(1) as ViewResult);
            RentalViewModel model = view.Model as RentalViewModel;


            Assert.AreEqual(1, model.BoardPost.Id);
            Assert.AreEqual("FishyDaniel", model.BoardPost.Name);
        }

        [TestMethod]
        public async Task RentIdBoardPost()
        {
            var getView = (await _boardPostsController.Rent(1) as ViewResult);
            RentalViewModel getModel = getView.Model as RentalViewModel;

            getModel.RentalPeriod = 5;

            var postResult = await _boardPostsController.Rent(1, getModel);
            var postView = (postResult as ViewResult);
            BoardPost postModel = _context.BoardPost.Find(1);

            string start1 = DateTime.Now.ToString();
            start1 = start1.Substring(0, start1.Length - 3);
            string start2 = postModel.RentalDate.ToString();
            start2 = start2.Substring(0,start2.Length - 3);

            string end1 = DateTime.Now.AddDays(5).ToString();
            end1 = end1.Substring(0, end1.Length - 3);
            string end2 = postModel.RentalDateEnd.ToString();
            end2 = end2.Substring(0, end2.Length - 3);

            Assert.AreEqual(getModel.BoardPost.Id,postModel.Id);
            Assert.AreEqual(start1, start2);
            Assert.AreEqual(end1, end2);
            Assert.AreEqual(true,postModel.IsRented);
        }
    }
}