using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace junior_backend_test.Domain.Model
{
    public abstract class BaseEntity
    {
        public Guid ID { get; set; }

        public string? CreatedById { get; set; }
        public string? CreatedByName { get; set; }
        public DateTime? CreatedDateTime { get; set; }

        public string? ModifiedById { get; set; }
        public string? ModifiedByName { get; set; }
        public DateTime? ModifiedDateTime { get; set; }

        public string? ArchivedById { get; set; }
        public string? ArchivedByName { get; set; }
        public DateTime? ArchivedDateTime { get; set; }

        public bool IsArchived { get; set; } = false;
    }
}
