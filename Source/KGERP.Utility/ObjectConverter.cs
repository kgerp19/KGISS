using AutoMapper;
using System.Collections.Generic;

namespace KGERP.Utility
{
    public static class ObjectConverter<TSource, TDestination>
    {
        public static TDestination Convert(TSource source)
        {
            return Mapper.Map<TSource, TDestination>(source);
        }

        public static IList<TDestination> ConvertList(ICollection<TSource> sources)
        {
            return Mapper.Map<ICollection<TSource>, IList<TDestination>>(sources);
        }
    }
}
