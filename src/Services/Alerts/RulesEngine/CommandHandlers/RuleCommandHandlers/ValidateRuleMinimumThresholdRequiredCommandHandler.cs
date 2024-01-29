﻿using MediatR;
using RulesEngine.Commands.RuleCommands;

namespace RulesEngine.CommandHandlers.RulesCommandHandler
{
    public class ValidateRuleMinimumThresholdRequiredCommandHandler : IRequestHandler<ValidateRuleMinimumThresholdRequiredCommand, bool>
    {
        public async Task<bool> Handle(ValidateRuleMinimumThresholdRequiredCommand request, CancellationToken cancellationToken)
        {
            request.MatchedClassesByAlert.Add(request.RequestClass);
            return request.RequestClass.Confidence > request.RuleConfig.MinimumThreshold * 0.01;
        }
    }
}