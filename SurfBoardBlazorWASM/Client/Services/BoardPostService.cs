using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using SurfUpLibary;
using System.Net;
using System.Net.Http.Json;

namespace SurfBoardBlazorWASM.Client.Services
{
    public class BoardPostService : IBoardPostService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly NavigationManager _navigationManager;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        public BoardPostService(IHttpClientFactory httpClientFactory, NavigationManager navManager, AuthenticationStateProvider authenticationStateProvider)
        {
            _httpClientFactory = httpClientFactory;
            _navigationManager = navManager;
            _authenticationStateProvider = authenticationStateProvider;
        }

        // Sends boardpost to API
        // API handles BoardCreator assignment
        public async Task CreateBoardPost(BoardPost boardPost)
        {
            HttpClient httpClient = _httpClientFactory.CreateClient("SurfBoardBlazorWASM.ServerAPI");
            var response = await httpClient.PostAsJsonAsync<BoardPost>("api/v2.0/Boards", boardPost);
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<BoardPost>> GetAllUnrentedBoardPosts()
        {
            List<BoardPost> boardPosts;
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            if (authState.User.Identity.IsAuthenticated)
            {
                HttpClient httpClient = _httpClientFactory.CreateClient("SurfBoardBlazorWASM.ServerAPI");
                boardPosts = await httpClient.GetFromJsonAsync<List<BoardPost>>("api/v2.0/Boards") ?? new List<BoardPost>();
            } else
            {
                HttpClient httpClient = _httpClientFactory.CreateClient("SurfBoardBlazorWASM.PublicServerAPI");
                boardPosts = await httpClient.GetFromJsonAsync<List<BoardPost>>("api/Boards") ?? new List<BoardPost>();
            }
            return boardPosts;
        }

        public async Task RentBoard(BoardPost boardPost, int days)
        {
            if (boardPost != null)
            {
                BoardPost returnBoardPost;
                var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
                if (authState.User.Identity.IsAuthenticated)
                {
                    HttpClient httpClient = _httpClientFactory.CreateClient("SurfBoardBlazorWASM.ServerAPI");
                    returnBoardPost = await httpClient.GetFromJsonAsync<BoardPost>($"api/v2.0/Boards/{boardPost.Id}?days={days}");
                }
                else
                {
                    HttpClient httpClient = _httpClientFactory.CreateClient("SurfBoardBlazorWASM.PublicServerAPI");
                    returnBoardPost = await httpClient.GetFromJsonAsync<BoardPost>($"api/Boards/{boardPost.Id}?days={days}");
                }
            } else
            {
                throw new NullReferenceException("boardPost not set.");
            }
        }
    }
}
