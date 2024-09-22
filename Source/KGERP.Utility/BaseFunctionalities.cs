using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace KGERP.Utility
{
    public static class BaseFunctionalities
    {
        public static string GetEnumDescription(Enum en)
        {
            Type type = en.GetType();
            MemberInfo[] memInfo = type.GetMember(en.ToString());

            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attrs != null && attrs.Length > 0)
                    return ((DescriptionAttribute)attrs[0]).Description;
            }
            return en.ToString();
        }

        public static List<object> GetEnumList<T>()
        {
            var list = new List<object>();

            foreach (var item in Enum.GetValues(typeof(T)))
            {
                list.Add(new { Value = (int)item, Text = GetEnumDescription((Enum)Enum.Parse(typeof(T), item.ToString())) });
            }

            return list;
        }

        internal static string GetEnumDescription(string originType)
        {
            throw new NotImplementedException();
        }
    }
}
