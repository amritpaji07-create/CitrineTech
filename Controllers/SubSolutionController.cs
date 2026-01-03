using CitrineTechData.DataContext;
using CitrineTechData.DataModel;
using CitrineTechData.ViewModel;
using CitrineTechService.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Extensions;

namespace CitrineTech.Controllers
{
    public class SubSolutionController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly ISubSolutionService _SubSolutionService;
        public SubSolutionController(ISubSolutionService SubSolutionService, ApplicationDBContext context)
        {
            _SubSolutionService = SubSolutionService;
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

        // GET: SubSolution
        public async Task<IActionResult> SubSolutions(int? page)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            try
            {
                int pageSize = 10;
                int pageNumber = page ?? 1;

                var data = await _SubSolutionService.GetAllAsync();

                var solution = _context.TblSolution
    .Where(s => !s.IsDeleted)
    .Select(s => new SelectListItem
    {
        Value = s.Name,
        Text = s.Name
    })
    .ToList();

                ViewBag.SolutionList = solution;

                if (data == null || !data.Any())
                {
                    ViewBag.Message = "Data not found.";
                    return View(Enumerable.Empty<SubSolution>().ToPagedList(pageNumber, pageSize));
                }

                return View(data.ToPagedList(pageNumber, pageSize));
            }
            catch (Exception ex)
            {
                // Log error (you can use ILogger or any logging framework)
                ViewBag.Error = "An error occurred while fetching subsolution data.";
                return View("Error"); // Or redirect to a proper error page/view
            }
        }

        // GET: SubSolution/Details/5
        public async Task<IActionResult> SubSolutionDetails(int id)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            var SubSolution = await _SubSolutionService.GetByIdAsync(id);
            if (SubSolution == null) return NotFound();

            var model = new SubSolutionViewModel
            {
                Id = SubSolution.Id,
                Name = SubSolution.Name,
                SolutionName = SubSolution.Solutions?.Name,
                Description = SubSolution.Description,
                ImagePaths = SubSolution.SubSolutionImages?.Select(i => i.ImagePath).ToList() ?? new List<string>()
            };

            return View(model);
        }


        public async Task<IActionResult> Details(int id)
        {           

            var SubSolution = await _SubSolutionService.GetByIdAsync(id);
            if (SubSolution == null) return NotFound();

            var model = new SubSolutionViewModel
            {
                Id = SubSolution.Id,
                Name = SubSolution.Name,
                SolutionName = SubSolution.Solutions?.Name,
                Description = SubSolution.Description,
                ImagePaths = SubSolution.SubSolutionImages?.Select(i => i.ImagePath).ToList() ?? new List<string>(),
            };

            return View(model);
        }




        // GET: SubSolution/Create
        public IActionResult SubSolutionCreate()
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            var solution = _context.TblSolution.Where(s => !s.IsDeleted)
        .Select(s => new SelectListItem
        {
            Value = s.Id.ToString(),  // Use s.Id if ServiceId is a foreign key
            Text = s.Name
        })
        .ToList();

            ViewBag.SolutionList = solution;

            return View();
        }

        // POST: SubSolution/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubSolutionCreate(SubSolutionViewModel SubSolution)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            int userId = 1;
            if (ModelState.IsValid)
            {
                await _SubSolutionService.AddAsync(SubSolution, userId);
                return RedirectToAction(nameof(SubSolutions));
            }
            return View(SubSolution);
        }

        // GET: SubSolution/Edit/5
        public async Task<IActionResult> SubSolutionEdit(int id)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            var data = await _SubSolutionService.GetByIdAsync(id);
            if (data == null) return NotFound();

            var solution = _context.TblSolution.Where(s => !s.IsDeleted)
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                })
                .ToList();

            ViewBag.SolutionList = solution;

            return View(new SubSolutionViewModel
            {
                Id = data.Id,
                Name = data.Name,
                Description = data.Description,
                ImageUrl = data.ImageUrl,
                SolutionId = data.SolutionId,
                ExistingImages = data.SubSolutionImages.Select(img => img.ImagePath).ToList()
            });
        }

        // POST: SubSolution/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubSolutionEdit(SubSolutionViewModel SubSolution)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid


            int userId = 1;
            if (ModelState.IsValid)
            {
                await _SubSolutionService.UpdateAsync(SubSolution, userId);
                return RedirectToAction(nameof(SubSolutions));
            }
            // 🔴 Without this, dropdown will break and ServiceId posts as 0
            ViewBag.SolutionList = _context.TblSolution.Where(s => !s.IsDeleted)
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                })
                .ToList();

            return View(SubSolution);
        }

        // GET: SubSolution/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid


            var SubSolution = await _SubSolutionService.GetByIdAsync(id);
            if (SubSolution == null) return NotFound();
            await _SubSolutionService.DeleteAsync(id);
            return RedirectToAction(nameof(SubSolutions));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid


            var data = await _context.TblSubSolution.FindAsync(id);
            if (data == null) return Json(new { success = false });

            data.IsActive = !data.IsActive;
            await _context.SaveChangesAsync();

            return Json(new { success = true, isActive = data.IsActive });
        }

    }

}
