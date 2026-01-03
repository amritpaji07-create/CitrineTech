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
    public class ContactController : Controller
    {
        private readonly IContactService _contactService;
        private readonly ApplicationDBContext _context;
        public ContactController(IContactService contactService, ApplicationDBContext context)
        {
            _contactService = contactService;
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

        // GET: Contact
        public async Task<IActionResult> Contacts(int? page)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            try
            {
                int pageSize = 10;
                int pageNumber = page ?? 1;

                var data = await _contactService.GetAllAsync();

                if (data == null || !data.Any())
                {
                    ViewBag.Message = "Data not found.";
                    return View(Enumerable.Empty<Contact>().ToPagedList(pageNumber, pageSize));
                }

                return View(data.ToPagedList(pageNumber, pageSize));
            }
            catch (Exception ex)
            {
                // Log error (you can use ILogger or any logging framework)
                ViewBag.Error = "An error occurred while fetching contact data.";
                return View("Error"); // Or redirect to a proper error page/view
            }
        }

        // GET: Contact/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            var contact = await _contactService.GetByIdAsync(id);
            if (contact == null) return NotFound();
            return View(contact);
        }

        // GET: Contact/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Contact/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ContactViewModel contact)
        {
            if (ModelState.IsValid)
            {
                await _contactService.AddAsync(contact);
                return RedirectToAction(nameof(Contacts));
            }
            return View(contact);
        }

        // GET: Contact/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            var contact = await _contactService.GetByIdAsync(id);
            if (contact == null) return NotFound();            
            return View(new ContactViewModel
            {
                Id = contact.Id,
                Name = contact.Name,
                Email = contact.Email,
                ContactNo = contact.ContactNo,               
                Remark = contact.Remark,                
            });
        }

        // POST: Contact/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ContactViewModel contact)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            if (ModelState.IsValid)
            {
                await _contactService.UpdateAsync(contact);
                return RedirectToAction(nameof(Contacts));
            }
            return View(contact);
        }

        // GET: Contact/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            var contact = await _contactService.GetByIdAsync(id);
            if (contact == null) return NotFound();
            await _contactService.DeleteAsync(id);
            return RedirectToAction(nameof(Contacts));
        }



        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var result = CheckSession();
            if (result != null) return result; // redirects if session invalid

            var data = await _context.TblContact.FindAsync(id);
            if (data == null) return Json(new { success = false });

            data.IsActive = !data.IsActive;
            await _context.SaveChangesAsync();

            return Json(new { success = true, isActive = data.IsActive });
        }

    }

}
