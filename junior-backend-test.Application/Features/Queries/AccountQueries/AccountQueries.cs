using junior_backend_test.Application.Extentions.Pagination;
using junior_backend_test.Application.Wapper;
using junior_backend_test.Domain.AccountsDtos;
using junior_backend_test.Domain.Interfaces;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace junior_backend_test.Application.Features.Queries.AccountQueries
{
    public class GetAllUsersQuery : PaginateBaseParamter, IRequest<Response<PaginatedList<UserReadDto>>>
    {
        public GetAllUsersQuery() : base(1, 10) { }
    }

    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, Response<PaginatedList<UserReadDto>>>
    {
        private readonly IAccountManager _accountManager;
        public GetAllUsersQueryHandler(IAccountManager accountManager) => _accountManager = accountManager;

        public async Task<Response<PaginatedList<UserReadDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var query = _accountManager.GetAllUsers();
            var paginatedData = await query.PaginateAsync(request.PageNumber, request.PageSize, cancellationToken);
            return Response<PaginatedList<UserReadDto>>.Success(paginatedData);
        }
    }

    public class GetUserByIdQuery : IRequest<Response<UserReadDto>>
    {
        public Guid Id { get; set; }
        public GetUserByIdQuery(Guid id) => Id = id;
    }

    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Response<UserReadDto>>
    {
        private readonly IAccountManager _accountManager;
        public GetUserByIdQueryHandler(IAccountManager accountManager) => _accountManager = accountManager;

        public async Task<Response<UserReadDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _accountManager.GetUserByIdAsync(request.Id);
            return user != null ? Response<UserReadDto>.Success(user) : Response<UserReadDto>.Fail("المستخدم غير موجود");
        }
    }
}
