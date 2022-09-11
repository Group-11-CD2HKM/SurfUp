using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SurfBoardManager.Data;
using SurfBoardManager.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace SurfBoardManager.Controllers
{
    public class BoardPostsController : Controller
    {
        private readonly SurfBoardManagerContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;

        //Opretter BoardPostController objekt som tager SurBoardManagerContext og RoleManager med type parameter IdentityRole som parameter.
        public BoardPostsController(SurfBoardManagerContext context, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _roleManager = roleManager;
        }

        // HTTPS GET metode
        //Metoden tager pageNumber, searchString, min og max parameter, som bliver brugt til at sortere i BoardPost listen fra databasen.
        public async Task<IActionResult> Index(int? pageNumber, string searchString, string min, string max)
        {
            //Sætter alle BoardPost fra databasen = boardPost variablen.
            var boardPosts = from b in _context.BoardPost
                             select b;

            //Sortere boardPost udfra om deres "Name" indeholder bruger inputtet fra "searchString" 
            if (!string.IsNullOrEmpty(searchString))
            {
                boardPosts = boardPosts.Where(b => b.Name.Contains(searchString));
            }

            //Sortere boardPost udfra om deres "Price", hvor "Price" er større end bruger inputtet fra "min"
            if (!string.IsNullOrEmpty(min))
            {
                boardPosts = boardPosts.Where(b => b.Price > decimal.Parse(min));
            }

            //Sortere boardPost udfra om deres "Price" hvor "Price" er mindre end bruger inputtet fra "max"
            if (!string.IsNullOrEmpty(max))
            {
                boardPosts = boardPosts.Where(b => b.Price < decimal.Parse(max));
            }

            //sætter default "pageSize" til 3.
            int pageSize = 3;
            //retunere View, som navigere brugeren til view'et tilhørende BoardPostControllerens Index.
            //View methoden laver også et check på om BoardPost'ne er blevet lejet ud. Hvis board'ne er blevet lejet, fjernes visningen af disse fra PaginatedListen
            return View(await PaginatedList<BoardPost>.CreateAsync(boardPosts.Where(b => b.RentalDateEnd == null || (DateTime.Compare((DateTime)b.RentalDateEnd, DateTime.Now)) < 0 ), pageNumber ?? 1, pageSize));

        }

        // GET: BoardPosts/Details/5
        //HTTPS Get metode som tager et id som parameter og bliver brugt til at vise detalierne for et bestemt boardpost.
        public async Task<IActionResult> Details(int? id)
        {
            //Checker om parameteren "id" har en værdig og om contexten har en værdig.
            if (id == null || _context.BoardPost == null)
            {
                return NotFound();
            }

            //Finder første boardPost med det inputtede id
            var boardPost = await _context.BoardPost
                .FirstOrDefaultAsync(m => m.Id == id);
            if (boardPost == null)
            {
                return NotFound();
            }

            //retunere View, som navigere brugeren til view'et tilhørende BoardPostControllerens details tilhørende boardposten fra View parameterne.
            return View(boardPost);
        }

        [Authorize(Policy = "RequiredAdminRole")]
        // GET: BoardPosts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BoardPosts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        
        [Authorize(Policy = "RequiredAdminRole")] //Fortæller Create metoden at brugeren skal have en admin rolle for at kunne oprette nye boardposts.
        [HttpPost]
        [ValidateAntiForgeryToken] 
        //Create metoden, opretter nye boardPost.
        public async Task<IActionResult> Create([Bind("Name,Width,Length,Thickness,Volume,BoardType,Equipment,Price, BoardImage")] BoardPost boardPost)
        {
            ModelState.Remove(nameof(SurfUpUser));

            if (ModelState.IsValid)
            {
                //Opretter et nyt objekt af boardpost og gemmer ændringerne i databasen. Navigere også til Viewet "Index" fra Boardpostcontrolleren
                _context.Add(boardPost);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(boardPost);
        }

        // GET: BoardPosts/Edit/5
        [Authorize(Policy = "RequiredAdminRole")] //Fortæller Edit metoden at brugeren skal have en admin rolle for at kunne ændre i boardpostens data.
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.BoardPost == null)
            {
                return NotFound();
            }

            var boardPost = await _context.BoardPost.FindAsync(id);
            if (boardPost == null)
            {
                return NotFound();
            }
            return View(boardPost);
        }

        // POST: BoardPosts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Policy = "RequiredAdminRole")] //Fortæller edit metoden at brugeren skal have en admin rolle for at kunne ændre i boardpost objektet.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //Edit metoden, lader en bruger med admin rettigheder ændre på boardpostens data.
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Width,Length,Thickness,Volume,BoardType,Equipment,Price, BoardImage")] BoardPost boardPost)
        {
            //checker om boardPost id'et matcher overens med input id'et.
            if (id != boardPost.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //updatere boardPost objektet og gemmer ændringerne i databasen.
                    _context.Update(boardPost);
                    await _context.SaveChangesAsync();
                }
                //Hvis databasen forventede at blive opdateret, men intet er blevet ændre fanger
                //"dbupdateConcurrencyexecption fejlen og navigere brugeren til Index siden for boardpostcontrolleren
                catch (DbUpdateConcurrencyException)
                {
                    if (!BoardPostExists(boardPost.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(boardPost);
        }

        // GET: BoardPosts/Delete/5
        [Authorize(Policy = "RequiredAdminRole")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.BoardPost == null)
            {
                return NotFound();
            }

            var boardPost = await _context.BoardPost
                .FirstOrDefaultAsync(m => m.Id == id);
            if (boardPost == null)
            {
                return NotFound();
            }

            return View(boardPost);
        }

        // POST: BoardPosts/Delete/5
        [Authorize(Policy ="RequiredAdminRole")] //Fortæller delete metoden at brugeren skal have en admin rolle for at kunne fjerne boardpost objektet.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        //DeleteConfirmed metoden, benytter sig af et id parameter til at fjerne en boardpost fra databasen.
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //Checker om der er nogle boardpost i første omgang. Hvis ikke kommer den med nedstående fejlbesked.
            if (_context.BoardPost == null)
            {
                return Problem("Entity set 'SurfBoardManagerContext.BoardPost'  is null.");
            }

            //Finder boardPost objektet ud fra id parameteren.
            var boardPost = await _context.BoardPost.FindAsync(id);
            if (boardPost != null)
            {
                //Fjerner det fundende boardPost.
                _context.BoardPost.Remove(boardPost);
            }
            
            //Opdatere databasen, og retunere til view'et tilhørede Index i boardPostControlleren.
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //Checker om et boardPost findes udfra "id" parametert i context'en og returener denne.
        private bool BoardPostExists(int id)
        {
            return (_context.BoardPost?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> Rent(int? id)
        {
            if (id == null || _context.BoardPost == null)
            {
                return NotFound();
            }

            var boardPost = await _context.BoardPost.FindAsync(id);
            if (boardPost == null)
            {
                return NotFound();
            }

            RentalViewModel rentalViewModel = new RentalViewModel()
            {
                BoardPost = boardPost
            };

            return View(rentalViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //
        public async Task<IActionResult> Rent(int id, [Bind("RentalPeriod,BoardPost")] RentalViewModel rentalViewModel, SurfUpUser surfUpUser)
        {
            if (id != rentalViewModel.BoardPost.Id)
            {
                return NotFound();
            }

            var boardPost = await _context.BoardPost
                .FirstOrDefaultAsync(m => m.Id == id);

            rentalViewModel.BoardPost = boardPost;

            ModelState.Remove("BoardPost");
            ModelState.Remove("BoardPost.Name");
            ModelState.Remove("BoardPost.Equipment");
            ModelState.Remove("BoardPost.BoardImage");
            ModelState.Remove("BoardPost.SurfUpUser");

            rentalViewModel.BoardPost.RentalDate = DateTime.Now;
            rentalViewModel.BoardPost.RentalDateEnd = DateTime.Now.AddDays(rentalViewModel.RentalPeriod);
            rentalViewModel.BoardPost.SurfUpUser = surfUpUser;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rentalViewModel.BoardPost);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BoardPostExists(rentalViewModel.BoardPost.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(rentalViewModel);
        }

    }
}
