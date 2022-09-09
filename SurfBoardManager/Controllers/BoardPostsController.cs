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
        private readonly UserManager<SurfUpUser> _userManager;
        private readonly SurfBoardManagerContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;

        public BoardPostsController(SurfBoardManagerContext context, RoleManager<IdentityRole> roleManager, UserManager<SurfUpUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        // GET
        public async Task<IActionResult> Index(int? pageNumber, string searchString, string min, string max)
        {
            var boardPosts = from b in _context.BoardPost
                             select b;

            if (!string.IsNullOrEmpty(searchString))
            {
                boardPosts = boardPosts.Where(b => b.Name.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(min))
            {
                boardPosts = boardPosts.Where(b => b.Price > decimal.Parse(min));
            }

            if (!string.IsNullOrEmpty(max))
            {
                boardPosts = boardPosts.Where(b => b.Price < decimal.Parse(max));
            }

            int pageSize = 3;
            return View(await PaginatedList<BoardPost>.CreateAsync(boardPosts.Where(b => b.IsRented == false), pageNumber ?? 1, pageSize));

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

        [Authorize]
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

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Rent(int id, [Bind("RentalPeriod,BoardPost")] RentalViewModel rentalViewModel)
        {
            var surfUpUser = await _userManager.GetUserAsync(User);

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
