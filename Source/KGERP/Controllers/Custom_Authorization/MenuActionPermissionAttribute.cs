using KGERP.Helpers;
using KGERP.Utility;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace KGERP.Controllers.Custom_Authorization
{
    /// <summary>
    /// Server-side attribute that enforces action-level permissions.
    /// 
    /// Strategy (3-level):
    ///   1. Session map lookup: Controller+Action+CompanyId → CompanyUserMenuId → PK check
    ///   2. Session stored PK: for POST/Delete after a GET page load
    ///   3. Fallback DB match: Controller+Action+CompanyId directly
    /// 
    /// CompanyUserMenuId is NEVER exposed in the URL.
    /// 
    /// Session handling:
    ///   - Session expired → redirect to login
    ///   - Session map missing → auto-rebuild from DB (no user error)
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class,
        Inherited = true, AllowMultiple = true)]
    public class MenuActionPermissionAttribute : AuthorizeAttribute
    {
        private readonly MenuAction _action;

        public MenuActionPermissionAttribute(MenuAction action)
        {
            _action = action;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var httpContext = filterContext.HttpContext;

            // Session or login missing — redirect to login gracefully
            if (httpContext.Session == null || httpContext.Session["Id"] == null)
            {
                HandleSessionExpired(filterContext);
                return;
            }

            int companyId = ResolveCompanyId(filterContext);
            string controllerName = filterContext.RouteData.Values["controller"]?.ToString();
            string actionName = filterContext.RouteData.Values["action"]?.ToString();

            // Strategy 1: Resolve CompanyUserMenuId from session permission map
            // The map auto-rebuilds from DB if lost (app pool recycle etc.)
            long companyUserMenuId = MenuPermissionProvider.ResolveCompanyUserMenuId(
                controllerName, actionName, companyId);

            if (companyUserMenuId > 0)
            {
                // Store for subsequent POST/AJAX/Delete actions on same page
                httpContext.Session["CompanyUserMenuId"] = companyUserMenuId;

                bool allowed = MenuPermissionProvider.HasPermission(_action, companyUserMenuId);
                if (!allowed) HandleDenied(filterContext);
                return;
            }

            // Strategy 2: Use stored CompanyUserMenuId from session
            // (set by the GET page load — covers Delete/Edit POST actions)
            object sessionMenuId = httpContext.Session["CompanyUserMenuId"];
            if (sessionMenuId != null)
            {
                long storedId;
                if (long.TryParse(sessionMenuId.ToString(), out storedId) && storedId > 0)
                {
                    bool allowed = MenuPermissionProvider.HasPermission(_action, storedId);
                    if (!allowed) HandleDenied(filterContext);
                    return;
                }
            }

            // Strategy 3: Fallback to Controller + Action + CompanyId DB matching
            if (companyId > 0)
            {
                bool allowed = MenuPermissionProvider.HasPermission(
                    _action, controllerName, actionName, companyId);
                if (!allowed) HandleDenied(filterContext);
                return;
            }

            // Nothing resolved — deny
            HandleDenied(filterContext);
        }

        /// <summary>
        /// Resolves companyId from QueryString → Form → Route → Session.
        /// </summary>
        private int ResolveCompanyId(AuthorizationContext filterContext)
        {
            var request = filterContext.HttpContext.Request;
            int companyId = 0;

            // 1. Query string
            string value = request.QueryString["companyId"];
            if (!string.IsNullOrEmpty(value) && int.TryParse(value, out companyId) && companyId > 0)
                return companyId;

            // 2. Form data
            value = request.Form["CompanyId"];
            if (string.IsNullOrEmpty(value))
            {
                value = request.Form["companyId"]
                     ?? request.Form["CompanyUserMenu.CompanyId"]
                     ?? request.Form["Voucher.CompanyId"];
            }
            if (!string.IsNullOrEmpty(value) && int.TryParse(value, out companyId) && companyId > 0)
                return companyId;

            // 3. Route values
            object routeVal = filterContext.RouteData.Values["companyId"];
            if (routeVal != null && int.TryParse(routeVal.ToString(), out companyId) && companyId > 0)
                return companyId;

            // 4. Session fallback
            object sessionVal = filterContext.HttpContext.Session["CompanyId"];
            if (sessionVal != null && int.TryParse(sessionVal.ToString(), out companyId) && companyId > 0)
                return companyId;

            return 0;
        }

        /// <summary>
        /// Session expired or missing — redirect to login.
        /// AJAX: 401 JSON. Normal: redirect.
        /// </summary>
        private void HandleSessionExpired(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.HttpContext.Response.StatusCode = 401;
                filterContext.HttpContext.Response.ContentType = "application/json";
                filterContext.Result = new JsonResult
                {
                    Data = new
                    {
                        success = false,
                        message = "Your session has expired. Please login again.",
                        sessionExpired = true
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            else
            {
                filterContext.Result = new RedirectResult("~/User/Login");
            }
        }

        /// <summary>
        /// Permission denied — 403 or redirect.
        /// </summary>
        private void HandleDenied(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.HttpContext.Response.StatusCode = 403;
                filterContext.HttpContext.Response.ContentType = "application/json";
                filterContext.Result = new JsonResult
                {
                    Data = new
                    {
                        success = false,
                        message = "You do not have permission to perform this action."
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new
                    {
                        controller = "AccessDenied",
                        action = "Index"
                    }));
            }
        }
    }
}
