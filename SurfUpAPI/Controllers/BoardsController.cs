using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SurfUpLibary;

namespace SurfUpAPI.Controllers
{
    [Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    public class BoardsController : ControllerBase
    {
        private readonly SurfBoardManagerContext _context;
        private readonly UserManager<SurfUpUser> _userManager;

        public BoardsController(SurfBoardManagerContext context, UserManager<SurfUpUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [MapToApiVersion("1.0")]
        [HttpGet]
        public async Task<IActionResult> GetAllV1()
        {
            //For ikke-premium brugere
            //Metoden returnerer boards der koster under 500kr.og ikke udlejet.

            var boards = await _context.BoardPost.ToListAsync();
            var unrentedBoards = boards.Where(b => b.IsRented == false && b.Price < 500);
            if (boards == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(unrentedBoards);
            }
        }

        [MapToApiVersion("2.0")]
        [HttpGet]
        public async Task<IActionResult> GetAllV2()
        {
            //For Premium brugere (Brugere der er loggede ind)
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

        [MapToApiVersion("1.0")]
        [MapToApiVersion("2.0")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRentV2(int id, string userId, DateTime? endDate)
        {
            //Metoden sætter et board til at være udlejet til en bestemt bruger

            var boardPost = await _context.BoardPost.FirstOrDefaultAsync(m => m.Id == id);
            var surfUpUser = await _userManager.FindByIdAsync(userId);

            //Brugere der er anonymous må ikke leje flere boards end et
            if (surfUpUser.IsAnonymous)
            {
                var count = _context.BoardPost.Where(b => b.SurfUpUserId == userId).Count();
                if(count >= 1)
                {
                    return Conflict("Only 1 board may be rented by anonymous users.");
                }
            }

            boardPost.RentalDate = DateTime.Now;
            boardPost.RentalDateEnd = endDate;

            try
            {
                _context.Update(boardPost);
                //_context.Attach(surfUpUser); // Required when using sqlite?
                await _context.SaveChangesAsync();
                return Ok(boardPost);
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
        [HttpGet ("UnrentBoards/{userId}")]
        public async Task<IActionResult> GetUnrentBoards(string userId)
        {
            try
            {
                var boards = _context.BoardPost.Include("SurfUpUser").Where(b => b.SurfUpUser.Id.Equals(userId)).ToList();
                foreach (var board in boards)
                {
                    board.RentalDateEnd = DateTime.Now;
                    board.SurfUpUser = null;
                    board.IsRented = false;
                }
                await _context.SaveChangesAsync();
                return Ok(boards);
            }
            catch (NullReferenceException ex)
            {
                return NotFound();
            }
            catch (DbUpdateException ex)
            {
                return Conflict(ex.Message);
            }
        }

    }
}
