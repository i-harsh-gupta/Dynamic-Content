using DynamicContent.Data;
using DynamicContent.Models;
using DynamicContent.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DynamicContent.Controllers
{
    [Authorize]
    public class PostController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PostController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            IEnumerable<Post> posts = _context.Posts;
            return View(posts);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(PageCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var url = _context.Posts.FirstOrDefault(e => e.PageUrl == model.PageUrl);
                if (url == null)
                {
                    string uniqueFileName = ProcessUploadedFile(model);
                    var post = new Post()
                    {
                        PageTitle = model.PageTitle,
                        PageUrl = model.PageUrl,
                        PageContent = model.PageContent,
                        PhotoPath = uniqueFileName
                    };
                    _context.Add(post);
                    _context.SaveChanges();
                    return RedirectToRoute(new
                    {
                        controller = "Post",
                        action = "DynamicPage",
                        pageUrl = model.PageUrl
                    });
                }
                else
                {
                    ModelState.AddModelError("", "This url has already been taken");
                }
            }
            return View(model);
        }

        private string ProcessUploadedFile(PageCreateViewModel model)
        {
            string uniqueFileName = null;
            if (model.Photo != null)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "files");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Photo.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult DynamicPage(string? pageUrl)
        {
            var pageDetails = _context.Posts.FirstOrDefault(e => e.PageUrl == pageUrl);
            if (pageDetails == null)
            {
                ViewBag.ErrorMessage = $"Page with url = {pageUrl} cannot be found";
                return View("NotFound");
            }

            var page = new PageDetailsViewModel()
            {
                Post = pageDetails
            };
            return View(page);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            var page = _context.Posts.Find(id.Value);
            if (id == null)
            {
                ViewBag.ErrorMessage = $"Page with id = {id} cannot be found";
            }
            var model = new PageEditViewModel()
            {
                PageTitle = page.PageTitle,
                PageContent = page.PageContent,
                PageUrl = page.PageUrl,
                ExistingPhotoPath = page.PhotoPath
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(PageEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var page = _context.Posts.Find(model.Id);
                if (page == null)
                {
                    ViewBag.ErrorMessage = $"Page with id = {model.Id} cannot be found";
                    return View("NotFound");
                }
                page.PageTitle = model.PageTitle;
                page.PageUrl = model.PageUrl;
                page.PageContent = model.PageContent;

                if (model.Photo != null)
                {
                    if (model.ExistingPhotoPath != null)
                    {
                        string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "files", model.ExistingPhotoPath);
                        System.IO.File.Delete(filePath);
                    }
                    page.PhotoPath = ProcessUploadedFile(model);
                }

                var updatedPage = _context.Update(page);
                _context.SaveChanges();
                return RedirectToRoute(new
                {
                    controller = "Post",
                    action = "DynamicPage",
                    pageUrl = model.PageUrl
                });
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            var page = _context.Posts.FirstOrDefault(e => e.Id == id.Value);
            if (page == null)
            {
                ViewBag.ErrorMessage = $"Page with url = {page.PageUrl} cannot be found";
                return View("NotFound");
            }
            var model = new PageDeleteViewModel()
            {
                PageTitle = page.PageTitle,
                PageContent = page.PageContent,
                PageUrl = page.PageUrl,
                ExistingPhotoPath = page.PhotoPath
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Delete(PageDeleteViewModel model)
        {
            var page = _context.Posts.FirstOrDefault(e => e.Id == model.Id);
            if (page != null)
            {
                if (model.ExistingPhotoPath != null)
                {
                    string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "files", model.ExistingPhotoPath);
                    System.IO.File.Delete(filePath);
                }
                _context.Posts.Remove(page);
                _context.SaveChanges();
                return RedirectToAction("index", "post");
            }
            return View(model);
        }
    }
}

