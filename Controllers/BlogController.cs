using CitrineTechData.DataContext;
using CitrineTechData.DataModel;
using CitrineTechData.ViewModel;
using CitrineTechService.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Extensions;

namespace CitrineTech.Controllers
{
    public class BlogController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly IBlogService _blogService;        
        public BlogController(IBlogService blogService, ApplicationDBContext context)
        {
            _blogService = blogService;
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

        // GET: Blog
        public async Task<IActionResult> Blogs(int? page)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            try
            {
                int pageSize = 10;
                int pageNumber = page ?? 1;

                var abouts = await _blogService.GetAllAsync();

                if (abouts == null || !abouts.Any())
                {
                    ViewBag.Message = "Data not found.";
                    return View(Enumerable.Empty<Blog>().ToPagedList(pageNumber, pageSize));
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

        // GET: Blog/Details/5
        public async Task<IActionResult> BlogDetails(int id)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid


            var blog = await _blogService.GetByIdAsync(id);
            if (blog == null) return NotFound();
            return View(blog);
        }

        public async Task<IActionResult> Details(int id)
        {
            var blog = await _blogService.GetDetailsByIdAsync(id);
            if (blog == null) return NotFound();
            return View(blog);
        }

        public async Task<IActionResult> OurBlogs()
        {
            var blogList = await _context.TblBlog
                .Where(t => !t.IsDeleted && t.IsActive).OrderBy(t => t.Index)
                .Select(s => new BlogViewModel
                {
                    Id = s.Id,
                    Title = s.Title,
                    SubTitle = s.SubTitle,
                    Description = s.Description,
                    BlogDate = s.BlogDate,
                    ImageUrl = s.ImageUrl
                }).ToListAsync();

            if (blogList == null) return NotFound();
            return View(blogList);
        }




        // GET: Blog/Create
        public IActionResult BlogCreate()
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            return View();
        }

        // POST: Blog/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BlogCreate(BlogViewModel blog)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            int userId = 1;
            if (ModelState.IsValid)
            {
                await _blogService.AddBlogAsync(blog, userId);
                return RedirectToAction(nameof(Blogs));
            }
            return View(blog);
        }

        // GET: Blog/Edit/5
        public async Task<IActionResult> BlogEdit(int id)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid


            var data = await _blogService.GetByIdAsync(id);
            return View(new BlogViewModel
            {
                Id = data.Id,
                Title = data.Title,
                SubTitle = data.SubTitle,
                BlogDate = data.BlogDate,
                Index = data.Index,
                Description = data.Description,
                ImageUrl = data.ImageUrl,
                ExistingImages  = data.BlogImages.Select(img => img.ImagePath).ToList()
            });            
        }

        // POST: Blog/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BlogEdit(BlogViewModel blog)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid


            int userId = 1;
            if (ModelState.IsValid)
            {
                await _blogService.UpdateBlogAsync(blog, userId);
                return RedirectToAction(nameof(Blogs));
            }
            return View(blog);
        }

        // GET: Blog/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            var blog = await _blogService.GetByIdAsync(id);
            if (blog == null) return NotFound();
            await _blogService.DeleteAsync(id);
            return RedirectToAction(nameof(Blogs));
        }


        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var data = await _context.TblBlog.FindAsync(id);
            if (data == null) return Json(new { success = false });

            data.IsActive = !data.IsActive;
            await _context.SaveChangesAsync();

            return Json(new { success = true, isActive = data.IsActive });
        }

    }

}
