﻿using Microsoft.AspNetCore.Mvc;
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

            var customers = await _context.CustomerInfos.OrderByDescending(x => x.Id).ToListAsync();

            return View(customers);
        }

        [HttpPost]
        public async Task<IActionResult> Create(IFormFile file, CustomerInfo customerInfo)
        {
            if (customerInfo.FullName == null || customerInfo.Description == null)
            {
                return NotFound();
            }
            if (file == null)
            {
                return NotFound();
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
            var customers = _context.CustomerInfos.Where(f => f.Id == Id).FirstOrDefault();

            return PartialView("_EditPartialView", customers);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(IFormFile file, CustomerInfo customerInfo)
        {
            if (customerInfo.FullName == null || customerInfo.Description == null)
            {
                return NotFound();
            }

            var oldCustomers = await _context.CustomerInfos.FindAsync(customerInfo.Id);

            if(file != null) {
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);

                oldCustomers.FileName = file.FileName;
                oldCustomers.ContentType = file.ContentType;
                oldCustomers.Data = memoryStream.ToArray();
            }
                oldCustomers.FullName = customerInfo.FullName;
                oldCustomers.DateTime = DateTime.Now;
                oldCustomers.Description = customerInfo.Description;
           

            _context.CustomerInfos.Update(oldCustomers);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Detail(int Id)
        {
            var customers = _context.CustomerInfos.Where(f => f.Id == Id).FirstOrDefault();

            return PartialView("_DetailPartialView", customers);
        }

        [HttpPost]
        public IActionResult ViewPdf(int id)
        {
            var customers = _context.CustomerInfos.Where(f => f.Id == id).FirstOrDefault();

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