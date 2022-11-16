using SurfUpLibary;

namespace SurfBoardBlazorWASM.Client.Services
{
    public interface IBoardPostService
    {
        public Task<List<BoardPost>> GetAllUnrentedBoardPosts();

        public void RentBoard(BoardPost boardPost);

        public void Rentboard(BoardPost boardPost, int rentalPeriod);
    }
}
