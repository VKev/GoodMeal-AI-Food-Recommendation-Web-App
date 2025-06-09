using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Abstractions.UnitOfWork;
using Application.Common.ResponseModel;
using Application.Guests.Commands;
using AutoMapper;
using Domain.Repositories;
using FluentAssertions;
using Moq;

namespace test.Guests.Commands
{
    public class CreateGuestCommandHandlerTests
    {
        private readonly Mock<IGuestRepository> _guestRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;

        public CreateGuestCommandHandlerTests()
        {
            _guestRepositoryMock = new();
            _mapperMock = new();
            _unitOfWorkMock = new();
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccessResult_When_UserNotExist()
        {
            var command = new CreateGuestCommand("test_user", "test_user_email");
            var handler = new CreateGuestCommandHandler(_guestRepositoryMock.Object, _mapperMock.Object, _unitOfWorkMock.Object);
            Result result = await handler.Handle(command,default);
            result.IsSuccess.Should().BeTrue();
        }
    }
}