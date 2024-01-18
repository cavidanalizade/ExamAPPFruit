using ExamAPP.DAL;
using ExamAPP.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ExamAPP.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ReadFruitsVM read = new ReadFruitsVM()
            {
                fruits = await _context.fruits.ToListAsync()
            };
            return View(read);
        }


    }
}