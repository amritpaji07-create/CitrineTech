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
    public class ServiceController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly IServiceService _ServiceService;        
        public ServiceController(IServiceService ServiceService, ApplicationDBContext context)
        {
            _ServiceService = ServiceService;
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


        // GET: Service
        public async Task<IActionResult> Services(int? page)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            try
            {
                int pageSize = 10;
                int pageNumber = page ?? 1;

                var abouts = await _ServiceService.GetAllAsync();

                if (abouts == null || !abouts.Any())
                {
                    ViewBag.Message = "Data not found.";
                    return View(Enumerable.Empty<Service>().ToPagedList(pageNumber, pageSize));
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

        // GET: Service/Details/5
        public async Task<IActionResult> ServiceDetails(int id)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            var Service = await _ServiceService.GetByIdAsync(id);
            if (Service == null) return NotFound();
            return View(Service);
        }

        // GET: Service/Create
        public IActionResult ServiceCreate()
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            return View();
        }

        // POST: Service/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ServiceCreate(ServiceViewModel Service)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            int userId = 1;
            if (ModelState.IsValid)
            {
                await _ServiceService.AddAsync(Service, userId);
                return RedirectToAction(nameof(Services));
            }
            return View(Service);
        }

        // GET: Service/Edit/5
        public async Task<IActionResult> ServiceEdit(int id)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            var data = await _ServiceService.GetByIdAsync(id);
            return View(new ServiceViewModel
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

        // POST: Service/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ServiceEdit(ServiceViewModel Service)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            int userId = 1;
            if (ModelState.IsValid)
            {
                await _ServiceService.UpdateAsync(Service, userId);
                return RedirectToAction(nameof(Services));
            }
            return View(Service);
        }

        // GET: Service/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            var Service = await _ServiceService.GetByIdAsync(id);
            if (Service == null) return NotFound();
            await _ServiceService.DeleteAsync(id);
            return RedirectToAction(nameof(Services));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            var data = await _context.TblService.FindAsync(id);
            if (data == null) return Json(new { success = false });

            data.IsActive = !data.IsActive;
            await _context.SaveChangesAsync();

            return Json(new { success = true, isActive = data.IsActive });
        }


        public async Task<IActionResult> OurServices()
        {
            var services = await _context.TblService
                .Where(t => !t.IsDeleted && t.IsActive)
                .OrderBy(t => t.Index)
                .Select(s => new ServiceViewModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    SubTitle = s.SubTitle,
                    Description = s.Description,
                    ImageUrl = s.ImageUrl
                }).ToListAsync();

            //var model = new CommonViewModel
            //{
            //    Services = services
            //};

            return View(services);
        }

        public async Task<IActionResult> Details(int Id)
        {
            //var result = CheckSession();
            //if (result != null) return result; // redirects if session invalid

            var service = await _ServiceService.GetFullServiceDetailsAsync(Id);
            if (service == null) return NotFound();

            var model = new ServiceViewModel
            {
                Id = service.Id,
                Name = service.Name,
                SubTitle = service.SubTitle,
                Description = service.Description,
                ImageUrl = service.ImageUrl,
                SubService = service.SubServices
                    .Where(sub => sub.IsActive && !sub.IsDeleted)
                    .Select(sub => new SubServiceViewModel
                    {
                        Id = sub.Id,
                        Name = sub.Name,
                        Description = sub.Description,
                        ImageUrl = sub.ImageUrl,
                        ImagePaths = sub.SubServiceImages?.Select(img => img.ImagePath).ToList() ?? new List<string>()
                    }).ToList()
            };

            return View(model);
        }

    }

}
