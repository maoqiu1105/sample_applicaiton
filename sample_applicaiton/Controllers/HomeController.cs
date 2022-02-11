using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using sample_applicaiton.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace sample_applicaiton.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Sample_1Context _context;

        public HomeController(ILogger<HomeController> logger,Sample_1Context context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index(int page = 0)
        {
            int totalProducts = _context.Products.ToList().Count();
            int totalPages = totalProducts / 20 + (totalProducts % 20 == 0 ? 0 : 1);

            ViewData["pages"] = totalPages;

            if (page < 0)
            {
                page = 0;
            }
            else if (page >= totalPages)
            {
                page = totalPages - 1 ;
            }
            ViewData["currentPage"] = page;
            ViewData["products"] = _context.Products.Include(x=>x.Brand).OrderBy(x=>x.ProductId).Skip(page * 20).Take(20).ToList();
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "BrandName");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> index([Bind("ProductId,ProductName,BrandId,CategoryId,ModelYear,ListPrice")] Product product)
        {
            if (_context.Products.Find(product.ProductId) != null)
            {
                ModelState.AddModelError("ProductId", "We detected that you entered duplicated Product ID, Please try a new one");
            }
            
            if (ModelState.IsValid)
            {
                _context.Add(product);

                try
                {
                    //I don't believe this is a good way to handle product id since it's identity and auto-increment
                    //_context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.products ON");
                    await _context.SaveChangesAsync();
                    //_context.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.products OFF;");
                }
                catch (Exception e)
                {

                    throw;
                }
                

                return RedirectToAction(nameof(Index));
            }
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "BrandName", product.BrandId);

            ViewData["ErrorMessages"] = ModelState.Where(x=>x.Value.Errors.Count() > 0).Select(x=>x.Value.Errors[0].ErrorMessage).ToList();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
