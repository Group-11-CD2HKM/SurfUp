using Microsoft.AspNetCore.Components;
using SurfUpLibary;
using System.Net.Http.Json;

namespace SurfBoardBlazorWASM.Client.Services
{
    public class BoardPostService : IBoardPostService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly NavigationManager _navigationManager;
        public BoardPostService(IHttpClientFactory httpClientFactory, NavigationManager navManager)
        {
            _httpClientFactory = httpClientFactory;
            _navigationManager = navManager;
        }

        public async Task<List<BoardPost>> GetAllUnrentedBoardPosts()
        {
            HttpClient httpClient = _httpClientFactory.CreateClient("SurfBoardBlazorWASM.PublicServerAPI");

            var bordPosts = await httpClient.GetFromJsonAsync<List<BoardPost>>("api/Boards") ?? new List<BoardPost>();
            return bordPosts;
        }

        public void RentBoard(BoardPost boardPost)
        {
            throw new NotImplementedException();
        }
    }
}
