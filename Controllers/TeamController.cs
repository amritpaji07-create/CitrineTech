using CitrineTechData.DataContext;
using CitrineTechData.DataModel;
using CitrineTechData.ViewModel;
using CitrineTechService.Interface;
using Microsoft.AspNetCore.Mvc;
using X.PagedList.Extensions;

namespace CitrineTech.Controllers
{
    public class TeamController : Controller
    {
        private readonly ITeamService _teamService;
        private readonly IWebHostEnvironment _env;
        private readonly ApplicationDBContext _context;
        public TeamController(ITeamService teamService, IWebHostEnvironment env, ApplicationDBContext context)
        {
            _teamService = teamService;
            _env = env;
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
        public async Task<IActionResult> Teams(int? page)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            try
            {
                int pageSize = 10;
                int pageNumber = page ?? 1;

                var team = await _teamService.GetAllAsync();

                if (team == null || !team.Any())
                {
                    ViewBag.Message = "Data not found.";
                    return View(Enumerable.Empty<Team>().ToPagedList(pageNumber, pageSize));
                }

                return View(team.ToPagedList(pageNumber, pageSize));
            }
            catch (Exception ex)
            {
                // Log error (you can use ILogger or any logging framework)
                ViewBag.Error = "An error occurred while fetching team data.";
                return View("Error"); // Or redirect to a proper error page/view
            }
        }

        public async Task<IActionResult> TeamDetails(int id)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            if (id == 0) return NotFound();

            var team = await _teamService.GetByIdAsync(id);
            if (team == null || team.IsDeleted) return NotFound();

            return View(team);
        }

        public IActionResult TeamCreate()
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TeamCreate(TeamViewModel model)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            var userId = 1;
            //var userId = HttpContext.Session.GetInt32("UserId") ?? 1;
            if (ModelState.IsValid)
            {
                await _teamService.AddAsync(model, userId);
                return RedirectToAction("Teams");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> TeamEdit(int id)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            var team = await _teamService.GetByIdAsync(id);
            if (team == null) return NotFound();

            return View(new TeamViewModel
            {
                Id = team.Id,
                Name = team.Name,
                Designation = team.Designation,
                Description = team.Description,
                Index = team.Index,
                IsActive = team.IsActive,
                ImageUrl = team.ImageUrl,
                ExistingImagePath = team.ImageUrl
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TeamEdit(TeamViewModel model)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            var userId = 1;
            //var userId = HttpContext.Session.GetInt32("UserId") ?? 1;
            if (ModelState.IsValid)
            {
                await _teamService.UpdateAsync(model, userId);
                return RedirectToAction("Teams");
            }
            return View(model);
        }


        public async Task<IActionResult> Delete(int id)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            var about = await _teamService.GetByIdAsync(id);
            if (about == null) return NotFound();

            //var userId = HttpContext.Session.GetInt32("UserId") ?? 1;
            await _teamService.SoftDeleteAsync(id);
            return RedirectToAction("Teams");
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            var data = await _context.TblTeam.FindAsync(id);
            if (data == null) return Json(new { success = false });

            data.IsActive = !data.IsActive;
            await _context.SaveChangesAsync();

            return Json(new { success = true, isActive = data.IsActive });
        }

    }
}
