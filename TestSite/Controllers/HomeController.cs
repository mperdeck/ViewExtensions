using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.IO;
using ViewExtensions;

namespace TestSite.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string pathInfo)
        {
           // string view = "~/Views/Home/Why.cshtml";
            //string view = pathInfo;
            //if ((view != null) && view.Contains("/"))
            //{
            //    view = Path.Combine("~/Views/", view);
            //}

            string view = Views.ByUrl(pathInfo).ViewPath;
            return View(view);


            //try
            //{
            //    return View(view);
            //}
            //catch (Exception e)
            //{
            //    string v2 = Path.Combine("~/Views/", view);
            //    return View(v2);

            //}
        }
    }
}
