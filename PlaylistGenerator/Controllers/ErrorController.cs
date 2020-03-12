using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PlaylistGenerator.ViewModels;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Models;

namespace PlaylistGenerator.Controllers
{
    public class ErrorController : Controller
    {
        private IndexViewModel viewModel = new IndexViewModel();
       
        public ActionResult NotFound()
        {
            setTempData();
            return View(viewModel);
        }

        public ActionResult NoItemSelected()
        {
            setTempData();
            return View(viewModel);
        }

        public ActionResult SomethingWentWrong() 
        {
            setTempData();
            return View(viewModel);
        }

        public void setTempData()
        {
            viewModel = (IndexViewModel)TempData["ViewModel"];

            TempData["Api"] = viewModel.api;
            TempData["User"] = viewModel.profile;
            TempData["Token"] = (Token)TempData["Token"];
            TempData["Auth"] = (AuthorizationCodeAuth)TempData["Auth"];
            TempData["ViewModel"] = viewModel;
        }
    }
}