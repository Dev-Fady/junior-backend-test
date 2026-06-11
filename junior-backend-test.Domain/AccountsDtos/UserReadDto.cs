using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace junior_backend_test.Domain.AccountsDtos
{
    public class UserReadDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public bool IsArchived { get; set; }
        public string? ArchiveReason { get; set; }
    }
}
