using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PlaylistGenerator.ViewModels;

namespace PlaylistGenerator.Controllers
{
    public class ErrorController : Controller
    {
        private IndexViewModel viewModel = new IndexViewModel();
       
        public ActionResult NotFound()
        {
            return View(viewModel);
        }

        public ActionResult NoItemSelected()
        {
            return View(viewModel);
        }

        public ActionResult SomethingWentWrong() 
        {
            return View(viewModel);
        }
    }
}