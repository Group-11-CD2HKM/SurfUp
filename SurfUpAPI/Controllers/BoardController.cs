using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SurfUpLibary;

namespace SurfUpAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoardController : ControllerBase
    {
        private readonly SurfBoardManagerContext _context;
        private readonly UserManager<SurfUpUser> _userManager;

        public BoardController(SurfBoardManagerContext context, UserManager<SurfUpUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            //Metoden returnerer alle boards der ikke er udlejet

            var boards = await _context.BoardPost.ToListAsync();
            var unrentedBoards = boards.Where(b => b.IsRented == false);
            if(boards == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(unrentedBoards);
            }
        }

        [HttpGet ("{id}")]
        public async Task<IActionResult> Rent(int id, string userId, DateTime? endDate)
        {
            //Metoden sætter et board til at være udlejet til en bestemt bruger

            var boardPost = await _context.BoardPost.FirstOrDefaultAsync(m => m.Id == id);
            var surfUpUser = await _userManager.FindByIdAsync(userId);

            boardPost.RentalDate = DateTime.Now;
            boardPost.RentalDateEnd = endDate;
            boardPost.SurfUpUser = surfUpUser;

            try
            {
                _context.Update(boardPost);
                //_context.Attach(surfUpUser); // Required when using sqlite?
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                //Finder den entity der var involveret i exception
                var exceptionEntry = ex.Entries.Single();
                //Trækker det enkelte objekt ud og hardcaster til et BoardPost objekt
                var clientValues = (BoardPost)exceptionEntry.Entity;
                //Forespørger databasen for at finde frem til de nye værdier der ligger i databasen
                var databaseEntry = exceptionEntry.GetDatabaseValues();
                return Conflict(ex.Message);
            }
        }

    }
}
