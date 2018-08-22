using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Goui.AspNetCore;
using GouiFormsApp.Models;
using GouiFormsApp.Pages;
using Xamarin.Forms;

namespace GouiFormsApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var page = new MainPage();
            var element = page.GetGouiElement();
            return new ElementResult(element, "Goui Forms");
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
