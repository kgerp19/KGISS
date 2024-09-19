//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;

//namespace KGERP.Utility
//{
//    public static class SortHelper
//    {
//        public static string ToStringNullAbleDate(this DateTime? dt, string format)
//        {
//           return dt == null ? "" : (dt)?.ToString(format);
//        }
            
//        public static List<T> OrderBy<T>(this List<T> source, string property)
//        {
//            return ApplyOrder<T>(source, property, "OrderBy");
//        }
//        public static List<T> OrderByDescending<T>(this List<T> source, string property)
//        {
//            return ApplyOrder<T>(source, property, "OrderByDescending");
//        }
//        public static List<T> ThenBy<T>(this List<T> source, string property)
//        {
//            return ApplyOrder<T>(source, property, "ThenBy");
//        }
//        public static List<T> ThenByDescending<T>(this List<T> source, string property)
//        {
//            return ApplyOrder<T>(source, property, "ThenByDescending");
//        }
//        static List<T> ApplyOrder<T>(List<T> source, string property, string methodName)
//        {
//            string[] props = property.Split('.');
//            Type type = typeof(T);
//            ParameterExpression arg = Expression.Parameter(type, "x");
//            Expression expr = arg;
//            foreach (string prop in props)
//            {
//                // use reflection (not ComponentModel) to mirror LINQ
//                PropertyInfo pi = type.GetProperty(prop);
//                expr = Expression.Property(expr, pi);
//                type = pi.PropertyType;
//            }
//            Type delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
//            LambdaExpression lambda = Expression.Lambda(delegateType, expr, arg);

//            object result = typeof(Queryable).GetMethods().Single(
//                    method => method.Name == methodName
//                            && method.IsGenericMethodDefinition
//                            && method.GetGenericArguments().Length == 2
//                            && method.GetParameters().Length == 2)
//                    .MakeGenericMethod(typeof(T), type)
//                    .Invoke(null, new object[] { source, lambda });
//            return (List<T>)result;
//        }
//    }
//}
