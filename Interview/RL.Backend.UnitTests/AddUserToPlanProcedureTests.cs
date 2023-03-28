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
    public class AddUserToPlanProcedureTests
    {
        [TestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        [DataRow(int.MinValue)]
        public async Task AddUserToPlanProcedureRelation_Tests_UserId_ReturnsBadRequest(int userId)
        {
            //Given
            var context = new Mock<RLContext>();
            var sut = new AddUserToPlanProcedureCommandHandler(context.Object);
            var request = new AddUserToPlanProcedureCommand()
            {
                PlanId = 1,
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
        public async Task AddUserToPlanProcedureRelation_Tests_ProcedureId_ReturnsBadRequest(int procedureId)
        {
            //Given
            var context = new Mock<RLContext>();
            var sut = new AddUserToPlanProcedureCommandHandler(context.Object);
            var request = new AddUserToPlanProcedureCommand()
            {
                PlanId = 1,
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
        [DataRow(-1)]
        [DataRow(0)]
        [DataRow(int.MinValue)]
        public async Task AddUserToPlanProcedureRelation_Tests_PlanId_ReturnsBadRequest(int planId)
        {
            //Given
            var context = new Mock<RLContext>();
            var sut = new AddUserToPlanProcedureCommandHandler(context.Object);
            var request = new AddUserToPlanProcedureCommand()
            {
                PlanId = planId,
                ProcedureId = 1,
                UserId = 1

            };
            //When
            var result = await sut.Handle(request, new CancellationToken());

            //Then
            result.Exception.Should().BeOfType(typeof(BadRequestException));
            result.Succeeded.Should().BeFalse();
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(19)]
        [DataRow(35)]
        public async Task AddUserToPlanProcedureRelation_Tests_PlanIdNotFound_ReturnsNotFound(int planId)
        {
            //Given
            var context = DbContextHelper.CreateContext();
            var sut = new AddUserToPlanProcedureCommandHandler(context);
            var request = new AddUserToPlanProcedureCommand()
            {
                PlanId = planId,
                ProcedureId = 1,
                UserId = 1
            };

            context.Plans.Add(new Data.DataModels.Plan
            {
                PlanId = planId + 1
            });
            await context.SaveChangesAsync();

            //When
            var result = await sut.Handle(request, new CancellationToken());

            //Then
            result.Exception.Should().BeOfType(typeof(NotFoundException));
            result.Succeeded.Should().BeFalse();
        }

        [TestMethod]
        [DataRow(19)]
        [DataRow(35)]
        public async Task AddUserToPlanProcedureRelation_Tests_UserIdNotFound_ReturnsNotFound(int userId)
        {
            //Given
            var context = DbContextHelper.CreateContext();
            var sut = new AddUserToPlanProcedureCommandHandler(context);
            var request = new AddUserToPlanProcedureCommand()
            {
                PlanId = 1,
                ProcedureId = 1,
                UserId = userId
            };

            context.Plans.Add(new Plan
            {
                PlanId = 1
            });
            context.PlanProcedures.Add(new PlanProcedure
            {
                PlanId = 1,
                ProcedureId =1
            });
            await context.SaveChangesAsync();

            //When
            var result = await sut.Handle(request, new CancellationToken());

            //Then
            result.Exception.Should().BeOfType(typeof(NotFoundException));
            result.Succeeded.Should().BeFalse();
        }

        [TestMethod]
        public async Task AddUserToPlanProcedureRelation_Tests_PlanProcedure_ReturnsNotFound()
        {
            //Given
            var context = DbContextHelper.CreateContext();
            var sut = new AddUserToPlanProcedureCommandHandler(context);
            var request = new AddUserToPlanProcedureCommand()
            {
                PlanId = 1,
                ProcedureId = 1,
                UserId = 1
            };

            context.Plans.Add(new Plan
            {
                PlanId = 1
            });
            await context.SaveChangesAsync();

            //When
            var result = await sut.Handle(request, new CancellationToken());

            //Then
            result.Exception.Should().BeOfType(typeof(NotFoundException));
            result.Succeeded.Should().BeFalse();
        }

        [TestMethod]
        [DataRow(1, 1)]
        [DataRow(19, 1010)]
        [DataRow(35, 69)]
        public async Task AddUserToPlanProcedureRelation_Tests_AlreadyContainsUserToPlanProcedureRelation_ReturnsSuccess(int planId, int procedureId)
        {
            //Given
            int userId = 1;
            var context = DbContextHelper.CreateContext();
            var sut = new AddUserToPlanProcedureCommandHandler(context);
            var request = new AddUserToPlanProcedureCommand()
            {
                PlanId = planId,
                ProcedureId = procedureId,
                UserId = userId
            };

            context.Plans.Add(new Plan
            {
                PlanId = planId
            });
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
            context.PlanProcedures.Add(new PlanProcedure
            {
                ProcedureId = procedureId,
                PlanId = planId
            });
            context.UserPlanProcedureRelations.Add(new UserPlanProcedureRelation
            {
                ProcedureId = procedureId,
                PlanId = planId,
                UserId = userId
            });
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
        public async Task AddUserToPlanProcedureRelation_Tests_DoesntContainsUserToPlanProcedureRelation_ReturnsSuccess(int planId, int procedureId)
        {
            //Given
            int userId = 1;
            var context = DbContextHelper.CreateContext();
            var sut = new AddUserToPlanProcedureCommandHandler(context);
            var request = new AddUserToPlanProcedureCommand()
            {
                PlanId = planId,
                ProcedureId = procedureId,
                UserId = userId
            };

            context.Plans.Add(new Plan
            {
                PlanId = planId
            });
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
            context.PlanProcedures.Add(new PlanProcedure
            {
                ProcedureId = procedureId,
                PlanId = planId
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
