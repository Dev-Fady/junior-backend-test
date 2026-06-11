using junior_backend_test.Domain.AccountsDtos;
using junior_backend_test.Domain.Interfaces;
using junior_backend_test.Domain.Model;
using junior_backend_test.Infrastructure.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using unior_backend_test.Domain.Interfaces;

namespace junior_backend_test.Infrastructure.Services
{
    public class AccountManager : IAccountManager
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly JuniorBackendTestContext _context;
        private readonly IEmailService _emailService;

        public AccountManager(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            IConfiguration configuration,
            JuniorBackendTestContext context,
            IEmailService emailService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _context = context;
            _emailService = emailService;
        }
        public async Task<string> RegisterAsync(RegisterDto registerDto)
        {
            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
                return "هذا البريد مستخدم بالفعل";

            var existingUserByPhone = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == registerDto.PhoneNumber);
            if (existingUserByPhone != null)
                return "رقم الهاتف هذا مستخدم بالفعل";

            ApplicationUser user = new ApplicationUser
            {
                Email = registerDto.Email,
                UserName = registerDto.Email,
                FullName = registerDto.UserName,
                Address = registerDto.Address,
                PhoneNumber = registerDto.PhoneNumber,
                CreateAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return $"خطأ في التسجيل: {errors}";
            }

            string defaultRole = "User";
            if (!await _roleManager.RoleExistsAsync(defaultRole))
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid>(defaultRole));
            }
            await _userManager.AddToRoleAsync(user, defaultRole);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName ?? user.UserName ?? "User")
            };

            await _userManager.AddClaimsAsync(user, claims);

            var otp = new Random().Next(100000, 999999).ToString();

            var oldOtps = _context.EmailOtps.Where(x=>x.Email == user.Email);
            _context.EmailOtps.RemoveRange(oldOtps);

            await _context.EmailOtps.AddAsync(new EmailOtp
            {
                Email = user.Email,
                Code = otp,
                ExpiryTime = DateTime.UtcNow.AddMinutes(10)
            });
            await _context.SaveChangesAsync();

            string emailBody = GetEmailTemplate("تأكيد البريد الإلكتروني", "أهلاً بك في تطبيق Test! لإكمال عملية التسجيل وتفعيل حسابك، يُرجى استخدام رمز التحقق التالي:", otp);
            await _emailService.SendEmailAsync(user.Email, "تطبيق Test - تأكيد البريد الإلكتروني", emailBody);

            return "تم التسجيل بنجاح، برجاء تأكيد البريد الإلكتروني";
        }

        public async Task<string> RegisterAdminAsync(string email, string password)
        {
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
                return "هذا البريد مستخدم بالفعل";

            ApplicationUser user = new ApplicationUser
            {
                Email = email,
                UserName = email,
                FullName = "Admin",
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return $"خطأ في التسجيل: {errors}";
            }

            string adminRole = "Admin";
            if (!await _roleManager.RoleExistsAsync(adminRole))
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid>(adminRole));
            }
            await _userManager.AddToRoleAsync(user, adminRole);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName ?? "Admin")
            };

            await _userManager.AddClaimsAsync(user, claims);

            return "تم تسجيل المشرف بنجاح وتفعيل الحساب";
        }
        public async Task<AuthUserResponseDto?> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

          
            if (user == null || string.IsNullOrEmpty(loginDto.Password))
                return null;

            if (!user.EmailConfirmed)
            {
                throw new Exception("يرجى تأكيد البريد الإلكتروني قبل تسجيل الدخول.");
            }

            var isValid = await _userManager.CheckPasswordAsync(user, loginDto.Password!);
            if (!isValid)
                return null;

            var claims = (await _userManager.GetClaimsAsync(user)).ToList();
            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var token = GenerateToken(claims);

            return new AuthUserResponseDto
            {
                UserId = user.Id.ToString(),
                UserName = user.FullName ?? user.UserName,
                Token = token,
                ExpiresAt = DateTime.Now.AddMonths(10)
            };
        }

        public async Task<AuthUserResponseDto?> LoginAdminAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user == null || string.IsNullOrEmpty(loginDto.Password))
                return null;

            var isValid = await _userManager.CheckPasswordAsync(user, loginDto.Password!);
            if (!isValid)
                return null;

            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains("Admin"))
            {
                 throw new UnauthorizedAccessException("غير مصرح لك بالدخول كمسئول.");
            }

            var claims = (await _userManager.GetClaimsAsync(user)).ToList();
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var token = GenerateToken(claims);

            return new AuthUserResponseDto
            {
                UserId = user.Id.ToString(),
                UserName = user.FullName ?? user.UserName,
                Token = token,
                ExpiresAt = DateTime.Now.AddMonths(10)
            };
        }
        public async Task<bool> ResendOtpAsync(string email)
        {
            var user =await _userManager.FindByEmailAsync(email);
            if (user == null || user.EmailConfirmed)
                return false;

            var otpOlds = _context.EmailOtps.Where(x => x.Email == email);
            _context.EmailOtps.RemoveRange(otpOlds);

            var otp  = new Random().Next(100000, 999999).ToString();

            await _context.EmailOtps.AddAsync(new EmailOtp
            {
                Email = email,
                Code = otp,
                ExpiryTime = DateTime.UtcNow.AddMinutes(10)
            });

            await _context.SaveChangesAsync();

            string emailBody = GetEmailTemplate("إعادة إرسال رمز التأكيد", "لقد طلبت إعادة إرسال رمز تأكيد البريد الإلكتروني لحسابك في تطبيق Test. يُرجى استخدام الرمز التالي:", otp);
            await _emailService.SendEmailAsync(email, "تطبيق Test - رمز تأكيد جديد", emailBody);

            return true;
        }
        public async Task<bool> ConfirmEmailAsync(string email, string otp)
        {
            var record = _context.EmailOtps.FirstOrDefault(x => x.Email == email && x.Code == otp);

            if (record == null || record.ExpiryTime < DateTime.UtcNow)
                return false;

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return false;
            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);
            _context.EmailOtps.Remove(record);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RequestPasswordResetAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return false;

            var otp = new Random().Next(100000, 999999).ToString();

            var oldOtps = _context.EmailOtps.Where(x => x.Email == email);
            _context.EmailOtps.RemoveRange(oldOtps);

            await _context.EmailOtps.AddAsync(new EmailOtp
            {
                Email = email,
                Code = otp,
                ExpiryTime = DateTime.UtcNow.AddMinutes(10)
            });

            await _context.SaveChangesAsync();

           string emailBody = GetEmailTemplate("إعادة تعيين كلمة المرور", "لقد تلقينا طلباً لإعادة تعيين كلمة المرور الخاصة بحسابك في تطبيق Test. يُرجى استخدام الرمز التالي لإكمال العملية:", otp);
           await _emailService.SendEmailAsync(email, "تطبيق Test - إعادة تعيين كلمة المرور", emailBody);

            return true;
        }
        public async Task<bool> ResetPasswordAsync(string email, string otp, string newPassword)
        {
            var record = _context.EmailOtps
                .FirstOrDefault(x => x.Email == email && x.Code == otp);
            if (record == null || record.ExpiryTime < DateTime.UtcNow)
                return false;
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return false;
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            if (result.Succeeded)
            {
                _context.EmailOtps.Remove(record);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public async Task<bool> DeleteRole(string Id)
        {
            if (!Guid.TryParse(Id, out Guid roleId))
                return false;

            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role == null)
                return false;

            var result = await _roleManager.DeleteAsync(role);
            return result.Succeeded;
        }
        public IQueryable<UserReadDto> GetAllUsers()
        {
            return _userManager.Users
                .Select(a => new UserReadDto
                {
                    Id = a.Id.ToString(),
                    Name = a.FullName ?? a.UserName,
                    Email = a.Email,
                    PhoneNumber = a.PhoneNumber,
                    Address = a.Address,
                    IsArchived = a.IsArchived ?? false,
                    ArchiveReason = a.ArchiveReason,
                });
        }

        public async Task<UserReadDto?> GetUserByIdAsync(Guid Id)
        {
            var user = await _userManager.FindByIdAsync(Id.ToString());
            if (user == null)
                return null;

            return new UserReadDto
            {
                Id = user.Id.ToString(),
                Name = user.FullName ?? user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                IsArchived = user.IsArchived ?? false,
                ArchiveReason = user.ArchiveReason,
            };
        }

        public async Task<bool> UpdateAsync(UpdateAccountDto updateAccountDto)
        {
            var user = await _userManager.FindByIdAsync(updateAccountDto.Id);

            if (user == null)
                return false;

            var existingUserByPhone = await _userManager.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == updateAccountDto.PhoneNumber && u.Id.ToString() != updateAccountDto.Id);
            if (existingUserByPhone != null)
                return false;

            user.Email = updateAccountDto.Email;
            user.UserName = updateAccountDto.Email;
            user.FullName = updateAccountDto.UserName;
            user.Address = updateAccountDto.Address;
            user.PhoneNumber = updateAccountDto.PhoneNumber;
            
            var isUpdated = await _userManager.UpdateAsync(user);

            if (isUpdated.Succeeded)
                return true;

            return false;
        }

        public async Task<bool> UpdatePasswordAsync(UpdatePasswordDto updatePasswordDto)
        {
            var user = await _userManager.FindByIdAsync(updatePasswordDto.Id);
            if (user == null)
                return false;

            var result = await _userManager.ChangePasswordAsync(user, updatePasswordDto.OldPassword, updatePasswordDto.NewPassword);
            return result.Succeeded;
        }

        public Task<bool> AssignRoleToUser(AssignRoleDto assignRoleDto)
        {
            // مُنفَّذ عبر AssignRoleToUserCommand في RolesController
            return Task.FromResult(true);
        }

        public Task<bool> CreateRole(CreateRoleDto createRoleDto)
        {
            // مُنفَّذ عبر CreateRoleCommand في RolesController
            return Task.FromResult(true);
        }

        public async Task<bool> DeleteAsync(Guid Id)
        {
            var user = await _userManager.FindByIdAsync(Id.ToString());
            var isDeleted = await _userManager.DeleteAsync(user);

            if (isDeleted.Succeeded)
                return true;

            return false;
        }

        
        public Task<IEnumerable<RoleReadDto>> GetAllRoles()
        {
            // مُنفَّذ عبر GetAllRolesQuery في RolesController
            return Task.FromResult<IEnumerable<RoleReadDto>>(new List<RoleReadDto>());
        }

        
        public Task<bool> UpdateRole(UpdateRoleDto updateRoleDto)
        {
            // مُنفَّذ عبر UpdateRoleCommand في RolesController
            return Task.FromResult(true);
        }

        public async Task<string> GenerateTokenAsync(ApplicationUser user)
        {
            var claims = (await _userManager.GetClaimsAsync(user)).ToList();
            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            return GenerateToken(claims);
        }

        private string GenerateToken(List<Claim> claims)
        {
            //Security Key , HasingAlgorithm
            string secutirykey = _configuration.GetSection("JWT:SecurityKey").Value!;

            var securityKeyByte = Encoding.ASCII.GetBytes(secutirykey!);

            SecurityKey securityKey = new SymmetricSecurityKey(securityKeyByte);

            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMonths(10),
                signingCredentials: signingCredentials
                );

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.WriteToken(jwtSecurityToken);
            return token;
        }

        private string GetEmailTemplate(string title, string message, string otp)
        {
            return $@"
    <!DOCTYPE html>
    <html lang='ar' dir='rtl'>
    <head>
        <meta charset='UTF-8'>
        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
        <title>{title}</title>
        <style>
            body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f4f7fa; margin: 0; padding: 0; }}
            .container {{ max-width: 600px; margin: 40px auto; background-color: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 15px rgba(0,0,0,0.1); }}
            .header {{ background-color: #2c3e50; color: #ffffff; padding: 20px; text-align: center; }}
            .header h1 {{ margin: 0; font-size: 24px; }}
            .content {{ padding: 30px; text-align: center; color: #333333; }}
            .content p {{ font-size: 16px; line-height: 1.6; margin-bottom: 20px; }}
            .otp-box {{ background-color: #f8f9fa; border: 2px dashed #3498db; color: #2c3e50; font-size: 32px; font-weight: bold; padding: 15px 30px; margin: 20px auto; display: inline-block; border-radius: 5px; letter-spacing: 5px; }}
            .footer {{ background-color: #f1f1f1; color: #777777; text-align: center; padding: 15px; font-size: 14px; border-top: 1px solid #eeeeee; }}
            .footer p {{ margin: 5px 0; }}
        </style>
    </head>
    <body>
        <div class='container'>
            <div class='header'>
                <h1>تطبيق Test</h1>
            </div>
            <div class='content'>
                <h2>{title}</h2>
                <p>{message}</p>
                <div class='otp-box'>{otp}</div>
                <p style='color: #e74c3c; font-weight: bold;'>هذا الرمز صالح لمدة 10 دقائق فقط. يُرجى عدم مشاركته مع أي شخص.</p>
            </div>
            <div class='footer'>
                <p>تم إرسال هذه الرسالة من تطبيق Test. إذا لم تقم بطلب هذا الرمز، يُرجى تجاهل هذه الرسالة.</p>
                <p>&copy; {DateTime.UtcNow.Year} تطبيق Test. جميع الحقوق محفوظة.</p>
            </div>
        </div>
    </body>
    </html>";
        }
    }
}
