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
    public class DeleteUserFromProcedureCommandHandler : IRequestHandler<DeleteUserFromProcedureCommand, ApiResponse<Unit>>
    {
        private readonly RLContext _context;

        public DeleteUserFromProcedureCommandHandler(RLContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<Unit>> Handle(DeleteUserFromProcedureCommand request, CancellationToken cancellationToken)
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

        private async Task<ApiResponse<Unit>> TryHandle(DeleteUserFromProcedureCommand request, CancellationToken cancellationToken)
        {
            if (request.UserId < 1)
                return ApiResponse<Unit>.Fail(new BadRequestException($"Invalid {nameof(DeleteUserFromProcedureCommand.UserId)}"));
            if (request.ProcedureId < 1)
                return ApiResponse<Unit>.Fail(new BadRequestException($"Invalid {nameof(DeleteUserFromProcedureCommand.ProcedureId)}"));

            var procedureUser = await _context.ProcedureUsers
                                            .FirstOrDefaultAsync(p => p.ProcedureId == request.ProcedureId && p.UserId == request.UserId, cancellationToken);

            if(procedureUser != null)
            {
                _context.Remove(procedureUser);
            }

            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse<Unit>.Succeed(new Unit());
        }

    }
}
