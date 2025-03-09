using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectPrn222.Models;

namespace ProjectPrn222.Controllers
{
    public class CustomerController : Controller
    {
        public CustomerController()
        {
            
        }

        public IActionResult Cart()
        {
            return View();
        }
    }
}
