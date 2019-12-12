using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vroom.AppDbContext;
using vroom.Helpers;
using vroom.Models;
using vroom.Models.ViewModels;
using cloudscribe.Pagination.Models;
using System.Diagnostics;

namespace vroom.Controllers
{   
    [Authorize(Roles = Roles.Admin +"," + Roles.Executive)]
    public class BikeController : Controller
    {
        private readonly VroomDbContext _db;
        private readonly HostingEnvironment _hostingEnvironment;

        [BindProperty]
        public BikeViewModel BikeVM { get; set; }

        public BikeController(VroomDbContext db, HostingEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;
                
            BikeVM = new BikeViewModel()
            {
                Makes = _db.Makes.ToList(),
                Models = _db.Models.ToList(),
                Bike = new Models.Bike()                         
            };
        }

        [AllowAnonymous]
        public IActionResult Index(string searchString, string sortOrder, int pageNumber=1, int pageSize=3)
        {
            ViewBag.CurrentSortOrder = sortOrder;
            ViewBag.CurrentFilter = searchString;
            ViewBag.PriceSortParam = String.IsNullOrEmpty(sortOrder) ? "price_desc" : "";
            int ExcludeRecords = (pageSize * pageNumber) - pageSize;

            var Bikes = from b in _db.Bikes.Include(m => m.Make).Include(m => m.Model)
                        select b;

            var BikeCount = Bikes.Count();

            if (!String.IsNullOrEmpty(searchString))
            {
                Bikes = Bikes.Where(b => b.Make.Name.Contains(searchString));
                BikeCount = Bikes.Count();
            }

            //Sorting Logic
            switch(sortOrder)
            {
                case "price_desc":
                    Bikes = Bikes.OrderByDescending(b => b.Price);
                    break;
                default:
                    Bikes = Bikes.OrderBy(b => b.Price);
                    break;
            }

            Bikes =Bikes
            .Skip(ExcludeRecords)
                .Take(pageSize);

            var result = new PagedResult<Bike>
            {
                Data = Bikes.AsNoTracking().ToList(),
                TotalItems = BikeCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            

            return View(result);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            BikeVM.Bike = _db.Bikes.SingleOrDefault(b => b.Id == id);

            //Filter the models associated to the selected make
            BikeVM.Models = _db.Models.Where(m => m.MakeID == BikeVM.Bike.MakeID);
            
            if(BikeVM.Bike==null)
            {
                return NotFound();
            }
            return View(BikeVM);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public IActionResult EditPost()
        {
            if (!ModelState.IsValid)
            {
                BikeVM.Makes = _db.Makes.ToList();
                BikeVM.Models = _db.Models.ToList();
                return View(BikeVM);
            }
            _db.Bikes.Update(BikeVM.Bike);
            UploadImageIfAvailable();
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }



        public IActionResult Create()
        {
            return View(BikeVM);
        }


        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public IActionResult CreatePost()
        {
            if (!ModelState.IsValid)
            {
                BikeVM.Makes = _db.Makes.ToList();
                BikeVM.Models = _db.Models.ToList();
                return View(BikeVM);
            }
            _db.Bikes.Add(BikeVM.Bike);

            UploadImageIfAvailable();

            _db.SaveChanges();
            
            return RedirectToAction(nameof(Index));
        }

        private void UploadImageIfAvailable()
        {
            //Get BikeID we have saved in database            
            var BikeID = BikeVM.Bike.Id;

            //Get wwrootPath to save the file on server
            string wwrootPath = _hostingEnvironment.WebRootPath;

            //Get the Uploaded files
            var files = HttpContext.Request.Form.Files;

            //Get the reference of DBSet for the bike we have saved in our database
            var SavedBike = _db.Bikes.Find(BikeID);


            //Upload the file on server and save the path in database if user have submitted file
            if (files.Count != 0)
            {
                //Extract the extension of submitted file
                var Extension = Path.GetExtension(files[0].FileName);

                //Create the relative image path to be saved in database table 
                var RelativeImagePath = Image.BikeImagePath + BikeID + Extension;

                //Create absolute image path to upload the physical file on server
                var AbsImagePath = Path.Combine(wwrootPath, RelativeImagePath);


                //Upload the file on server using Absolute Path
                using (var filestream = new FileStream(AbsImagePath, FileMode.Create))
                {
                    files[0].CopyTo(filestream);
                }

                //Set the path in database
                SavedBike.ImagePath = RelativeImagePath;
               }
            }


        [HttpPost]
        public IActionResult Delete(int id)
        {
            Bike Bike = _db.Bikes.Find(id);
            if (Bike == null)
            {
                return NotFound();
            }
            _db.Bikes.Remove(Bike);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult View(int id)
        {
            BikeVM.Bike = _db.Bikes.SingleOrDefault(b => b.Id == id);

            if (BikeVM.Bike == null)
            {
                return NotFound();
            }
            return View(BikeVM);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}