using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RBC_Consulting_WebApplication.DAL;
using RBC_Consulting_WebApplication.Extensions;
using RBC_Consulting_WebApplication.Models;
using System.Diagnostics;
using System.Reflection.Metadata;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace RBC_Consulting_WebApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly RBC_Context _context;

        private readonly IHostingEnvironment _hostingEnvironment;

        public HomeController(ILogger<HomeController> logger, RBC_Context context, IHostingEnvironment hostingEnvironment)
        {
            _logger = logger;

            _context = context;

            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
           
            var customers = await _context.CustomerInfos.ToListAsync();

            return View(customers);
        }

        [HttpPost]
        public async Task<IActionResult> Create(IFormFile file, CustomerInfo customerInfo)
        {
            if (customerInfo == null)
            {
                return BadRequest();
            }

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            var pdfFile = new CustomerInfo
            {
                FullName = customerInfo.FullName,
                DateTime = DateTime.Now,
                Description = customerInfo.Description,
                FileName = file.FileName,
                ContentType = file.ContentType,
                Data = memoryStream.ToArray()
            };



            await _context.CustomerInfos.AddRangeAsync(pdfFile);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public IActionResult Edit(int Id)
        {
            var customers =  _context.CustomerInfos.Where(f => f.Id == Id).FirstOrDefault();

            return PartialView("_EditPartialView", customers);
        }

        [HttpGet]
        public IActionResult Detail(int Id)
        {
            var customers = _context.CustomerInfos.Where(f => f.Id == Id).FirstOrDefault();

            return PartialView("_DetailPartialView", customers);
        }
        //[HttpGet]
        //public async Task<IActionResult> DownloadPdf(int id)
        //{
        //    var pdfFile = await _context.CustomerInfos.FindAsync(id);
        //    if (pdfFile == null)
        //        return NotFound();

        //    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files", pdfFile.FileName);
        //    if (!System.IO.File.Exists(filePath))
        //    {
        //        return NotFound("File not found");
        //    }

        //    var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        //    return File(fileStream, "application/pdf");
        //   // return File(pdfFile.Data, pdfFile.ContentType, pdfFile.FileName);
        //}

        [HttpPost]
        public IActionResult ViewPdf(int fileId)
        {
            var customers = _context.CustomerInfos.Where(f => f.Id == fileId).FirstOrDefault();

            String base64 = System.Convert.ToBase64String(customers.Data);

            byte[] newBytes = Convert.FromBase64String(base64);
            var stringBase64 = "data:application/pdf;base64," + Convert.ToBase64String(customers.Data, 0, customers.Data.Length);

            return Json(new
            {
                base64 = stringBase64

            }); ;
        }


        public async Task<IActionResult> Delete(int Id)
        {
           var customers = await _context.CustomerInfos.FindAsync(Id);
             _context.CustomerInfos.Remove(customers);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
       
       

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}