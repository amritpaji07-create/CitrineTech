using CitrineTech.Models;
using CitrineTechData.DataContext;
using CitrineTechData.ViewModel;
using CitrineTechService.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CitrineTech.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly IProjectService _projectService;
        public HomeController(ApplicationDBContext context, IProjectService projectService)
        {
            _context = context;
            _projectService = projectService;
        }

        public async Task<IActionResult> Index()
        {
            //Optional session logic
            //if (HttpContext.Session.GetString("UserEmail") != null)
            //{
            //    ViewBag.MySession = HttpContext.Session.GetString("UserEmail");
            //}

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

            var careerList = await _context.TblCareer
                .Where(t => !t.IsDeleted && t.IsActive)
                .AsNoTracking()
                .Select(s => new CareerViewModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    Email = s.Email,
                    Contact = s.Contact,
                    Location = s.Location,
                    Position = s.Position,
                    ExistingCvUrl = s.CvUrl
                }).ToListAsync();

            var contact = await _context.TblContact
            .FirstOrDefaultAsync(c => !c.IsDeleted);

            var partnerList = await _context.TblPartner
                .Where(t => !t.IsDeleted && t.IsActive)
                .Select(s => new PartnerViewModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    PartnerLogo = s.PartnerLogo
                }).ToListAsync();

            var patronsList = await _context.Patrons
                .Where(t => t.IsDeleted == false && t.IsActive == true)
                .Select(s => new PatronsViewModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    PatronLogo = s.PatronLogo
                }).ToListAsync();


            var testimonialList = await _context.TblTestimonial
                .Where(t => !t.IsDeleted && t.IsActive)
                .Select(s => new TestimonialViewModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    Date = s.Date,
                    ImageUrl = s.ImageUrl,
                    Description = s.Description,
                    Designation = s.Designation,
                }).ToListAsync();

            var teamList = await _context.TblTeam
                .Where(t => !t.IsDeleted && t.IsActive).OrderByDescending(t => t.CreatedDate)
                .Select(s => new TeamViewModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    Designation = s.Designation,
                    Description = s.Description,
                    ImageUrl = s.ImageUrl,
                }).ToListAsync();

            var projectList = await _context.TblProject
                .Where(t => !t.IsDeleted && t.IsActive).OrderByDescending(t => t.CreatedDate)
                .Select(s => new ProjectViewModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    SubTitle = s.SubTitle,
                    Description = s.Description,
                    ImageUrl = s.ImageUrl,
                    VideoUrl = s.VideoUrl,
                }).ToListAsync();

            var serviceList = await _context.TblService
                .Where(t => !t.IsDeleted && t.IsActive).OrderBy(t => t.Index)
                .Select(s => new ServiceViewModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    SubTitle = s.SubTitle,
                    Description = s.Description,
                    ImageUrl = s.ImageUrl,
                }).ToListAsync();

            var solutionList = await _context.TblSolution
                .Where(t => !t.IsDeleted && t.IsActive).OrderBy(t => t.Index)
                .Select(s => new SolutionViewModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    SubTitle = s.SubTitle,
                    Description = s.Description,
                    ImageUrl = s.ImageUrl,
                }).ToListAsync();

            var subSolutionList = await _context.TblSubSolution
                .Where(t => !t.IsDeleted && t.IsActive).OrderBy(t => t.Id)
                .Select(s => new SubSolutionViewModel
                {
                    Id = s.Id,
                    Name = s.Name,                    
                    Description = s.Description,
                    ImageUrl = s.ImageUrl,
                }).ToListAsync();


            var model = new CommonViewModel
            {
                Blogs = blogList,
                Career = careerList,
                Testimonial = testimonialList,
                Partners = partnerList,
                Patrons = patronsList,
                Team = teamList,
                Projects = projectList,
                Services = serviceList,
                Solution = solutionList,
                SubSolution = subSolutionList,
            };
            return View(model);
        }

        public async Task<IActionResult> About()
        {
            var teamList = await _context.TblTeam
            .Where(t => !t.IsDeleted && t.IsActive).OrderByDescending(t => t.CreatedDate)
            .Select(s => new TeamViewModel
            {
                Id = s.Id,
                Name = s.Name,
                Designation = s.Designation,
                Description = s.Description,
                ImageUrl = s.ImageUrl,
            }).ToListAsync();
            var model = new CommonViewModel
            {
                Team = teamList,
            };
            return View(model);
        }

        public IActionResult Project()
        {
            var project = _context.TblProject.Where(p => p.IsActive && p.IsDeleted == false).ToList();
            return View(project);
        }


        //public async Task<IActionResult> Solutions()
        //{
        //    var solutions = await _context.TblSolution
        //        .Where(t => !t.IsDeleted && t.IsActive).OrderByDescending(t => t.CreatedDate)                
        //        .Select(s => new SolutionViewModel
        //        {
        //            Id = s.Id,
        //            Name = s.Name,
        //            SubTitle = s.SubTitle,
        //            Description = s.Description,
        //            ImageUrl = s.ImageUrl
        //        }).ToListAsync();

        //    var model = new CommonViewModel
        //    {
        //        Solution = solutions
        //    };

        //    return View(model);
        //}



        public async Task<IActionResult> Solutions()
        {
            var projectList = await _context.TblSolution
                .Where(t => !t.IsDeleted && t.IsActive).OrderByDescending(t => t.CreatedDate)
                .Select(s => new SolutionViewModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    SubTitle = s.SubTitle,
                    Description = s.Description,
                    ImageUrl = s.ImageUrl,
                }).ToListAsync();

            //var testimonialList = await _context.TblTestimonial
            //.Where(t => !t.IsDeleted && t.IsActive)
            //.Select(s => new TestimonialViewModel
            //{
            //    Id = s.Id,
            //    Name = s.Name,
            //    Date = s.Date,
            //    ImageUrl = s.ImageUrl,
            //    Description = s.Description,
            //    Designation = s.Designation,
            //}).ToListAsync();

            //var model = new CommonViewModel
            //{
            //    Solution = projectList,
            //    Testimonial = testimonialList,
            //};
            //return View(model);
            if (projectList == null) return NotFound();
            return View(projectList);
        }

        public async Task<IActionResult> SubSolution(int id)
        {
            var subSolution = await _context.TblSubSolution
                .Where(t => t.Id == id && !t.IsDeleted && t.IsActive)
                .Select(s => new SubSolutionViewModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    ImageUrl = s.ImageUrl,
                    ImagePaths = s.SubSolutionImages.Select(img => img.ImagePath).ToList()
                })
                .FirstOrDefaultAsync();

            if (subSolution == null)
            {
                return NotFound();
            }

            return View(subSolution);
        }


        public async Task<IActionResult> Service()
        {
            var serviceList = await _context.TblService
                .Where(t => !t.IsDeleted && t.IsActive).OrderBy(t => t.Index)
            .Select(s => new ServiceViewModel
            {
                Id = s.Id,
                Name = s.Name,
                SubTitle = s.SubTitle,
                Description = s.Description,
                ImageUrl = s.ImageUrl,
            }).ToListAsync();
            var model = new CommonViewModel
            {
                Services = serviceList,
            };
            return View(model);
        }

        public async Task<IActionResult> SubService(int id)
        {
            var subService = await _context.TblSubService
                .Where(t => t.Id == id && !t.IsDeleted && t.IsActive)
                .Select(s => new SubServiceViewModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    ImageUrl = s.ImageUrl,
                    ImagePaths = s.SubServiceImages.Select(img => img.ImagePath).ToList()
                })
                .FirstOrDefaultAsync();

            if (subService == null)
            {
                return NotFound();
            }

            return View(subService);
        }

           
        public async Task<IActionResult> PDetails(int id)
        {
            if (id <= 0)
                return NotFound();

            // Assuming you have a service or DbContext
            var project = await _context.TblProject
                .Where(p => p.Id == id && !p.IsDeleted)
                .Select(p => new ProjectViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    SubTitle = p.SubTitle,
                    Description = p.Description,
                    ImageUrl = p.ImageUrl,                   
                })
                .FirstOrDefaultAsync();

            if (project == null)
                return NotFound();

            return View(project);
        }


       

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
