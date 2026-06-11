using junior_backend_test.Application.Features.Commands.AccountCommands;
using junior_backend_test.Application.Features.Queries.AccountQueries;
using junior_backend_test.Domain.AccountsDtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace junior_backend_test.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AccountsController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterDto dto)
            => Ok(await _mediator.Send(new RegisterCommand { RegisterDto = dto }));

        [HttpPost]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailCommand command)
            => Ok(await _mediator.Send(command));

        [HttpPost]
        public async Task<IActionResult> RequestPasswordReset([FromBody] RequestPasswordResetCommand command)
            => Ok(await _mediator.Send(command));

        [HttpPost]
        public async Task<IActionResult> ResendOtp([FromBody] ResendOtpCommand command)
            => Ok(await _mediator.Send(command));

        [HttpPost]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
            => Ok(await _mediator.Send(command));

        [HttpPost]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDto dto)
            => Ok(await _mediator.Send(new LoginCommand { LoginDto = dto }));

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateAccount([FromBody] UpdateAccountDto dto)
            => Ok(await _mediator.Send(new UpdateAccountCommand { UpdateAccountDto = dto }));

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordDto dto)
            => Ok(await _mediator.Send(new UpdatePasswordCommand { UpdatePasswordDto = dto }));

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers([FromQuery] GetAllUsersQuery query)
            => Ok(await _mediator.Send(query));

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetUserById(Guid id)
            => Ok(await _mediator.Send(new GetUserByIdQuery(id)));
    }
}
