using ClinicManagement.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicManagement.Application.Commands.Users.DeactivateUser
{
    public class DeactivateUserHandler : IRequestHandler<DeactivateUserCommand, Unit>
    {
        private readonly IEnumerable<IUserDeactivationStrategy> _strategies;

        public DeactivateUserHandler(IEnumerable<IUserDeactivationStrategy> strategies)
        {
            _strategies = strategies;
        }

        public async Task<Unit> Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
        {
            var strategy = _strategies.FirstOrDefault(s => s.CanHandle(request.UserType));

            if (strategy == null)
                throw new Exception($"No strategy found for user type {request.UserType}");

            await strategy.DeactivateAsync(request.UserId);
            return Unit.Value;
        }
    }
}
