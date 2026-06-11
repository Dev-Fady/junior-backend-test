using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace unior_backend_test.Domain.Interfaces
{
    public interface ICurrentUserService
    {
        public string UserId { get; }
        public string UserName { get; }
    }
}
