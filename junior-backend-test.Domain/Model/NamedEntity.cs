using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace junior_backend_test.Domain.Model
{
    public abstract class NamedEntity : BaseEntity
    {
        public string NameAr { get; set; } = null!;
        public string NameEn { get; set; } = null!;
        public string DescriptionAr { get; set; } = null!;
        public string DescriptionEn { get; set; } = null!;
    }
}
