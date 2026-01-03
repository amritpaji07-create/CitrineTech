using CitrineTechData.DataContext;
using CitrineTechData.DataModel;
using CitrineTechData.ViewModel;
using CitrineTechService.Implementation;
using CitrineTechService.Interface;
using Microsoft.AspNetCore.Mvc;
using X.PagedList.Extensions;

namespace CitrineTech.Controllers
{
    public class PartnerController : Controller
    {
        private readonly IPartnersService _partnersService;
        private readonly ApplicationDBContext _context;
        public PartnerController(IPartnersService partnersService, ApplicationDBContext context)
        {
            _partnersService = partnersService;
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

        public async Task<IActionResult> Partners(int? page)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            try
            {
                int pageSize = 10;
                int pageNumber = page ?? 1;

                var partners = await _partnersService.GetAllAsync();

                if (partners == null || !partners.Any())
                {
                    ViewBag.Message = "No partners found.";
                    return View(Enumerable.Empty<Partners>().ToPagedList(pageNumber, pageSize));
                }

                return View(partners.ToPagedList(pageNumber, pageSize));
            }
            catch (Exception ex)
            {
                // Log error (you can use ILogger or any logging framework)
                ViewBag.Error = "An error occurred while fetching partner data.";
                return View("Error"); // Or redirect to a proper error page/view
            }
        }

        
        [HttpGet]
        public async Task<IActionResult> PartnerDetails(int id)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            var partner = await _partnersService.GetByIdAsync(id);
            if (partner == null) return NotFound();
            return View(partner);
        }

        public async Task<IActionResult> OurPartner()
        {
            var data = await _partnersService.GetAllAsync();
            return View(data);
        }

        public IActionResult PartnerCreate()
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PartnerCreate(PartnerViewModel partner)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            //var userId = HttpContext.Session.GetInt32("UserId");
            var userId = 1;
            if (ModelState.IsValid)
            {
                await _partnersService.AddAsync(partner, userId);
                return RedirectToAction("Partners");
            }
            return View(partner);
        }

        public async Task<IActionResult> PartnerEdit(int id)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            var userId = 1;
            var partner = await _partnersService.GetByIdAsync(id);
            if (partner == null) return NotFound();

            var viewModel = new PartnerViewModel
            {
                Id = partner.Id,
                Name = partner.Name,
                PartnerLogo = partner.PartnerLogo,
                IsActive = partner.IsActive,
                CreatedDate = DateTime.Now,
                CreatedBy = userId,
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> PartnerEdit(PartnerViewModel partner)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            var userId = 1;
            //var userId = HttpContext.Session.GetInt32("UserId");
            if (ModelState.IsValid)
            {
                await _partnersService.UpdateAsync(partner, userId);
                return RedirectToAction(nameof(Partners));
            }
            return View(partner);
        }


        public async Task<IActionResult> Delete(int id)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            var userId = 1;
            //var userId = HttpContext.Session.GetInt32("UserId");

            var partner = await _partnersService.GetByIdAsync(id);
            if (partner == null) return NotFound();

            await _partnersService.DeleteAsync(id, userId);
            return RedirectToAction(nameof(Partners));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            var data = await _context.TblPartner.FindAsync(id);
            if (data == null) return Json(new { success = false });

            data.IsActive = !data.IsActive;
            await _context.SaveChangesAsync();

            return Json(new { success = true, isActive = data.IsActive });
        }



    }
}
