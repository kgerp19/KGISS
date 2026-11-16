using KGERP.Data.Models;
using KGERP.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KGERP.Helpers
{
    /// <summary>
    /// Loads all CompanyUserMenu rows for the current user in ONE query,
    /// caches the result in HttpContext.Items for the rest of the request.
    /// Both the attribute and the HtmlHelper use this — no extra DB calls.
    /// </summary>
    internal static class MenuPermissionProvider
    {
        private const string CacheKey = "__MenuPermissions__";
        private const string MapKey = "MenuPermissionMap";

        /// <summary>
        /// Returns the cached permission list for the current user.
        /// Loads from DB on first call per HTTP request.
        /// </summary>
        internal static List<UserMenuPermissionDto> GetPermissions()
        {
            var httpContext = HttpContext.Current;
            if (httpContext == null) return new List<UserMenuPermissionDto>();

            // Return cached list if already loaded this request
            if (httpContext.Items[CacheKey] is List<UserMenuPermissionDto> cached)
            {
                return cached;
            }

            // Get current user identity
            string userId = httpContext.User?.Identity?.Name;
            if (string.IsNullOrEmpty(userId))
            {
                return new List<UserMenuPermissionDto>();
            }

            // Load ALL active permissions for this user in one query
            List<UserMenuPermissionDto> permissions;
            using (var db = new ERPEntities())
            {
                permissions = (from um in db.CompanyUserMenus
                               join sm in db.CompanySubMenus
                                   on um.CompanySubMenuId equals sm.CompanySubMenuId
                               where um.UserId == userId
                                     && um.IsActive
                               select new UserMenuPermissionDto
                               {
                                   CompanyUserMenuId = um.CompanyUserMenuId,
                                   CompanyId = um.CompanyId,
                                   CompanySubMenuId = um.CompanySubMenuId,
                                   Controller = sm.Controller,
                                   Action = sm.Action,
                                   IsView = um.IsView,
                                   IsAdd = um.IsAdd,
                                   IsUpdate = um.IsUpdate,
                                   IsDelete = um.IsDelete
                               }).ToList();
            }

            // Cache for the rest of this HTTP request
            httpContext.Items[CacheKey] = permissions;
            return permissions;
        }

        /// <summary>
        /// Resolves CompanyUserMenuId from the session permission map.
        /// If the map is missing (session recycled), auto-rebuilds it from DB.
        /// This keeps CompanyUserMenuId hidden from the URL.
        /// </summary>
        internal static long ResolveCompanyUserMenuId(string controllerName,
            string actionName, int companyId)
        {
            var httpContext = HttpContext.Current;
            if (httpContext?.Session == null) return 0;
            if (string.IsNullOrEmpty(controllerName)) return 0;

            // Ensure the map exists — rebuild from DB if session was lost
            EnsurePermissionMap();

            var map = httpContext.Session[MapKey] as Dictionary<string, long>;
            if (map == null) return 0;

            // Exact key: Controller|Action|CompanyId
            string key = controllerName + "|" + (actionName ?? "") + "|" + companyId;
            long id;
            if (map.TryGetValue(key, out id))
                return id;

            return 0;
        }

        /// <summary>
        /// If the session permission map is missing, rebuilds it from DB.
        /// This handles session recycle / app pool restart gracefully.
        /// </summary>
        private static void EnsurePermissionMap()
        {
            var httpContext = HttpContext.Current;
            if (httpContext?.Session == null) return;

            // Already exists — nothing to do
            if (httpContext.Session[MapKey] != null) return;

            // Rebuild from the DB-loaded permissions
            var permissions = GetPermissions();
            if (permissions.Count == 0) return;

            var map = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase);
            foreach (var p in permissions)
            {
                string key = (p.Controller ?? "") + "|" + (p.Action ?? "") + "|" + (p.CompanyId ?? 0);
                if (!map.ContainsKey(key))
                {
                    map[key] = p.CompanyUserMenuId;
                }
            }
            httpContext.Session[MapKey] = map;
        }

        /// <summary>
        /// PRIMARY overload: checks permission by CompanyUserMenuId (exact PK match).
        /// No ambiguity between submenus that share the same controller.
        /// </summary>
        internal static bool HasPermission(MenuAction action, long companyUserMenuId)
        {
            if (companyUserMenuId <= 0) return false;

            var permissions = GetPermissions();

            var row = permissions.FirstOrDefault(p => p.CompanyUserMenuId == companyUserMenuId);
            if (row == null) return false;

            switch (action)
            {
                case MenuAction.View: return row.IsView;
                case MenuAction.Add: return row.IsAdd;
                case MenuAction.Edit: return row.IsUpdate;
                case MenuAction.Delete: return row.IsDelete;
                default: return false;
            }
        }

        /// <summary>
        /// Fallback overload: checks by Controller + Action + CompanyId.
        /// Used when companyUserMenuId is not available.
        /// </summary>
        internal static bool HasPermission(MenuAction action, string controllerName,
            string mvcActionName, int companyId)
        {
            if (string.IsNullOrEmpty(controllerName) || companyId <= 0)
                return false;

            var permissions = GetPermissions();

            var matchingRows = permissions.Where(p =>
                p.CompanyId == companyId &&
                string.Equals(p.Controller, controllerName, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(p.Action, mvcActionName, StringComparison.OrdinalIgnoreCase));

            foreach (var row in matchingRows)
            {
                bool allowed = false;
                switch (action)
                {
                    case MenuAction.View: allowed = row.IsView; break;
                    case MenuAction.Add: allowed = row.IsAdd; break;
                    case MenuAction.Edit: allowed = row.IsUpdate; break;
                    case MenuAction.Delete: allowed = row.IsDelete; break;
                }
                if (allowed) return true;
            }

            // If no exact Action match, try Controller only (null action rows)
            if (!string.IsNullOrEmpty(mvcActionName))
            {
                var controllerOnlyRows = permissions.Where(p =>
                    p.CompanyId == companyId &&
                    string.Equals(p.Controller, controllerName, StringComparison.OrdinalIgnoreCase) &&
                    string.IsNullOrEmpty(p.Action));

                foreach (var row in controllerOnlyRows)
                {
                    bool allowed = false;
                    switch (action)
                    {
                        case MenuAction.View: allowed = row.IsView; break;
                        case MenuAction.Add: allowed = row.IsAdd; break;
                        case MenuAction.Edit: allowed = row.IsUpdate; break;
                        case MenuAction.Delete: allowed = row.IsDelete; break;
                    }
                    if (allowed) return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Backward-compatible overload (controller only, any submenu).
        /// </summary>
        internal static bool HasPermission(MenuAction action, string controllerName, int companyId)
        {
            if (string.IsNullOrEmpty(controllerName) || companyId <= 0)
                return false;

            var permissions = GetPermissions();

            var matchingRows = permissions.Where(p =>
                p.CompanyId == companyId &&
                string.Equals(p.Controller, controllerName, StringComparison.OrdinalIgnoreCase));

            foreach (var row in matchingRows)
            {
                bool allowed = false;
                switch (action)
                {
                    case MenuAction.View: allowed = row.IsView; break;
                    case MenuAction.Add: allowed = row.IsAdd; break;
                    case MenuAction.Edit: allowed = row.IsUpdate; break;
                    case MenuAction.Delete: allowed = row.IsDelete; break;
                }
                if (allowed) return true;
            }

            return false;
        }
    }

    /// <summary>
    /// Lightweight DTO to hold one permission row in memory.
    /// </summary>
    internal class UserMenuPermissionDto
    {
        public long CompanyUserMenuId { get; set; }
        public int? CompanyId { get; set; }
        public int CompanySubMenuId { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public bool IsView { get; set; }
        public bool IsAdd { get; set; }
        public bool IsUpdate { get; set; }
        public bool IsDelete { get; set; }
    }
}
