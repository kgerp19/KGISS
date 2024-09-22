using KGERP.Data.Models;
using KGERP.Service.ServiceModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;

namespace KGERP.Controllers.Custom_Authorization
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class ParentAuthorizedAttribute : AuthorizeAttribute
    {
        private ERPEntities _db = new ERPEntities();
        public override async void OnAuthorization(AuthorizationContext filterContext)
        {
            var isParentAuthorized = await IsParentAuthorized(filterContext);

            if (!isParentAuthorized)
            {
                HandleUnauthorizedRequest(filterContext);
            }
        }
    
        private async Task<bool> IsParentAuthorized(AuthorizationContext filterContext)
        {
            var routeData = filterContext.RequestContext.RouteData;
            var queryString = filterContext.HttpContext.Request.QueryString;
            var userid=System.Web.HttpContext.Current.User.Identity.Name;
            var controllerName = routeData.Values["controller"];
            var actionName = routeData.Values["action"];
            string companyId = queryString["companyId"];
            int id=Convert.ToInt32(companyId);
            var result =  checkmenuAsync(controllerName.ToString(), actionName.ToString(), userid, id);          
            if (result != null)
            {
             return true;
            }
            else
            {
             return false;
            }
            
        }


        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "AccessDenied", action = "Index" }));
        }



        private  vm checkmenuAsync(string controllerName, string actionName, string userid,int companyId)
        {
            vm menu = new vm();
            menu =   (from t1 in _db.CompanySubMenus
                                         join t2 in _db.CompanyUserMenus on t1.CompanySubMenuId equals t2.CompanySubMenuId into t2_join
                                         from t2 in t2_join.DefaultIfEmpty()
                                         where t2.UserId == userid&&t2.IsActive&&t1.Action == actionName&&t1.Controller == controllerName&&t1.CompanyId == companyId
                                         select new vm
                                         {
                                             Action = t1.Action,
                                             Controller = t1.Controller,
                                             IsActive = t1.IsActive,
                                         }).FirstOrDefault() ;

            return menu;

        }

        public class vm
        {
            public string Controller { get; set; }    
            public string Action { get; set; }    
            public bool IsActive { get; set; }    
        }
    }
}