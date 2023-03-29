using Azure;
using Azure.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RL.Backend.Exceptions;
using RL.Backend.Models;
using RL.Data;
using RL.Data.DataModels;
using System.Numerics;
using System.Threading;

namespace RL.Backend.Commands.Handlers.Plans
{
    public class AddUserToProcedureCommandHandler : IRequestHandler<AddUserToProcedureCommand, ApiResponse<Unit>>
    {
        private readonly RLContext _context;

        public AddUserToProcedureCommandHandler(RLContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<Unit>> Handle(AddUserToProcedureCommand request, CancellationToken cancellationToken)
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

        private async Task<ApiResponse<Unit>> TryHandle(AddUserToProcedureCommand request, CancellationToken cancellationToken)
        {
            if (request.UserId < 1)
                return GetIncorrectIdResponse(nameof(AddUserToProcedureCommand.UserId));
            if (request.ProcedureId < 1)
                return GetIncorrectIdResponse(nameof(AddUserToProcedureCommand.ProcedureId));


            var procedure = await _context.Procedures
                                                .Include(p => p.ProcedureUsers)
                                                .FirstOrDefaultAsync(p => p.ProcedureId == request.ProcedureId, cancellationToken);
            var user = await _context.Users.FirstOrDefaultAsync(p => p.UserId == request.UserId, cancellationToken);

            if (user is null)
                return ApiResponse<Unit>.Fail(new NotFoundException($"{nameof(AddUserToProcedureCommand.UserId)}: {request.UserId} not found"));

            if (procedure is null)
                return ApiResponse<Unit>.Fail(new NotFoundException($"{nameof(AddUserToProcedureCommand.ProcedureId)}: {request.ProcedureId} not found"));

            if (procedure.ProcedureUsers.Any(p => p.UserId == request.UserId))
                return ApiResponse<Unit>.Succeed(new Unit());

            _context.ProcedureUsers.Add(new ProcedureUser
            {
                UserId = request.UserId,
                ProcedureId = request.ProcedureId,    
            });

            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse<Unit>.Succeed(new Unit());
        }

        private ApiResponse<Unit> GetIncorrectIdResponse(string propertyName)
        {
            return ApiResponse<Unit>.Fail(new BadRequestException($"Invalid {propertyName}"));
        }
    }
}
