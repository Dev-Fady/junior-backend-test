using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace junior_backend_test.Application.Extentions.Pagination
{
    public class PaginatedList<T>
    {
        public IEnumerable<T> Items { get; }
        public int PageNumber { get; }
        public int TotalPages { get; }
        public int TotalCount { get; }

        public PaginatedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;//1
            TotalPages = count == 0 ? 1 : (int)Math.Ceiling(count / (double)pageSize);// 1000 / 100 = 10
            TotalCount = count; //1000
            Items = items;
        }

        public bool HasPreviousPage => PageNumber > 1;

        public bool HasNextPage => PageNumber < TotalPages;
    }
}
