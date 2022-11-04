using SurfUpLibary;

namespace SurfBoardManagerBlazorWASM.Services
{
    public interface IBoardPostService
    {
        public List<BoardPost> GetAllUnrentedBoardPosts();

        public void RentBoard(BoardPost boardPost);
    }
}
