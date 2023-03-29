using Moq;
using RL.Backend.Commands.Handlers.Plans;
using RL.Backend.Commands;
using RL.Backend.Exceptions;
using RL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using RL.Data.DataModels;
using System.Numerics;
using MediatR;

namespace RL.Backend.UnitTests
{
    [TestClass]
    public class DeleteuserFromProcedureTests
    {
        [TestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        [DataRow(int.MinValue)]
        public async Task DeleteuserFromProcedureTests_Tests_UserId_ReturnsBadRequest(int userId)
        {
            //Given
            var context = new Mock<RLContext>();
            var sut = new AddUserToProcedureCommandHandler(context.Object);
            var request = new AddUserToProcedureCommand()
            {
                ProcedureId = 1,
                UserId = userId
                
            };
            //When
            var result = await sut.Handle(request, new CancellationToken());

            //Then
            result.Exception.Should().BeOfType(typeof(BadRequestException));
            result.Succeeded.Should().BeFalse();
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        [DataRow(int.MinValue)]
        public async Task DeleteuserFromProcedureTests_Tests_ProcedureId_ReturnsBadRequest(int procedureId)
        {
            //Given
            var context = new Mock<RLContext>();
            var sut = new DeleteUserFromProcedureCommandHandler(context.Object);
            var request = new DeleteUserFromProcedureCommand()
            {
                ProcedureId = procedureId,
                UserId = 1
            };
            //When
            var result = await sut.Handle(request, new CancellationToken());

            //Then
            result.Exception.Should().BeOfType(typeof(BadRequestException));
            result.Succeeded.Should().BeFalse();
        }

        [TestMethod]
        [DataRow(1, 1)]
        [DataRow(19, 1010)]
        [DataRow(35, 69)]
        public async Task AddUserToProcedure_Tests_DidNotFoundProcedureUser_ReturnsSuccess(int userId, int procedureId)
        {
            //Given
            var context = DbContextHelper.CreateContext();
            var sut = new DeleteUserFromProcedureCommandHandler(context);
            var request = new DeleteUserFromProcedureCommand()
            {
                ProcedureId = procedureId,
                UserId = userId
            };
            await context.SaveChangesAsync();

            //When
            var result = await sut.Handle(request, new CancellationToken());

            //Then
            result.Value.Should().BeOfType(typeof(Unit));
            result.Succeeded.Should().BeTrue();
        }

        [TestMethod]
        [DataRow(1, 1)]
        [DataRow(19, 1010)]
        [DataRow(35, 69)]
        public async Task AddUserToProcedure_Tests_FoundProcedureUser_ReturnsSuccess(int userId, int procedureId)
        {
            //Given
            var context = DbContextHelper.CreateContext();
            var sut = new DeleteUserFromProcedureCommandHandler(context);
            var request = new DeleteUserFromProcedureCommand()
            {
                ProcedureId = procedureId,
                UserId = userId
            };

            context.Users.Add(new User
            {
                UserId = userId,
                Name = "Marek Michna"
            });
            context.Procedures.Add(new Procedure
            {
                ProcedureId = procedureId,
                ProcedureTitle = "Test Procedure"
            });
            context.ProcedureUsers.Add(new ProcedureUser
            {
                ProcedureId = procedureId,
                UserId = userId
            });
            await context.SaveChangesAsync();

            //When
            var result = await sut.Handle(request, new CancellationToken());

            //Then
            result.Value.Should().BeOfType(typeof(Unit));
            result.Succeeded.Should().BeTrue();
        }
    }
}
