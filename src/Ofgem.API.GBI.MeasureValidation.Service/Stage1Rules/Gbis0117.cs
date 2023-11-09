using DotNetRuleEngine;
using DotNetRuleEngine.Interface;
using DotNetRuleEngine.Models;
using Microsoft.Extensions.Logging;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage1Rules;

public class Gbis0117 : RuleAsync<MeasureModel>
{
    private readonly ILogger<Gbis0117> _logger;

    public Gbis0117(ILogger<Gbis0117> logger)
    {
        _logger = logger;
    }

    public async override Task<IRuleResult> InvokeAsync()
    {
        try
        {
            var matchedSupplierLicence = Model.SupplierLicences.FirstOrDefault(c => c.SupplierLicenceReference == Model.SupplierReference);

            var isMatched = matchedSupplierLicence != null && matchedSupplierLicence.SupplierName == Model.SupplierName;

            if (!isMatched)
            {
                var result = new RuleResult();
                var error = new StageValidationError()
                {
                    MeasureReferenceNumber = Model.MeasureReferenceNumber,
                    WhatWasAddedToTheNotificationTemplate = Model.SupplierReference,
                    TestNumber = "GBIS0117",
                };

                result.AddError(error);
                return result;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Gbis0117.InvokeAsync() failed: {ex.Message}", ex);
            throw;
        }

        return await RuleResult.Nil();
    }
}
