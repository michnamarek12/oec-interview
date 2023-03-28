using Azure;
using Azure.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RL.Backend.Exceptions;
using RL.Backend.Models;
using RL.Data;
using RL.Data.DataModels;
using System.Threading;

namespace RL.Backend.Commands.Handlers.Plans
{
    public class AddUserToPlanProcedureCommandHandler : IRequestHandler<AddUserToPlanProcedureCommand, ApiResponse<Unit>>
    {
        private readonly RLContext _context;

        public AddUserToPlanProcedureCommandHandler(RLContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<Unit>> Handle(AddUserToPlanProcedureCommand request, CancellationToken cancellationToken)
        {
            try
            {
                return await TryHandle(request, cancellationToken);
            }
            catch (Exception e)
            {
                return ApiResponse<Unit>.Fail(e);
            }
        }

        private async Task<ApiResponse<Unit>> TryHandle(AddUserToPlanProcedureCommand request, CancellationToken cancellationToken)
        {
            if (request.PlanId < 1)
                return GetIncorrectIdResponse(nameof(AddUserToPlanProcedureCommand.PlanId));
            if (request.UserId < 1)
                return GetIncorrectIdResponse(nameof(AddUserToPlanProcedureCommand.UserId));
            if (request.ProcedureId < 1)
                return GetIncorrectIdResponse(nameof(AddUserToPlanProcedureCommand.ProcedureId));


            var planProcedure = await _context.PlanProcedures
                                        .FirstOrDefaultAsync(pp => pp.PlanId == request.PlanId &&
                                                                    pp.ProcedureId == request.ProcedureId, cancellationToken);
            var user = await _context.Users.FirstOrDefaultAsync(p => p.UserId == request.UserId, cancellationToken);

            if (user is null)
                return ApiResponse<Unit>.Fail(new NotFoundException($"{nameof(AddUserToPlanProcedureCommand.UserId)}: {request.UserId} not found"));

            if (planProcedure is null)
                return ApiResponse<Unit>.Fail(new NotFoundException($"PlanProcedure with {nameof(AddUserToPlanProcedureCommand.PlanId)}: {request.PlanId} and {nameof(AddUserToPlanProcedureCommand.ProcedureId)} : {request.ProcedureId} not found"));

            if (_context.UserPlanProcedureRelations.Any(r => r.PlanId == planProcedure.PlanId && r.ProcedureId == planProcedure.ProcedureId && r.UserId == request.UserId))
                return ApiResponse<Unit>.Succeed(new Unit());

            _context.UserPlanProcedureRelations.Add(new UserPlanProcedureRelation
            {
                ProcedureId = request.PlanId,
                UserId = request.UserId,
                PlanId = request.PlanId,    
            });

            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse<Unit>.Succeed(new Unit());
        }

        private static ApiResponse<Unit> GetIncorrectIdResponse(string propertyName)
        {
            return ApiResponse<Unit>.Fail(new BadRequestException($"Invalid {propertyName}"));
        }
    }
}
