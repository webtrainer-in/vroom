using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using vroom.Models;

namespace vroom.Controllers
{
    public class MakeController : Controller
    {
        //make
        //make/bikes
        [Route("Make")]
        [Route("Make/Bikes")]
        public IActionResult Bikes()
        {
            Make make = new Make { Id = 1, Name = "Harley Davidson" };
            return View(make);            
           
        }
        [Route("make/bikes/{year:int:length(4)}/{month:int:range(1,13)}")]
        public IActionResult ByYearMonth(int year, int month)
        {
            return Content(year + ";" + month);
        }

    }
}