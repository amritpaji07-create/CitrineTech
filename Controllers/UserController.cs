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
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly ApplicationDBContext _context;
        public UserController(IUserService userService, ApplicationDBContext context)
        {
            _userService = userService;
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

        // GET: /User
        public async Task<IActionResult> Users(int? page)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            try
            {
                int pageSize = 10;
                int pageNumber = page ?? 1;

                var data = await _userService.GetAllAsync();

                if (data == null || !data.Any())
                {
                    ViewBag.Message = "Data not found.";
                    return View(Enumerable.Empty<User>().ToPagedList(pageNumber, pageSize));
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

        // GET: /User/Create
        public IActionResult UserCreate()
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            return View();
        }

        // POST: /User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserCreate(User model)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            if (ModelState.IsValid)
            {
                await _userService.AddAsync(model);
                return RedirectToAction(nameof(Users));
            }
            return View(model);
        }

        // GET: /User/Edit/5
        public async Task<IActionResult> UserEdit(int id)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();

            var model = new User
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                ContactNo = user.ContactNo,                
            };

            return View(model);
        }

        // POST: /User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserEdit(User model)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            if (ModelState.IsValid)
            {
                await _userService.UpdateAsync(model);
                return RedirectToAction(nameof(Users));
            }
            return View(model);
        }

        // GET: /User/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();
            await _userService.DeleteAsync(id);
            return RedirectToAction(nameof(Users));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            var data = await _context.TblUser.FindAsync(id);
            if (data == null) return Json(new { success = false });

            data.IsActive = !data.IsActive;
            await _context.SaveChangesAsync();

            return Json(new { success = true, isActive = data.IsActive });
        }
    }

}
