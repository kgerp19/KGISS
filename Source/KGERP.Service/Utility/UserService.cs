using KGERP.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Service.Utility
{
    public static class UserService
    {
        private static readonly ERPEntities _context = new ERPEntities();
        public static int InactiveResignUser(DateTime checkDay)
        {
            int totalCount = 0;
            var resignUserList = (from e in _context.Employees
                                  join u in _context.Users on e.EmployeeId equals u.UserName
                                  where !e.Active && e.EndDate.HasValue && e.EndDate <= checkDay
                                  && u.Active
                                  select u).ToList();
            if (resignUserList == null || resignUserList.Count() == 0)
            {
                return totalCount;
            }

            foreach (var user in resignUserList)
            {
                user.Active = false;
                user.IsEmailVerified = false;
               totalCount += _context.SaveChanges();
            }

            return totalCount;
        }
    }
}
