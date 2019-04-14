using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vroom.AppDbContext;
using vroom.Models;
using vroom.Models.ViewModels;

namespace vroom.Controllers
{
    public class ModelController : Controller
    {
        private readonly VroomDbContext _db;

        [BindProperty]
        public ModelViewModel ModelVM { get; set; }

        public ModelController(VroomDbContext db)
        {
            _db = db;
            ModelVM = new ModelViewModel()
            {
                Makes = _db.Makes.ToList(),
                Model = new Models.Model()
            };
        }
        public IActionResult Index()
        {
            var model = _db.Models.Include(m => m.Make);
            return View(model.ToList());
        }

        public IActionResult Create()
        {
            return View(ModelVM);
        }
        [HttpPost,ActionName("Create")]
        [ValidateAntiForgeryToken]
        public IActionResult CreatePost()
        {
            if (!ModelState.IsValid)
            {
                return View(ModelVM);
            }
            _db.Models.Add(ModelVM.Model);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public IActionResult EditPost()
        {            
            if (!ModelState.IsValid)
            {
                return View(ModelVM);
            }
            //var MakeFromDB = _db.Models.Where(m => m.Id == id).FirstOrDefault();
            //if(MakeFromDB==null)
            //{
            //    return NotFound();
            //}
            //MakeFromDB.Name = ModelVM.Model.Name;
            //MakeFromDB.MakeID = ModelVM.Model.MakeID;   
            _db.Update(ModelVM.Model);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        //HTTP Get Method
        [HttpGet]
        public IActionResult Edit(int id)
        {
            ModelVM.Model = _db.Models.Include(m => m.Make).SingleOrDefault(m => m.Id == id);            
            if (ModelVM.Model == null)
            {
                return NotFound();
            }

            return View(ModelVM);
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            Model model = _db.Models.Find(id);
            if (model == null)
            {
                return NotFound();
            }
            _db.Models.Remove(model);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }



    }
}