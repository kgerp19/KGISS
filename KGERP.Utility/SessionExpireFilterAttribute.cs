using System.Web;
using System.Web.Mvc;

namespace KGERP.Utility
{
    //public class SessionExpireAttribute : System.Web.Mvc.ActionFilterAttribute
    //{
    //    public override void OnActionExecuting(ActionExecutingContext filterContext)
    //    {
    //        HttpContext ctx = HttpContext.Current;
    //        // check  sessions here
    //        if (HttpContext.Current.Session["UserName"] == null)
    //        {
    //            filterContext.Result = new RedirectResult("~/user/login");
    //            return;
    //        }
    //        base.OnActionExecuting(filterContext);
    //    }
    //}

}
