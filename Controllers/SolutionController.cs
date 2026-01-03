using CitrineTechData.DataContext;
using CitrineTechData.DataModel;
using CitrineTechData.ViewModel;
using CitrineTechService.Implementation;
using CitrineTechService.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Extensions;

namespace CitrineTech.Controllers
{
    public class SolutionController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly ISolutionService _SolutionSolution;        
        public SolutionController(ISolutionService SolutionSolution, ApplicationDBContext context)
        {
            _SolutionSolution = SolutionSolution;
            _context = context;
        }

        private IActionResult CheckSession()
        {
            var name = HttpContext.Session.GetString("Name");
            if (string.IsNullOrEmpty(name))
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.MySession = name;
            return null; // means session is valid
        }

        // GET: Solution
        public async Task<IActionResult> Solutions(int? page)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            try
            {
                int pageSize = 10;
                int pageNumber = page ?? 1;

                var abouts = await _SolutionSolution.GetAllAsync();

                if (abouts == null || !abouts.Any())
                {
                    ViewBag.Message = "Data not found.";
                    return View(Enumerable.Empty<Solutions>().ToPagedList(pageNumber, pageSize));
                }

                return View(abouts.ToPagedList(pageNumber, pageSize));
            }
            catch (Exception ex)
            {
                // Log error (you can use ILogger or any logging framework)
                ViewBag.Error = "An error occurred while fetching about data.";
                return View("Error"); // Or redirect to a proper error page/view
            }
        }

        // GET: Solution/Details/5
        public async Task<IActionResult> SolutionDetails(int id)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            var Solution = await _SolutionSolution.GetByIdAsync(id);
            if (Solution == null) return NotFound();
            return View(Solution);
        }

        // GET: Solution/Create
        public IActionResult SolutionCreate()
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            return View();
        }

        // POST: Solution/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SolutionCreate(SolutionViewModel Solution)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            int userId = 1;
            if (ModelState.IsValid)
            {
                await _SolutionSolution.AddAsync(Solution, userId);
                return RedirectToAction(nameof(Solutions));
            }
            return View(Solution);
        }

        // GET: Solution/Edit/5
        public async Task<IActionResult> SolutionEdit(int id)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            var data = await _SolutionSolution.GetByIdAsync(id);
            return View(new SolutionViewModel
            {
                Id = data.Id,
                Name = data.Name,
                SubTitle = data.SubTitle,
                Index = data.Index,
                Description = data.Description,
                ImageUrl = data.ImageUrl,
                ExistingImagePath = data.ImageUrl
            });            
        }

        // POST: Solution/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SolutionEdit(SolutionViewModel Solution)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            int userId = 1;
            if (ModelState.IsValid)
            {
                await _SolutionSolution.UpdateAsync(Solution, userId);
                return RedirectToAction(nameof(Solutions));
            }
            return View(Solution);
        }

        // GET: Solution/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            var Solution = await _SolutionSolution.GetByIdAsync(id);
            if (Solution == null) return NotFound();
            await _SolutionSolution.DeleteAsync(id);
            return RedirectToAction(nameof(Solutions));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            var data = await _context.TblSolution.FindAsync(id);
            if (data == null) return Json(new { success = false });

            data.IsActive = !data.IsActive;
            await _context.SaveChangesAsync();

            return Json(new { success = true, isActive = data.IsActive });
        }


        // GET: Solution/Details/5
        public async Task<IActionResult> Details(int id)
        {            

            var solution = await _SolutionSolution.GetFullSolutionDetailsAsync(id);
            if (solution == null) return NotFound();

            var model = new SolutionViewModel
            {
                Id = solution.Id,
                Name = solution.Name,
                SubTitle = solution.SubTitle,
                Description = solution.Description,
                ImageUrl = solution.ImageUrl,
                SubSolutions = solution.SubSolutions
                    .Where(sub => sub.IsActive && !sub.IsDeleted)
                    .Select(sub => new SubSolutionViewModel
                    {
                        Id = sub.Id,
                        Name = sub.Name,
                        Description = sub.Description,
                        ImageUrl = sub.ImageUrl,
                        ImagePaths = sub.SubSolutionImages?.Select(img => img.ImagePath).ToList() ?? new List<string>()
                    }).ToList()
            };

            return View(model);
        }
        
    }

}
