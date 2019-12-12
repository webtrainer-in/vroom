using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vroom.AppDbContext;
using vroom.Controllers.Resources;
using vroom.Models;
using vroom.Models.ViewModels;
using vroom.Helpers;

namespace vroom.Controllers
{
    [Authorize(Roles = Roles.Admin+","+Roles.Executive)]
    public class ModelController : Controller
    {
        private readonly VroomDbContext _db;
        private readonly IMapper _mapper;

        [BindProperty]
        public ModelViewModel ModelVM { get; set; }

        public ModelController(VroomDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
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

        [AllowAnonymous]
        [HttpGet("api/models/{MakeID}")]
        public IEnumerable<ModelResources> Models(int MakeID)
        {
            var models = _db.Models.ToList();
            var modelResources = models
                .Select(m => new ModelResources
                {
                    Id = m.Id,
                    Name = m.Name,
                    MakeID = m.MakeID
                }).ToList()
                .Where(m => m.MakeID == MakeID);
            return modelResources;
        }

        [AllowAnonymous]
        [HttpGet("api/models")]
        public IEnumerable<ModelResources> Models()
        {            
            var models = _db.Models.ToList();
            return _mapper.Map<List<Model>, List<ModelResources>>(models);
            
            //var modelResources = models
            //    .Select(m => new ModelResources
            //    {
            //        Id = m.Id,
            //        Name = m.Name
            //    }).ToList();         
        }
    }
}