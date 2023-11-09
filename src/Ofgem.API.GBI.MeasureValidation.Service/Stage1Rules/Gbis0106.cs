using DotNetRuleEngine;
using DotNetRuleEngine.Interface;
using DotNetRuleEngine.Models;
using Microsoft.Extensions.Logging;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Service.Constants;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage1Rules
{
    public class Gbis0106 : RuleAsync<MeasureModel>
    {
        private readonly ILogger<Gbis0106> _logger;
        private readonly List<int> _userEditableStatusIds = new()
        {
            MeasureStatusConstants.NotifiedIncomplete,
            MeasureStatusConstants.NotifiedPending,
            MeasureStatusConstants.WithSupplier
        };

        public Gbis0106(ILogger<Gbis0106> logger)
        {
            _logger = logger;
        }
        
        public override async Task<IRuleResult> InvokeAsync()
        {
            if (Model.MeasureReferenceNumber == null || !Model.MeasureStatusId.HasValue || _userEditableStatusIds.Contains((int)Model.MeasureStatusId))
            {
                return await RuleResult.Nil();
            }
            try
            {
                var result = new RuleResult();
                var error = new StageValidationError()
                {
                    MeasureReferenceNumber = Model.MeasureReferenceNumber,
                    WhatWasAddedToTheNotificationTemplate = Model.MeasureReferenceNumber,
                    TestNumber = "GBIS0106"
                };

                result.AddError(error);
                return result;
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw;
            }
        }
    }
}
