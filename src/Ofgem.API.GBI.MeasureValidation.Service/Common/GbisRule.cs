using DotNetRuleEngine.Interface;
using DotNetRuleEngine.Models;
using DotNetRuleEngine;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;

namespace Ofgem.API.GBI.MeasureValidation.Service.Common
{
    public class GbisRule : RuleAsync<MeasureModel>
    {
        private readonly IRuleLogic ruleLogic;

        public GbisRule(IRuleLogic ruleLogic)
        {
            this.ruleLogic = ruleLogic;
        }

        public override Task InitializeAsync()
        {
            Configuration.Constraint = ruleLogic.FailureCondition;

            return base.InitializeAsync();
        }

        public override async Task<IRuleResult> InvokeAsync()
        {
            var result = new RuleResult();

            result.AddError(new StageValidationError
            {
                MeasureReferenceNumber = Model.MeasureReferenceNumber,
                WhatWasAddedToTheNotificationTemplate = ruleLogic.FailureFieldValueFunction(Model),
                TestNumber = ruleLogic.TestNumber,
            });

            return await Task.FromResult(result);
        }
    }
}
