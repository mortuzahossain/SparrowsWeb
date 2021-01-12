using System.Web.Mvc;

namespace WebApplication.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            string errMsg = null;
            //DataTable dt = (new ProcedureManager(DBName.Sparrows, ref errMsg)).ExecSPreturnDataTable(ExecutionSP.sp_get_shop_info_all, ref errMsg);
            //Session["UserGroupId"] = "1";
            ViewBag.Title = "Home Page";

            return View();
        }
    }
}
