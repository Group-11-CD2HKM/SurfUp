namespace SurfBoardManager.Models
{
    public class RentalViewModel
    {
        // Keeps stuff related to rental of boards together.
        // Is injected into the corresponding view.
        public int RentalPeriod { get; set; }
        public BoardPost BoardPost { get; set; }
    }
}
