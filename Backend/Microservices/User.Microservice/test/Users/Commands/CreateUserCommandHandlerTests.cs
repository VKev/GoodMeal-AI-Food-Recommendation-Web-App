using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions.UnitOfWork;
using Application.Common.ResponseModel;
using Application.Users.Commands;
using AutoMapper;
using Domain.Repositories;
using FluentAssertions;
using Moq;

namespace test.Users.Commands
{
    public class CreateUserCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;

        public CreateUserCommandHandlerTests()
        {
            _userRepositoryMock = new();
            _mapperMock = new();
            _unitOfWorkMock = new();
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccessResult_When_UserNotExist()
        {
            var command = new CreateUserCommand("test_user", "test_user_email");
            var handler = new CreateUserCommandHandler(_userRepositoryMock.Object, _mapperMock.Object, _unitOfWorkMock.Object);
            Result result = await handler.Handle(command,default);
            result.IsSuccess.Should().BeTrue();
        }
    }
}