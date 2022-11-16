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

        public void RentBoard(BoardPost boardPost)
        {
            throw new NotImplementedException();
        }

        public void Rentboard(BoardPost boardPost, int rentalPeriod)
        {
            throw new NotImplementedException();
        }
    }
}
