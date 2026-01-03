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
    public class SubServiceController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly ISubServiceService _SubServiceService;        
        public SubServiceController(ISubServiceService SubServiceService, ApplicationDBContext context)
        {
            _SubServiceService = SubServiceService;
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


        // GET: SubService
        public async Task<IActionResult> SubServices(int? page)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            try
            {
                int pageSize = 10;
                int pageNumber = page ?? 1;

                var data = await _SubServiceService.GetAllAsync();
                var services = _context.TblService.Where(s => !s.IsDeleted).Select(s => new SelectListItem { Value = s.Name, Text = s.Name }).ToList();

                ViewBag.ServiceList = services;

                if (data == null || !data.Any())
                {
                    ViewBag.Message = "Data not found.";
                    return View(Enumerable.Empty<SubService>().ToPagedList(pageNumber, pageSize));
                }

                return View(data.ToPagedList(pageNumber, pageSize));
            }
            catch (Exception ex)
            {
                // Log error (you can use ILogger or any logging framework)
                ViewBag.Error = "An error occurred while fetching about data.";
                return View("Error"); // Or redirect to a proper error page/view
            }
        }

        // GET: SubService/Details/5
        public async Task<IActionResult> SubServiceDetails(int id)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            var item = await _SubServiceService.GetByIdAsync(id);
            if (item == null) return NotFound();

            var model = new SubServiceViewModel
            {
                Id = item.Id,
                Name = item.Name,
                ServiceName = item.Service.Name,
                Description = item.Description,
                ImagePaths = item.SubServiceImages.Select(i => i.ImagePath).ToList()
            };

            return View(model);
        }

        // GET: SubService/Create
        public IActionResult SubServiceCreate()
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            var services = _context.TblService.Where(s => !s.IsDeleted)
        .Select(s => new SelectListItem
        {
            Value = s.Id.ToString(),  // Use s.Id if ServiceId is a foreign key
            Text = s.Name
        })
        .ToList();

            ViewBag.ServiceList = services;


            return View();
        }

        // POST: SubService/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubServiceCreate(SubServiceViewModel SubService)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            int userId = 1;
            if (ModelState.IsValid)
            {
                await _SubServiceService.AddAsync(SubService, userId);
                return RedirectToAction(nameof(SubServices));
            }
            return View(SubService);
        }

        // GET: SubService/Edit/5
        public async Task<IActionResult> SubServiceEdit(int id)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            var data = await _SubServiceService.GetByIdAsync(id);

            if (data == null) return NotFound();

            var services = _context.TblService.Where(s => !s.IsDeleted)
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                })
                .ToList();

            ViewBag.ServiceList = services;

            return View(new SubServiceViewModel
            {
                Id = data.Id,
                Name = data.Name,
                Description = data.Description,
                ImageUrl = data.ImageUrl,
                ServiceId = data.ServiceId,  // ✅ so dropdown preselects
                ExistingImages = data.SubServiceImages.Select(img => img.ImagePath).ToList()
            });
        }


        // POST: SubService/Edit/5
        [HttpPost]        
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubServiceEdit(SubServiceViewModel SubService)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            int userId = 1;
            if (ModelState.IsValid)
            {
                await _SubServiceService.UpdateAsync(SubService, userId);
                return RedirectToAction(nameof(SubServices));
            }

            // 🔴 Without this, dropdown will break and ServiceId posts as 0
            ViewBag.ServiceList = _context.TblService.Where(s => !s.IsDeleted)
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                })
                .ToList();

            return View(SubService);
        }


        // GET: SubService/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            var SubService = await _SubServiceService.GetByIdAsync(id);
            if (SubService == null) return NotFound();
            await _SubServiceService.DeleteAsync(id);
            return RedirectToAction(nameof(SubServices));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            var data = await _context.TblSubService.FindAsync(id);
            if (data == null) return Json(new { success = false });

            data.IsActive = !data.IsActive;
            await _context.SaveChangesAsync();

            return Json(new { success = true, isActive = data.IsActive });
        }

    }

}
