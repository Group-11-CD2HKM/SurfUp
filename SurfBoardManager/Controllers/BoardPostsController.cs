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
        // Alloker variabler til rollerstyring og context (database).
        private readonly SurfBoardManagerContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BoardPostsController(
            SurfBoardManagerContext context,
            RoleManager<IdentityRole> roleManager,
            UserManager<SurfUpUser> userManager,
            IHttpContextAccessor httpContextAccessor)
        //Opretter BoardPostController objekt som tager SurBoardManagerContext og RoleManager med type parameter IdentityRole som parameter.
        // Parametrene bliver injected af Asp.net, så længe de er registreret som en service i program.cs

        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
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
                // Kører lambda funktion for at filtrere listen
                // b er input variablen (csharp gætter sig til variabl typen)
                // b.Name.Contains bruges til at filtrerre listen afhængig af searchStrings værdi.
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

            // Sisdte filtrering for at fjerne hvad der er udlejet.
            boardPosts = boardPosts.Where(b => b.RentalDateEnd == null || (DateTime.Compare((DateTime)b.RentalDateEnd, DateTime.Now)) < 0);

            //sætter default "pageSize" til 3.
            int pageSize = 3;

            // Laver en PaginatedList ud fra vores filtrerede boardPosts, et sidetal og sidestørrelsen (pageSize).
            // Hvis sidetallet er null, så bruges 1 som sidetal.
            PaginatedList<BoardPost> paginatedList = await PaginatedList<BoardPost>.CreateAsync(boardPosts, pageNumber ?? 1, pageSize);
            //retunere View, som navigere brugeren til view'et tilhørende BoardPostControllerens Index.
            return View(paginatedList);

        }

        // GET: BoardPosts/Details/5
        //HTTP Get metode som tager et id som parameter og bliver brugt til at vise detaljerne for et bestemt boardpost.
        public async Task<IActionResult> Details(int? id)
        {
            //Checker om parametren "id" har en værdi og om contexten har en værdi.
            if (id == null || _context.BoardPost == null)
            {
                return NotFound();
            }

            //Finder første boardPost med det inputtede id eller null.
            var boardPost = await _context.BoardPost
                .FirstOrDefaultAsync(m => m.Id == id);
            if (boardPost == null)
            {
                return NotFound();
            }

            //retunere View, som navigere brugeren til view'et tilhørende BoardPostControllerens Details metod.
            // Injecter boardPost i viewet.
            return View(boardPost);
        }

        [Authorize(Roles = "Admin")]
        // GET: BoardPosts/Create
        // Get versionen af vores create metode, returnerer et view uden nogen injectet model.
        public IActionResult Create()
        {
            return View();
        }

        // POST: BoardPosts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")] //Fortæller Create metoden at brugeren skal have en admin rolle for at kunne oprette nye boardposts.
        [HttpPost]
        [ValidateAntiForgeryToken] 
        //Create metoden, opretter nye boardPost.
        public async Task<IActionResult> Create([Bind("Name,Width,Length,Thickness,Volume,BoardType,Equipment,Price, BoardImage")] BoardPost boardPost)
        {
            // Skal ikke længere bruges, hvis Modelstate.IsValid brokker sig,
            // så tjek om propertyen egentligt skal være nullable i modellen?
            // ModelState.Remove(nameof(SurfUpUser));

            ModelState.Remove("RowVersion");

            if (ModelState.IsValid)
            {
                //Tilføjer boardPost til vores context.
                // Contexten gemmes derefter.
                // Navigere også til Viewet "Index" fra Boardpostcontrolleren
                _context.Add(boardPost);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            // Hvis modellen ikke validerer korrekt, så hopper vi tilbage til create viewet med vores boardpost, og lader jquery vise os hvad der er galt.
            return View(boardPost);
        }

        // GET: BoardPosts/Edit/5
        [Authorize(Roles = "Admin")] //Fortæller Edit metoden at brugeren skal have en admin rolle for at kunne ændre i boardpostens data.
        public async Task<IActionResult> Edit(int? id)
        {
            // Finder boardPost med id parametren, og sender det videre til Edit viewt, så vi kan rette i det.
            if (id == null || _context.BoardPost == null)
            {
                return NotFound();
            }

            var boardPost = await _context.BoardPost.AsNoTracking().FirstOrDefaultAsync(g => g.Id == id);

            if (boardPost == null)
            {
                return NotFound();
            }
            return View(boardPost);
        }

        // POST: BoardPosts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")] //Fortæller edit metoden at brugeren skal have en admin rolle for at kunne ændre i boardpost objektet.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //Edit metoden, lader en bruger med admin rettigheder ændre på boardpostens data.
        public async Task<IActionResult> Edit(int id,[Bind("Id,Name,Width,Length,Thickness,Volume,BoardType,Equipment,Price,BoardImage,RowVersion")] BoardPost boardPost)
        {
            //checker om boardPost id'et matcher overens med input id'et.
            if (id != boardPost.Id)
            {
                return NotFound();
            }
            
            //Finder det Boardpost objekt fra databasen der matcher ID'et sendt fra viewet
            var boardPostToBeUpdated = await _context.BoardPost.FirstOrDefaultAsync(x => x.Id == id);

            //Hvis objektet ikke kan findes, er det slettet. Den følgende 'if' kodeblok håndterer dette scenarie
            if(boardPostToBeUpdated == null)
            {
                BoardPost deletedBoardPost = new BoardPost();
                //Forsøger at udfylde deletedBoardPost objektet, med det objekt controlleren holder på fra viewet.
                //Det vil sige, det boardpost objektet vi modtog fra viewet i denne metode.
                await TryUpdateModelAsync(deletedBoardPost);
                //AddModelError, tilføjer fejlbeskeden brugeren skal se.
                ModelState.AddModelError("", "Board ændringer kan ikke gemmes. En anden bruger har slettet boarded");
                //Sender objektet tilbage til viewet
                return View(deletedBoardPost);
            }

            //Sætter original værdien for RowVersion,
            //dvs. den oprindelige vi fik fra da vi kaldte Edit GET metoden.
            _context.Entry(boardPostToBeUpdated).Property("RowVersion").OriginalValue = boardPost.RowVersion;         

            if (await TryUpdateModelAsync<BoardPost>(boardPostToBeUpdated, "", 
                b => b.Name,
                b => b.Length,
                b => b.Thickness,
                b => b.Volume,
                b => b.BoardType,
                b => b.Equipment,
                b => b.Price,
                b => b.Width,
                b => b.BoardImage,
                b => b.IsRented
                ))
            {
                try
                {
                    //SaveChangesAsync skaber en ConcurrenceException såfremt UPDATE SQL command returnerer 0 ændrede rækker.
                    //Det gør den hvis RowVersion kollonen på objektet i databasen
                    //er anderledes fra det boardpost objekt vi arbejder med her.
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    //Finder den entity der var involveret i exception
                    var exceptionEntry = ex.Entries.Single();
                    //Trækker det enkelte objekt ud og hardcaster til et BoardPost objekt
                    var clientValues = (BoardPost)exceptionEntry.Entity;
                    //Forespørger databasen for at finde frem til de nye værdier der ligger i databasen
                    var databaseEntry = exceptionEntry.GetDatabaseValues();

                    //Hvis boardet er slettet i mellemtiden:
                    if(databaseEntry == null)
                    {
                        ModelState.AddModelError("", "Board ændringer kan ikke gemmes. En anden bruger har slettet boarded");
                    }
                    else
                    {
                        //Caster objektet til et Boardpost objekt
                        var databaseValue = (BoardPost)databaseEntry.ToObject();

                        #region Fejlmeddelelser for hver textbox i view
                        if (clientValues.Name != databaseValue.Name)
                        {
                            ModelState.AddModelError("Name", $"Nuværende værdi: {databaseValue.Name}");
                        }
                        if (clientValues.Length != databaseValue.Length)
                        {
                            ModelState.AddModelError("Length", $"Nuværende værdi: {databaseValue.Length}");
                        }
                        if (clientValues.Width != databaseValue.Width)
                        {
                            ModelState.AddModelError("Name", $"Nuværende værdi: {databaseValue.Width}");
                        }
                        if (clientValues.Volume != databaseValue.Volume)
                        {
                            ModelState.AddModelError("Name", $"Nuværende værdi: {databaseValue.Volume}");
                        }
                        if (clientValues.Thickness != databaseValue.Thickness)
                        {
                            ModelState.AddModelError("Name", $"Nuværende værdi: {databaseValue.Thickness}");
                        }
                        if (clientValues.BoardType != databaseValue.BoardType)
                        {
                            ModelState.AddModelError("Name", $"Nuværende værdi: {databaseValue.BoardType}");
                        }
                        if (clientValues.BoardImage != databaseValue.BoardImage)
                        {
                            ModelState.AddModelError("Name", $"Nuværende værdi: {databaseValue.BoardImage}");
                        }
                        if (clientValues.Price != databaseValue.Price)
                        {
                            ModelState.AddModelError("Name", $"Nuværende værdi: {databaseValue.Price}");
                        }
                        if (clientValues.Equipment != databaseValue.Equipment)
                        {
                            ModelState.AddModelError("Name", $"Nuværende værdi: {databaseValue.Equipment}");
                        }
                        #endregion

                        ModelState.AddModelError("", "Kunne ikke gemme ændringerne." +
                            " En anden bruger har i mellemtiden lavet ændringer i dette board." +
                            " Ædringerne er vist i textboksene. Click på Save igen, for at gemme dine ændringer");
                        //Sætter RowVersion propertien for objektet til at være den nyere fra databasen
                        boardPostToBeUpdated.RowVersion = (byte[])databaseValue.RowVersion;
                        ModelState.Remove("RowVersion");
                    }
                }
            }
            return View(boardPostToBeUpdated);
        }

        // GET: BoardPosts/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            // Sæt model op og send den videre til delet viewet.
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
        [Authorize(Roles = "Admin")]
        //Fortæller delete metoden at brugeren skal have en admin rolle for at kunne fjerne boardpost objektet.
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

        //Checker om et boardPost findes udfra "id" parametert, i context'en og returener denne.
        private bool BoardPostExists(int id)
        {
            return (_context.BoardPost?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [Authorize]
        public async Task<IActionResult> Rent(int? id)
        {
            // Sætter rentalViewModel op og injecter den i Rent viewet.
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
            var _user = _httpContextAccessor.HttpContext.User;
            var surfUpUser = await _userManager.FindByEmailAsync(_user.Identity.Name);
            // Error checking. Maybe som user got here by accident or w.e.
            if (rentalViewModel.BoardPost is null || id != rentalViewModel.BoardPost.Id)
            {
                return NotFound();
            }

            var boardPost = await _context.BoardPost
                .FirstOrDefaultAsync(m => m.Id == id);

            rentalViewModel.BoardPost = boardPost;

            // Is no longer needed.
            // These properties cannot be marked nullable,
            // but they are all value types, so they can be hidden in the corresponding form and sent to us (see /Views/BoardPosts/Rent.cshtml).
            //ModelState.Remove("BoardPost");
            //ModelState.Remove("BoardPost.Name");
            //ModelState.Remove("BoardPost.Equipment");
            //ModelState.Remove("BoardPost.BoardImage");
            
            // Set rental related properties.
            rentalViewModel.BoardPost.RentalDate = DateTime.Now;
            rentalViewModel.BoardPost.RentalDateEnd = DateTime.Now.AddDays(rentalViewModel.RentalPeriod);
            rentalViewModel.BoardPost.SurfUpUser = surfUpUser;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rentalViewModel.BoardPost);
                    _context.Attach(surfUpUser); // Required when using sqlite?
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
