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

namespace vroom.Controllers
{
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
            _db.SaveChanges();

            ///////////////////
            //Save Bike Logic
            ///////////////////
            
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
                _db.SaveChanges();
            }
            ///////////////////////

            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public IActionResult EditPost()
        {
            if (!ModelState.IsValid)
            {
                return View(BikeVM);
            }

            //Save Image
            string wwrootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;
            var BikeID = BikeVM.Bike.Id;
            var SavedBike = _db.Bikes.Find(BikeID);

            if (files.Count != 0)
            {
                var Extension = Path.GetExtension(files[0].FileName);
                var RelativeImagePath = Image.BikeImagePath + BikeID + Extension;
                var AbsImagePath = Path.Combine(wwrootPath, RelativeImagePath);


                using (var filestream = new FileStream(AbsImagePath, FileMode.Create))
                {
                    files[0].CopyTo(filestream);
                }
                SavedBike.ImagePath = RelativeImagePath;
            }
            SavedBike.MakeID = BikeVM.Bike.MakeID;
            SavedBike.ModelID= BikeVM.Bike.ModelID;
            SavedBike.Year = BikeVM.Bike.Year;            
            SavedBike.Mileage = BikeVM.Bike.Mileage;
            SavedBike.Price = BikeVM.Bike.Price;
            SavedBike.Currency = BikeVM.Bike.Currency;
            SavedBike.Features = BikeVM.Bike.Features;
            SavedBike.SellerName = BikeVM.Bike.SellerName;
            SavedBike.SellerEmail = BikeVM.Bike.SellerEmail;
            SavedBike.SellerPhone = BikeVM.Bike.SellerPhone;
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        //HTTP Get Method
        [HttpGet]
        public IActionResult Edit(int id)
        {
            BikeVM.Bike = _db.Bikes.Include(m => m.Make).Include(m => m.Model).SingleOrDefault(m => m.Id == id);
            if (BikeVM.Bike == null)
            {
                return NotFound();
            }

            return View(BikeVM);
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
    }
}