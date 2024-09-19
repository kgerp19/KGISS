using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KGERP.Utility.Extensions
{
    public static class IntExtensions
    {
        public static bool IsNullOrZero(this int? value)
        {
            return value == null || value == 0;
        }
    }
}
