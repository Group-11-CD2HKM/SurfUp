﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SurfBoardManager.Data;
using SurfBoardManager.Models;

namespace SurfBoardManager.Controllers
{
    public class BoardPostsController : Controller
    {
        private readonly SurfBoardManagerContext _context;

        public BoardPostsController(SurfBoardManagerContext context)
        {
            _context = context;
        }

        // GET: BoardPosts
        public async Task<IActionResult> Index()
        {
              return _context.BoardPost != null ? 
                          View(await _context.BoardPost.ToListAsync()) :
                          Problem("Entity set 'SurfBoardManagerContext.BoardPost'  is null.");
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

        // GET: BoardPosts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BoardPosts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Width,Length,Thickness,Volume,BoardType,Equipment,Price, BoardImage")] BoardPost boardPost)
        {
            if (ModelState.IsValid)
            {
                _context.Add(boardPost);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(boardPost);
        }

        // GET: BoardPosts/Edit/5
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
    }
}
