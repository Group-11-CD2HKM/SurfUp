using Microsoft.AspNetCore.Components;
using SurfUpLibary;

namespace SurfBoardManagerBlazorWASM.Services
{
    public class BoardPostService : IBoardPostService
    {
        private readonly HttpClient _httpClient;
        private readonly NavigationManager _navigationManager;
        public BoardPostService(HttpClient http, NavigationManager navManager)
        {
            _httpClient = http;
            _navigationManager = navManager;
        }

        public List<BoardPost> GetAllUnrentedBoardPosts()
        {

        }

        public void RentBoard(BoardPost boardPost)
        {
            throw new NotImplementedException();
        }
    }
}
