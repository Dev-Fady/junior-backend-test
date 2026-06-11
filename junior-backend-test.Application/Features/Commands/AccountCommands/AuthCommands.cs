using junior_backend_test.Application.Wapper;
using junior_backend_test.Domain.AccountsDtos;
using junior_backend_test.Domain.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace junior_backend_test.Application.Features.Commands.AccountCommands
{
    public class RegisterCommand : IRequest<Response<string>>
    {
        public RegisterDto RegisterDto { get; set; } = null!;
    }

    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Response<string>>
    {
        private readonly IAccountManager _accountManager;
        public RegisterCommandHandler(IAccountManager accountManager) => _accountManager = accountManager;

        public async Task<Response<string>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var result = await _accountManager.RegisterAsync(request.RegisterDto);
            return Response<string>.Success(null, result);
        }
    }

    public class RegisterAdminCommand : IRequest<Response<string>>
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class RegisterAdminCommandHandler : IRequestHandler<RegisterAdminCommand, Response<string>>
    {
        private readonly IAccountManager _accountManager;
        public RegisterAdminCommandHandler(IAccountManager accountManager) => _accountManager = accountManager;

        public async Task<Response<string>> Handle(RegisterAdminCommand request, CancellationToken cancellationToken)
        {
            var result = await _accountManager.RegisterAdminAsync(request.Email, request.Password);
            return Response<string>.Success(null, result);
        }
    }

    public class LoginCommand : IRequest<Response<AuthUserResponseDto>>
    {
        public LoginDto LoginDto { get; set; } = null!;
    }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, Response<AuthUserResponseDto>>
    {
        private readonly IAccountManager _accountManager;
        public LoginCommandHandler(IAccountManager accountManager) => _accountManager = accountManager;

        public async Task<Response<AuthUserResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var result = await _accountManager.LoginAsync(request.LoginDto);
            if (result == null) return Response<AuthUserResponseDto>.Fail("فشل تسجيل الدخول");
            return Response<AuthUserResponseDto>.Success(result, "تم تسجيل الدخول بنجاح");
        }
    }

    public class LoginAdminCommand : IRequest<Response<AuthUserResponseDto>>
    {
        public LoginDto LoginDto { get; set; } = null!;
    }

    public class LoginAdminCommandHandler : IRequestHandler<LoginAdminCommand, Response<AuthUserResponseDto>>
    {
        private readonly IAccountManager _accountManager;
        public LoginAdminCommandHandler(IAccountManager accountManager) => _accountManager = accountManager;

        public async Task<Response<AuthUserResponseDto>> Handle(LoginAdminCommand request, CancellationToken cancellationToken)
        {
            var result = await _accountManager.LoginAdminAsync(request.LoginDto);
            if (result == null) return Response<AuthUserResponseDto>.Fail("فشل تسجيل الدخول");
            return Response<AuthUserResponseDto>.Success(result, "تم تسجيل الدخول بنجاح");
        }
    }

    public class ConfirmEmailCommand : IRequest<Response<bool>>
    {
        public string Email { get; set; } = null!;
        public string Otp { get; set; } = null!;
    }

    public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, Response<bool>>
    {
        private readonly IAccountManager _accountManager;
        public ConfirmEmailCommandHandler(IAccountManager accountManager) => _accountManager = accountManager;

        public async Task<Response<bool>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            var result = await _accountManager.ConfirmEmailAsync(request.Email, request.Otp);
            return result ? Response<bool>.Success(true, "تم التأكيد بنجاح") : Response<bool>.Fail("فشل التأكيد");
        }
    }

    public class ResendOtpCommand : IRequest<Response<bool>>
    {
        public string Email { get; set; } = null!;
    }

    public class ResendOtpCommandHandler : IRequestHandler<ResendOtpCommand, Response<bool>>
    {
        private readonly IAccountManager _accountManager;
        public ResendOtpCommandHandler(IAccountManager accountManager) => _accountManager = accountManager;

        public async Task<Response<bool>> Handle(ResendOtpCommand request, CancellationToken cancellationToken)
        {
            var result = await _accountManager.ResendOtpAsync(request.Email);
            return result ? Response<bool>.Success(true, "تم إعادة الإرسال بنجاح") : Response<bool>.Fail("فشل إعادة الإرسال");
        }
    }

    public class RequestPasswordResetCommand : IRequest<Response<bool>>
    {
        public string Email { get; set; } = null!;
    }

    public class RequestPasswordResetCommandHandler : IRequestHandler<RequestPasswordResetCommand, Response<bool>>
    {
        private readonly IAccountManager _accountManager;
        public RequestPasswordResetCommandHandler(IAccountManager accountManager) => _accountManager = accountManager;

        public async Task<Response<bool>> Handle(RequestPasswordResetCommand request, CancellationToken cancellationToken)
        {
            var result = await _accountManager.RequestPasswordResetAsync(request.Email);
            return result ? Response<bool>.Success(true, "تم إرسال رمز استعادة كلمة المرور بنجاح") : Response<bool>.Fail("فشل الإرسال");
        }
    }

    public class ResetPasswordCommand : IRequest<Response<bool>>
    {
        public string Email { get; set; } = null!;
        public string Otp { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }

    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Response<bool>>
    {
        private readonly IAccountManager _accountManager;
        public ResetPasswordCommandHandler(IAccountManager accountManager) => _accountManager = accountManager;

        public async Task<Response<bool>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var result = await _accountManager.ResetPasswordAsync(request.Email, request.Otp, request.NewPassword);
            return result ? Response<bool>.Success(true, "تمت إعادة تعيين كلمة المرور بنجاح") : Response<bool>.Fail("فشل إعادة تعيين كلمة المرور");
        }
    }

    public class UpdateAccountCommand : IRequest<Response<bool>>
    {
        public UpdateAccountDto UpdateAccountDto { get; set; } = null!;
    }

    public class UpdateAccountCommandHandler : IRequestHandler<UpdateAccountCommand, Response<bool>>
    {
        private readonly IAccountManager _accountManager;
        public UpdateAccountCommandHandler(IAccountManager accountManager) => _accountManager = accountManager;

        public async Task<Response<bool>> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
        {
            var result = await _accountManager.UpdateAsync(request.UpdateAccountDto);
            return result ? Response<bool>.Success(true, "تم التحديث بنجاح") : Response<bool>.Fail("فشل التحديث");
        }
    }

    public class UpdatePasswordCommand : IRequest<Response<bool>>
    {
        public UpdatePasswordDto UpdatePasswordDto { get; set; } = null!;
    }

    public class UpdatePasswordCommandHandler : IRequestHandler<UpdatePasswordCommand, Response<bool>>
    {
        private readonly IAccountManager _accountManager;
        public UpdatePasswordCommandHandler(IAccountManager accountManager) => _accountManager = accountManager;

        public async Task<Response<bool>> Handle(UpdatePasswordCommand request, CancellationToken cancellationToken)
        {
            var result = await _accountManager.UpdatePasswordAsync(request.UpdatePasswordDto);
            return result ? Response<bool>.Success(true, "تم التحديث بنجاح") : Response<bool>.Fail("فشل التحديث");
        }
    }
}
