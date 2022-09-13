using Microsoft.AspNetCore.Identity;
using Moq;
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
        public static Mock<UserManager<TUser>> MockUserManager<TUser>(List<TUser> ls) where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            var mgr = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
            mgr.Object.UserValidators.Add(new UserValidator<TUser>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<TUser>());

            mgr.Setup(x => x.DeleteAsync(It.IsAny<TUser>())).ReturnsAsync(IdentityResult.Success);
            mgr.Setup(x => x.CreateAsync(It.IsAny<TUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success).Callback<TUser, string>((x, y) => ls.Add(x));
            mgr.Setup(x => x.UpdateAsync(It.IsAny<TUser>())).ReturnsAsync(IdentityResult.Success);
            mgr.Setup(x => x.GetUserAsync(null)).ReturnsAsync(ls.FirstOrDefault());

            return mgr;
        }

        public static Mock<RoleManager<TRole>> MockRoleManager<TRole>(List<TRole> ls) where TRole : class
        {
            var store = new Mock<IRoleStore<TRole>>();
            var mgr = new Mock<RoleManager<TRole>>(store.Object, null, null, null, null);
            mgr.Object.RoleValidators.Add(new RoleValidator<TRole>());

            mgr.Setup(x => x.DeleteAsync(It.IsAny<TRole>())).ReturnsAsync(IdentityResult.Success);
            mgr.Setup(x => x.CreateAsync(It.IsAny<TRole>())).ReturnsAsync(IdentityResult.Success);
            mgr.Setup(x => x.UpdateAsync(It.IsAny<TRole>())).ReturnsAsync(IdentityResult.Success);

            return mgr;
        }
    }
}
