using KGERP.Utility;
using System.Web;
using System.Web.Mvc;

namespace KGERP.Helpers
{
    /// <summary>
    /// Razor HtmlHelper extension to check action-level permissions in views.
    /// Resolves CompanyUserMenuId from session map (hidden from URL).
    /// </summary>
    public static class PermissionHelper
    {
        /// <summary>
        /// Check by CompanyUserMenuId directly (when you have the PK).
        /// </summary>
        public static bool HasMenuAction(this HtmlHelper html, MenuAction action, long companyUserMenuId)
        {
            return MenuPermissionProvider.HasPermission(action, companyUserMenuId);
        }

        /// <summary>
        /// Auto-resolve: tries session map → session stored PK → fallback to controller+action.
        /// This is the most commonly used overload.
        /// </summary>
        public static bool HasMenuAction(this HtmlHelper html, MenuAction action, int companyId)
        {
            // Try session map: controller+action → CompanyUserMenuId
            string controllerName = html.ViewContext.RouteData.Values["controller"]?.ToString();
            string actionName = html.ViewContext.RouteData.Values["action"]?.ToString();

            long companyUserMenuId = MenuPermissionProvider.ResolveCompanyUserMenuId(
                controllerName, actionName, companyId);

            if (companyUserMenuId > 0)
                return MenuPermissionProvider.HasPermission(action, companyUserMenuId);

            // Try session stored PK (set by attribute on page load)
            var httpContext = HttpContext.Current;
            if (httpContext?.Session != null)
            {
                object sessionVal = httpContext.Session["CompanyUserMenuId"];
                if (sessionVal != null)
                {
                    long storedId;
                    if (long.TryParse(sessionVal.ToString(), out storedId) && storedId > 0)
                        return MenuPermissionProvider.HasPermission(action, storedId);
                }
            }

            // Fallback: controller + action + companyId
            return MenuPermissionProvider.HasPermission(action, controllerName, actionName, companyId);
        }

        /// <summary>
        /// Check permission for a SPECIFIC controller + action.
        /// Useful for buttons linking to a different controller/action.
        /// </summary>
        public static bool HasMenuAction(this HtmlHelper html, MenuAction action,
            int companyId, string controllerName, string actionName)
        {
            return MenuPermissionProvider.HasPermission(action, controllerName, actionName, companyId);
        }

        /// <summary>
        /// Check by controller only (any submenu under this controller).
        /// </summary>
        public static bool HasMenuAction(this HtmlHelper html, MenuAction action,
            int companyId, string controllerName)
        {
            return MenuPermissionProvider.HasPermission(action, controllerName, companyId);
        }
    }
}
