using CitrineTechData.DataContext;
using CitrineTechData.DataModel;
using CitrineTechData.ViewModel;
using CitrineTechService.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace CitrineTech.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly IPartnersService _partnersService;
        private readonly IUserService _userService;
        private readonly IBlogService _blogService;
        private readonly IServiceService _serviceService;
        private readonly ISolutionService _solutionService;

        public AccountController(ApplicationDBContext context, 
            IPartnersService partnersService, 
            IUserService userService,
            IBlogService blogService,
            IServiceService serviceService,
            ISolutionService solutionService
            )
        {
            _context = context;
            _userService = userService;
            _blogService = blogService;
            _serviceService = serviceService;
            _solutionService = solutionService;
            _partnersService  = partnersService;
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


        public async Task<IActionResult> Dashboard()
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid


            ViewBag.Blogs = (await _blogService.GetAllAsync()).Count();
            ViewBag.Users = (await _userService.GetAllAsync()).Count();
            ViewBag.Services = (await _serviceService.GetAllAsync()).Count();
            ViewBag.Solutions = (await _solutionService.GetAllAsync()).Count();
            ViewBag.Partners = (await _partnersService.GetAllAsync()).Count();

            return View();
        }

        public IActionResult Profile()
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            var user = _context.TblTeam.Find(userId);
            return View(user);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.TblUser
                    .FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == model.Password && u.IsDeleted == false);

                if (user != null)
                {
                    // Login Success: Save data into session or cookie as needed
                    HttpContext.Session.SetString("Email", user.Email);
                    HttpContext.Session.SetString("Name", user.Name);
                    HttpContext.Session.SetInt32("UserId", user.Id);                    
                    HttpContext.Session.SetString("UserRole", user.Role);

                    if (user.Role == "Admin")
                    {
                        return RedirectToAction("Dashboard", "Account"); 
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid Email or Password");
                }
            }
            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }


        //[HttpPost]
        //public JsonResult CheckEmailExists(string email)
        //{
        //    var user = _context.TblUsers.FirstOrDefault(u => u.Email == email && !u.IsDeleted);
        //    if (user != null)
        //        return Json(new { success = true });

        //    return Json(new { success = false, message = "Email not registered." });
        //}

        //[HttpPost]
        //public JsonResult UpdatePassword(string email, string newPassword)
        //{
        //    var user = _context.TblUsers.FirstOrDefault(u => u.Email == email && !u.IsDeleted);
        //    if (user != null)
        //    {
        //        user.Password = newPassword; // consider hashing in production
        //        _context.SaveChanges();
        //        return Json(new { success = true });
        //    }

        //    return Json(new { success = false, message = "Failed to update password." });
        //}




    }
}