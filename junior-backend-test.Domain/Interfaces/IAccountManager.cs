using junior_backend_test.Domain.AccountsDtos;
using junior_backend_test.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace junior_backend_test.Domain.Interfaces
{
    public interface IAccountManager
    {
        Task<AuthUserResponseDto?> LoginAsync(LoginDto loginDto);
        Task<AuthUserResponseDto?> LoginAdminAsync(LoginDto loginDto);
        Task<string> RegisterAsync(RegisterDto registerDto);
        Task<string> RegisterAdminAsync(string email, string password);
        Task<bool> ConfirmEmailAsync(string email, string otp);
        Task<bool> RequestPasswordResetAsync(string email);
        Task<bool> ResetPasswordAsync(string email, string otp, string newPassword);
        Task<bool> ResendOtpAsync(string email);
        Task<bool> DeleteAsync(Guid Id);
        Task<bool> UpdateAsync(UpdateAccountDto updateAccountDto);
        Task<bool> UpdatePasswordAsync(UpdatePasswordDto updatePasswordDto);
        IQueryable<UserReadDto> GetAllUsers();
        Task<UserReadDto?> GetUserByIdAsync(Guid Id);

        Task<bool> CreateRole(CreateRoleDto createRoleDto);
        Task<bool> AssignRoleToUser(AssignRoleDto assignRoleDto);
        Task<bool> UpdateRole(UpdateRoleDto updateRoleDto);
        Task<bool> DeleteRole(string Id);
        Task<IEnumerable<RoleReadDto>> GetAllRoles();
        Task<string> GenerateTokenAsync(ApplicationUser user);
    }
}
