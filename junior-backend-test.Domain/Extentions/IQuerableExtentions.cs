using junior_backend_test.Domain.Model;
using System.Linq.Expressions;

namespace junior_backend_test.Domain.Extentions
{
    public static class IQuerableExtentions
    {
        public static IQueryable<T> IF<T>(this IQueryable<T> source, bool condition, Expression<Func<T, bool>> predicate)
            where T : class
        {
            if (condition)
                return source.Where(predicate);
            return source;
        }

        public static IQueryable<T> IF<T>(this IQueryable<T> source, bool condition, Expression<Func<T, bool>> predicate, Expression<Func<T, bool>> elsePredicate)
            where T : class
        {
            if (condition)
                return source.Where(predicate);
            return source.Where(elsePredicate);
        }

    public static IQueryable<T> FilterText<T>(this IQueryable<T> source, string? TextSeach)
       where T : NamedEntity
    {
            if (string.IsNullOrEmpty(TextSeach))
            {
                return source;
            }
            return source.Where(a =>
                                         a.NameAr.ToLower().Contains(TextSeach.ToLower()) ||
                                         a.NameEn.ToLower().Contains(TextSeach.ToLower()) ||
                                         a.DescriptionAr.ToLower().Contains(TextSeach.ToLower()) ||
                                         a.DescriptionEn.ToLower().Contains(TextSeach.ToLower())
            );
     }
        public static IQueryable<T> OrderGroupBy<T>(this IQueryable<T> source,
            List<(bool condition, Expression<Func<T, object>>)> predicate,
            bool IsDesc = false)
        {
            foreach (var item in predicate)
            {
                if (item.condition)
                {
                    if (IsDesc)
                        source = source.OrderByDescending(item.Item2);
                    else
                        source = source.OrderBy(item.Item2);
                }
            }
            return source;
        }

    }
}
