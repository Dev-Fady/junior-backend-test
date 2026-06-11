using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace junior_backend_test.Domain.AccountsDtos
{
    public class AuthUserResponseDto
    {
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string? Token { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
