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
        // Alloker variabler til rollerstyring og context (database).
        private readonly SurfBoardManagerContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;

        // Constructoren kaldes når vi får requests fra vores klient og en controller skabes.
        // Aps sørger for at injecte context og rolemanager som fungerer.
        public BoardPostsController(SurfBoardManagerContext context, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _roleManager = roleManager;
        }

        // GET
        // Index metoden som viser vores boards, pageNumber viser hvilken side vores paginated list er på, searchString bruges
        // til at filtrere i navnene og min og max bruges til at filtrere inden for et pris interval.
        public async Task<IActionResult> Index(int? pageNumber, string searchString, string min, string max)
        {
            // LINQ statement som henter alt i BoardPost table.
            var boardPosts = from b in _context.BoardPost
                             select b;

            // Guard som sikrer at stringen er brugbar (ikke null eller tom)
            if (!string.IsNullOrEmpty(searchString))
            {
                // Kører lambda funktion for at filtrere listen
                // b er input variablen (csharp gætter sig til variabl typen)
                // b.Name.Contains bruges til at filtrerre listen afhængig af searchStrings værdi.
                boardPosts = boardPosts.Where(b => b.Name.Contains(searchString));
            }

            // as above so below.
            if (!string.IsNullOrEmpty(min))
            {
                boardPosts = boardPosts.Where(b => b.Price > decimal.Parse(min));
            }

            if (!string.IsNullOrEmpty(max))
            {
                boardPosts = boardPosts.Where(b => b.Price < decimal.Parse(max));
            }

            // Page size er hvor mange surfboards vi kan vise på vores paginated side.
            int pageSize = 3;
            // Filtrer lejede boards, tjek om pageNumber er null (hvis den er null,
            // så start visning på side 1, ellers så brug værdien i pageNumber), brug pagesize.
            return View(await PaginatedList<BoardPost>.CreateAsync(boardPosts.Where(b => b.RentalDateEnd == null || (DateTime.Compare((DateTime)b.RentalDateEnd, DateTime.Now)) < 0), pageNumber ?? 1, pageSize));
        }

        // GET: BoardPosts/Details/5
        public async Task<IActionResult> Details(int? id)
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

        [Authorize(Policy = "RequiredAdminRole")]
        // GET: BoardPosts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BoardPosts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Policy = "RequiredAdminRole")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Width,Length,Thickness,Volume,BoardType,Equipment,Price, BoardImage")] BoardPost boardPost)
        {
            ModelState.Remove(nameof(SurfUpUser));

            if (ModelState.IsValid)
            {
                _context.Add(boardPost);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(boardPost);
        }

        // GET: BoardPosts/Edit/5
        [Authorize(Policy = "RequiredAdminRole")]
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
        [Authorize(Policy = "RequiredAdminRole")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Width,Length,Thickness,Volume,BoardType,Equipment,Price, BoardImage")] BoardPost boardPost)
        {
            if (id != boardPost.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(boardPost);
                    await _context.SaveChangesAsync();
                }
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
        [Authorize(Policy ="RequiredAdminRole")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.BoardPost == null)
            {
                return Problem("Entity set 'SurfBoardManagerContext.BoardPost'  is null.");
            }
            var boardPost = await _context.BoardPost.FindAsync(id);
            if (boardPost != null)
            {
                _context.BoardPost.Remove(boardPost);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

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
