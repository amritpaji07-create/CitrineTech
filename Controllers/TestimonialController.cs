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
    
    public class TestimonialController : Controller
    {
        private readonly ITestimonialService _testimonialService;
        private readonly ApplicationDBContext _context;
        public TestimonialController(ITestimonialService testimonialService, ApplicationDBContext context)
        {
            _testimonialService = testimonialService;
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

        // GET: /Testimonial
        public async Task<IActionResult> Testimonials(int? page)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid


            try
            {
                int pageSize = 10;
                int pageNumber = page ?? 1;

                var data = await _testimonialService.GetAllAsync();

                if (data == null || !data.Any())
                {
                    ViewBag.Message = "Data not found.";
                    return View(Enumerable.Empty<Testimonial>().ToPagedList(pageNumber, pageSize));
                }

                return View(data.ToPagedList(pageNumber, pageSize));
            }
            catch (Exception ex)
            {
                // Log error (you can use ILogger or any logging framework)
                ViewBag.Error = "An error occurred while fetching testimonial data.";
                return View("Error"); // Or redirect to a proper error page/view
            }
        }

        // GET: /Testimonial/Create
        public IActionResult TestimonialCreate()
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            return View();
        }

        // POST: /Testimonial/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TestimonialCreate(TestimonialViewModel model)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            if (ModelState.IsValid)
            {
                int userId = 1; // ✅ Replace with actual logged-in UserId
                await _testimonialService.AddAsync(model, userId);
                return RedirectToAction(nameof(Testimonials));
            }
            return View(model);
        }

        // GET: /Testimonial/Edit/5
        public async Task<IActionResult> TestimonialEdit(int id)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            var testimonial = await _testimonialService.GetByIdAsync(id);
            if (testimonial == null) return NotFound();

            var model = new TestimonialViewModel
            {
                Id = testimonial.Id,
                Name = testimonial.Name,
                Date = testimonial.Date,
                Designation = testimonial.Designation,
                Description = testimonial.Description,
                Index = testimonial.Index,
                IsActive = testimonial.IsActive,
                ExistingImageUrl = testimonial.ImageUrl
            };

            return View(model);
        }

        // POST: /Testimonial/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TestimonialEdit(TestimonialViewModel model)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            if (ModelState.IsValid)
            {
                int userId = 1; // ✅ Replace with actual logged-in UserId
                await _testimonialService.UpdateAsync(model, userId);
                return RedirectToAction(nameof(Testimonials));
            }
            return View(model);
        }

        // GET: /Testimonial/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid


            var testimonial = await _testimonialService.GetByIdAsync(id);
            if (testimonial == null) return NotFound();

            int userId = 1; // ✅ Replace with actual logged-in UserId
            await _testimonialService.DeleteAsync(id, userId);
            return RedirectToAction(nameof(Testimonials));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            var data = await _context.TblTestimonial.FindAsync(id);
            if (data == null) return Json(new { success = false });

            data.IsActive = !data.IsActive;
            await _context.SaveChangesAsync();

            return Json(new { success = true, isActive = data.IsActive });
        }


    }

}
