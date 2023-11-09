using DotNetRuleEngine;
using DotNetRuleEngine.Interface;
using DotNetRuleEngine.Models;
using Microsoft.Extensions.Logging;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage1Rules
{
    public class Gbis0119 : RuleAsync<MeasureModel>
    {
        private readonly ILogger<Gbis0119> logger;

        public Gbis0119(ILogger<Gbis0119> logger)
        {
            this.logger = logger;
        }

        public async override Task<IRuleResult> InvokeAsync()
        {
            try
            {
                var measureReferenceNumber = Model.MeasureReferenceNumber;
                var supplierName = Model.SupplierName;

                if (string.IsNullOrWhiteSpace(measureReferenceNumber) || string.IsNullOrWhiteSpace(supplierName)
                    || measureReferenceNumber.Length < 3 || !measureReferenceNumber[..3].CaseInsensitiveEquals(supplierName))
                {
                    var result = new RuleResult();
                    var error = new StageValidationError()
                    {
                        MeasureReferenceNumber = measureReferenceNumber,
                        WhatWasAddedToTheNotificationTemplate = measureReferenceNumber,
                        TestNumber = nameof(Gbis0119).ToUpperInvariant(),
                    };

                    result.AddError(error);
                    return result;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "{ruleClassNameMethodName} failed: {Message}", $"{nameof(Gbis0118)}.{nameof(InvokeAsync)}", ex.Message);
                throw;
            }

            return await RuleResult.Nil();
        }
    }
}
