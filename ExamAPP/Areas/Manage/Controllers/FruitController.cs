using ExamAPP.Areas.Manage.ViewModels;
using ExamAPP.DAL;
using ExamAPP.Helper;
using ExamAPP.Models;
using ExamAPP.ViewModels;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Reflection.Metadata;

namespace ExamAPP.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles = "Admin")]

    public class FruitController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IValidator<CreateFruitVM> _createValidator;
        private readonly IValidator<UpdateFruitVM> _updateValidator;
        private readonly IWebHostEnvironment _env;

        public FruitController(AppDbContext context, IValidator<CreateFruitVM> createValidator, IValidator<UpdateFruitVM> updateValidator, IWebHostEnvironment env)
        {
            _context = context;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _env = env; 
        }

        public async Task<IActionResult> Index()
        {
            ReadFruitsManageVM readFruitsVM = new ReadFruitsManageVM()
            {
                fruits = await _context.fruits.ToListAsync()
            };
            return View(readFruitsVM);
        }
        public async Task<IActionResult> Create()
        {
            return View();

        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateFruitVM createFruitVM)
        {
            ValidationResult result = await _createValidator.ValidateAsync(createFruitVM);
            if (!result.IsValid)
            {
                result.AddToModelState(this.ModelState);
                return View("Create" , createFruitVM);
                
            }
            Fruit fruit = new Fruit()
            {
                Title = createFruitVM.Title,
                Subtitle = createFruitVM.Subtitle,
                ImageUrl = createFruitVM.Image.UploadFile(envPath:_env.WebRootPath ,"/Upload/"),
                CreatedAt = DateTime.Now,

                
            };
            await _context.fruits.AddAsync(fruit);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Update(int id)
        {
            TempData["error"] = "";
            if (id <= 0)
            {
                TempData["error"] = "Problem bas verdi";
                return RedirectToAction(nameof(Index));
            }
            Fruit fruit = await _context.fruits.Where(b => b.Id == id).FirstOrDefaultAsync();
            if (fruit is null)
            {
                TempData["error"] = "Problem bas verdi";
                return RedirectToAction(nameof(Index));
            }
            UpdateFruitVM updateFruitVM = new UpdateFruitVM()
            {
                Id = id,
                Title = fruit.Title,
                Subtitle = fruit.Subtitle,
                ImageUrl = fruit.ImageUrl,
                CreatedAt = fruit.CreatedAt,

            };

            return View(updateFruitVM);

        }

        [HttpPost]

        public async Task<IActionResult> Update(UpdateFruitVM updateFruitVM)
        {
            ValidationResult result = await _updateValidator.ValidateAsync(updateFruitVM);

            if (!result.IsValid)
            {
                result.AddToModelState(this.ModelState);
                return View("Update", updateFruitVM);
            }
            if (!updateFruitVM.Image.CheckContent("image/"))
            {
                ModelState.AddModelError("Image", "Duzgun format daxil edin");
                return View();
            }
            Fruit fruit = await _context.fruits.FindAsync(updateFruitVM.Id);
            TempData["error"] = "";
            if (fruit is null)
            {
                TempData["error"] = "Problem bas verdi";
                return RedirectToAction(nameof(Index));

            }
            fruit.Title = updateFruitVM.Title;
            fruit.Subtitle = updateFruitVM.Subtitle;
            fruit.CreatedAt = updateFruitVM.CreatedAt;
            fruit.ImageUrl = updateFruitVM.Image.UploadFile(_env.WebRootPath, "/Upload/");
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));


        }
        public async Task<IActionResult> Delete(int id)
        {
            Fruit fruit = await _context.fruits.FirstOrDefaultAsync(b => b.Id == id);
            TempData["error"] = "";

            if (fruit is null)
            {
                TempData["error"] = "Problem Bas verdi";
                return RedirectToAction(nameof(Index));


            }
            if (fruit.ImageUrl == null)
            {
                TempData["error"] = "Problem Bas verdi";
                return RedirectToAction(nameof(Index));
            }
            fruit.ImageUrl.RemoveFile(_env.WebRootPath, @"\Upload\Blog\");
            _context.fruits.Remove(fruit);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));


        }

    }
}
