using Microsoft.AspNetCore.Mvc;
using SurfBoardManager.Models;
using SurfUpUnitTests;
using SurfUpLibary;

namespace SurfUpUnitTests
{
    [TestClass]
    public class Krav4
    {


        [TestInitialize]
        public async Task Init()
        {
            await MockHelper.SetUpTestingVariables("test@test.dk");
        }

        [TestMethod]
        public async Task RentIdBoardGet()
        {
            var view = (await MockHelper.BoardPostManager.Rent(1) as ViewResult);
            RentalViewModel model = view.Model as RentalViewModel;


            Assert.AreEqual(1, model.BoardPost.Id);
            Assert.AreEqual("FishyDaniel", model.BoardPost.Name);
        }

        [TestMethod]
        public async Task RentIdBoardPost()
        {
            MockHelper.SetHttpContext();

            var getView = (await MockHelper.BoardPostManager.Rent(1) as ViewResult);
            RentalViewModel getModel = getView.Model as RentalViewModel;

            getModel.RentalPeriod = 5;

            var postResult = await MockHelper.BoardPostManager.Rent(1, getModel);
            var postView = (postResult as ViewResult);
            BoardPost postModel = MockHelper.Context.BoardPost.Find(1);

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