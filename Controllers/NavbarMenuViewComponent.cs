using CitrineTechData.DataContext;
using CitrineTechData.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CitrineTech.Controllers
{
    public class NavbarMenuViewComponent : ViewComponent
    {
        private readonly ApplicationDBContext _context;

        public NavbarMenuViewComponent(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = new CommonViewModel
            {
                Service = await _context.TblService
                                        .Include(s => s.SubServices).Where(t => !t.IsDeleted && t.IsActive).OrderBy(t => t.Index)   // 👈 Load subservices
                                        .ToListAsync(),

                Solutions = await _context.TblSolution
                                          .Include(s => s.SubSolutions).Where(t => !t.IsDeleted && t.IsActive).OrderBy(t => t.Index)// 👈 Load subsolutions
                                          .ToListAsync()
            };

            return View(model);
        }

    }
}
