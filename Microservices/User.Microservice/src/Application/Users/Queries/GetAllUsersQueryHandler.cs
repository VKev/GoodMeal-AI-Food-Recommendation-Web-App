using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;
using Application.Common.ResponseModel;
using AutoMapper;
using Domain.Repositories;
using MediatR;

namespace Application.Users.Queries
{
    public sealed record GetAllUsersQuery : IQuery<IEnumerable<GetUserResponse>>;
    internal sealed class GetAllUsersQueryHandler : IQueryHandler<GetAllUsersQuery, IEnumerable<GetUserResponse>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetAllUsersQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }


        public async Task<Result<IEnumerable<GetUserResponse>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetAllAsync(cancellationToken);
            var userResponses = _mapper.Map<IEnumerable<GetUserResponse>>(users);
            return Result.Success(userResponses);
        }
    }
}