using Microsoft.AspNetCore.Components;
using SurfUpLibary;
using System.Net.Http.Json;

namespace SurfBoardBlazorWASM.Client.Services
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

        public async Task<List<BoardPost>> GetAllUnrentedBoardPosts()
        {
            return await _httpClient.GetFromJsonAsync<List<BoardPost>>("api/BoadPosts") ?? new List<BoardPost>();
        }

        public void RentBoard(BoardPost boardPost)
        {
            throw new NotImplementedException();
        }
    }
}
