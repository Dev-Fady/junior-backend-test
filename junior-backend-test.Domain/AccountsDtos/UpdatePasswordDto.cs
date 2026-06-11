using System;

namespace junior_backend_test.Domain.AccountsDtos
{
    public class UpdatePasswordDto
    {
        public string Id { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
