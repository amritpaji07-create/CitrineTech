using CitrineTechData.DataContext;
using CitrineTechData.DataModel;
using CitrineTechData.ViewModel;
using CitrineTechService.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Extensions;

namespace CitrineTech.Controllers
{
    public class ProjectController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly IProjectService _ProjectService;        
        public ProjectController(IProjectService ProjectService, ApplicationDBContext context)
        {
            _ProjectService = ProjectService;
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

        // GET: Project
        public async Task<IActionResult> Projects(int? page)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            try
            {
                int pageSize = 10;
                int pageNumber = page ?? 1;

                var abouts = await _ProjectService.GetAllAsync();

                if (abouts == null || !abouts.Any())
                {
                    ViewBag.Message = "Data not found.";
                    return View(Enumerable.Empty<Projects>().ToPagedList(pageNumber, pageSize));
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

        // GET: Project/Details/5
        public async Task<IActionResult> ProjectDetails(int id)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            var Project = await _ProjectService.GetByIdAsync(id);
            if (Project == null) return NotFound();
            return View(Project);
        }

        // GET: Project/Create
        public IActionResult ProjectCreate()
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            return View();
        }

        // POST: Project/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProjectCreate(ProjectViewModel Project)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            int userId = 1;
            if (ModelState.IsValid)
            {
                await _ProjectService.AddProjectAsync(Project, userId);
                return RedirectToAction(nameof(Projects));
            }
            return View(Project);
        }

        // GET: Project/Edit/5
        public async Task<IActionResult> ProjectEdit(int id)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            var data = await _ProjectService.GetByIdAsync(id);
            return View(new ProjectViewModel
            {
                Id = data.Id,
                Name = data.Name,
                SubTitle = data.SubTitle,               
                Index = data.Index,
                Description = data.Description,
                ImageUrl = data.ImageUrl,
                ExistingImages  = data.ProjectImages.Select(img => img.ImagePath).ToList()
            });            
        }

        // POST: Project/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProjectEdit(ProjectViewModel Project)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            int userId = 1;
            if (ModelState.IsValid)
            {
                await _ProjectService.UpdateProjectAsync(Project, userId);
                return RedirectToAction(nameof(Projects));
            }
            return View(Project);
        }

        // GET: Project/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            var Project = await _ProjectService.GetByIdAsync(id);
            if (Project == null) return NotFound();
            await _ProjectService.DeleteAsync(id);
            return RedirectToAction(nameof(Projects));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            var data = await _context.TblProject.FindAsync(id);
            if (data == null) return Json(new { success = false });

            data.IsActive = !data.IsActive;
            await _context.SaveChangesAsync();

            return Json(new { success = true, isActive = data.IsActive });
        }

    }

}
