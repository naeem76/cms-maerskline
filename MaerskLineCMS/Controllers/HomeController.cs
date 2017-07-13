using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.DirectoryServices.AccountManagement;
using System.Security.Claims;

namespace MaerskLineCMS.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //IEnumerable<string> groups = Startup.GetGroupMembershipsByObjectId();
            //Startup so = new Startup();
            //ArrayList groups = so.Groups();
            //groups.Contains("Admin")

            //ClaimsPrincipal cp = ClaimsPrincipal.Current;
            var cp = (ClaimsPrincipal)System.Web.HttpContext.Current.User;
            Models.GlobalVariables.fullname = "";
            Models.GlobalVariables.role = cp.FindFirst("roles").Value;
            Models.GlobalVariables.fullname = cp.FindFirst("name").Value;

             /*if (User.IsInRole("Admin"))
                 Models.GlobalVariables.role = "Admin";
             else if (User.IsInRole("Staff"))
                 Models.GlobalVariables.role = "Staff";*/
                 

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}