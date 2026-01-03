using CitrineTechData.DataContext;
using CitrineTechData.DataModel;
using CitrineTechData.ViewModel;
using CitrineTechService.Interface;
using Microsoft.AspNetCore.Mvc;
using X.PagedList.Extensions;

namespace CitrineTech.Controllers
{
    public class PatronsController : Controller
    {
        private readonly IPatronsService _PatronssService;
        private readonly ApplicationDBContext _context;
        public PatronsController(IPatronsService PatronssService, ApplicationDBContext context)
        {
            _PatronssService = PatronssService;
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

        public async Task<IActionResult> Patrons(int? page)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            try
            {
                int pageSize = 10;
                int pageNumber = page ?? 1;

                var Patronss = await _PatronssService.GetAllAsync();

                if (Patronss == null || !Patronss.Any())
                {
                    ViewBag.Message = "No Patronss found.";
                    return View(Enumerable.Empty<Patrons>().ToPagedList(pageNumber, pageSize));
                }

                return View(Patronss.ToPagedList(pageNumber, pageSize));
            }
            catch (Exception ex)
            {
                // Log error (you can use ILogger or any logging framework)
                ViewBag.Error = "An error occurred while fetching Patrons data.";
                return View("Error"); // Or redirect to a proper error page/view
            }
        }

        
        [HttpGet]
        public async Task<IActionResult> PatronsDetails(int id)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            var Patrons = await _PatronssService.GetByIdAsync(id);
            if (Patrons == null) return NotFound();
            return View(Patrons);
        }

        public async Task<IActionResult> OurPatrons()
        {           
            var patrons = await _PatronssService.GetAllAsync();
            return View(patrons);
        }





        public IActionResult PatronsCreate()
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PatronsCreate(PatronsViewModel Patrons)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            //var userId = HttpContext.Session.GetInt32("UserId");
            var userId = 1;
            if (ModelState.IsValid)
            {
                await _PatronssService.AddAsync(Patrons, userId);
                return RedirectToAction("Patrons");
            }
            return View(Patrons);
        }

        public async Task<IActionResult> PatronsEdit(int id)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            var userId = 1;
            var Patrons = await _PatronssService.GetByIdAsync(id);
            if (Patrons == null) return NotFound();

            var viewModel = new PatronsViewModel
            {
                Id = Patrons.Id,
                Name = Patrons.Name,
                PatronLogo = Patrons.PatronLogo,
                IsActive = Patrons.IsActive,
                CreatedDate = DateTime.Now,
                CreatedBy = userId,
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> PatronsEdit(PatronsViewModel Patrons)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            var userId = 1;
            //var userId = HttpContext.Session.GetInt32("UserId");
            if (ModelState.IsValid)
            {
                await _PatronssService.UpdateAsync(Patrons, userId);
                return RedirectToAction(nameof(Patrons));
            }
            return View(Patrons);
        }


        public async Task<IActionResult> Delete(int id)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            var userId = 1;
            //var userId = HttpContext.Session.GetInt32("UserId");

            var Patrons = await _PatronssService.GetByIdAsync(id);
            if (Patrons == null) return NotFound();

            await _PatronssService.DeleteAsync(id, userId);
            return RedirectToAction(nameof(Patrons));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            var data = await _context.Patrons.FindAsync(id);
            if (data == null) return Json(new { success = false });

            data.IsActive = !data.IsActive;
            await _context.SaveChangesAsync();

            return Json(new { success = true, isActive = data.IsActive });
        }



    }
}
