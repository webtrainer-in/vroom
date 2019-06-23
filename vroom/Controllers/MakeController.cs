using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using vroom.AppDbContext;
using vroom.Models;

namespace vroom.Controllers
{
    [Authorize(Roles ="Admin,Executive")]
    public class MakeController : Controller
    {
        private readonly VroomDbContext _db;

        public MakeController(VroomDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View(_db.Makes.ToList());
        }

        //HTTP Get Method
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Make make)
        {
            if(ModelState.IsValid)
            {
                _db.Add(make);
                _db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(make);

        }

        //HTTP Get Method
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var make = _db.Makes.Find(id);
            if(make==null)
            {
                return NotFound();
            }

            return View(make);
        }

        [HttpPost]
        public IActionResult Edit(Make make)
        {
            if (ModelState.IsValid)
            {
                _db.Update(make);
                _db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(make);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var make = _db.Makes.Find(id);
            if (make == null)
            {
                return NotFound();
            }
            _db.Makes.Remove(make);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }



    }
}