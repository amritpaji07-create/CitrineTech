using CitrineTechData.DataContext;
using CitrineTechData.DataModel;
using CitrineTechService.Interface;
using Microsoft.AspNetCore.Mvc;
using CitrineTechData.ViewModel;
using X.PagedList.Extensions;
using CitrineTechService.Implementation;

namespace CitrineTech.Controllers
{
    public class CareerController : Controller
    {
        private readonly ICareerService _careerService;
        private readonly ApplicationDBContext _context;

        public CareerController(ICareerService careerService, ApplicationDBContext context)
        {
            _careerService = careerService;
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

        // GET: Career
        public async Task<IActionResult> Careers(int? page)
        {
            var result = CheckSession();
            if (result != null) return result; 

            try
            {
                int pageSize = 10;
                int pageNumber = page ?? 1;

                var allUsers = await _careerService.GetAllAsync();

                // Filter out deleted users (assuming IsDeleted exists)
                var filteredUsers = allUsers.Where(u => !u.IsDeleted);

                //// Apply startDate filter
                //if (startDate.HasValue)
                //{
                //    filteredUsers = filteredUsers.Where(u => u.CreatedDate.Date >= startDate.Value.Date);
                //}

                //// Apply endDate filter
                //if (endDate.HasValue)
                //{
                //    filteredUsers = filteredUsers.Where(u => u.CreatedDate.Date <= endDate.Value.Date);
                //}

                // If no data found after filtering
                if (!filteredUsers.Any())
                {
                    ViewBag.Message = "No data found for selected filters.";
                    return View(Enumerable.Empty<Career>().ToPagedList(pageNumber, pageSize));
                }

                //// Pass date values back to the view to prefill inputs
                //ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
                //ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");

                return View(filteredUsers.ToPagedList(pageNumber, pageSize));
            }
            catch (Exception ex)
            {
                ViewBag.Error = "An error occurred while fetching user data.";
                return View("Error");
            }
        }

        // GET: Career/Create
        public IActionResult CareerCreate()
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            return View();
        }

        // POST: Career/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CareerCreate(CareerViewModel model)
        {

            if (ModelState.IsValid)
            {
                int userId = 1; // Replace with logged-in user id
                await _careerService.AddAsync(model, userId);
                return RedirectToAction(nameof(Careers));
            }
            return View(model);
        }

        // GET: Career/Edit/5
        public async Task<IActionResult> CareerEdit(int id)
        {           

            var career = await _careerService.GetByIdAsync(id);
            if (career == null) return NotFound();

            var model = new CareerViewModel
            {
                Id = career.Id,
                Name = career.Name,
                Email = career.Email,
                Contact = career.Contact,
                Position = career.Position,
                ExistingCvUrl = career.CvUrl,
                Location = career.Location,
                IsActive = career.IsActive
            };

            return View(model);
        }

        // POST: Career/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CareerEdit(CareerViewModel model)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            if (ModelState.IsValid)
            {
                int userId = 1; // Replace with logged-in user id
                await _careerService.UpdateAsync(model, userId);
                return RedirectToAction(nameof(Careers));
            }
            return View(model);
        }

        // GET: Career/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            var career = await _careerService.GetByIdAsync(id);
            if (career == null) return NotFound();

            int userId = 1; // Replace with logged-in user id
            await _careerService.DeleteAsync(id, userId);
            return RedirectToAction(nameof(Careers));
        }

        // GET: Blog/Details/5
        public async Task<IActionResult> CareerDetails(int id)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            var blog = await _careerService.GetByIdAsync(id);
            if (blog == null) return NotFound();
            return View(blog);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            var data = await _context.TblCareer.FindAsync(id);
            if (data == null) return Json(new { success = false });

            data.IsActive = !data.IsActive;
            await _context.SaveChangesAsync();

            return Json(new { success = true, isActive = data.IsActive });
        }



    }


}
