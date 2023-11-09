using DotNetRuleEngine;
using DotNetRuleEngine.Interface;
using DotNetRuleEngine.Models;
using Microsoft.Extensions.Logging;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage1Rules;

public class Gbis0118 : RuleAsync<MeasureModel>
{
    private readonly ILogger<Gbis0118> _logger;

    public Gbis0118(ILogger<Gbis0118> logger)
    {
        _logger = logger;
    }

    public async override Task<IRuleResult> InvokeAsync()
    {
        if (Model.SupplierReference != null && Model.ExistingSupplierReference != null)
        {
            try
            {
                string supplierReference = Model.SupplierReference.ToLowerInvariant().Trim();
                string existingSupplierReference = Model.ExistingSupplierReference.ToLowerInvariant().Trim();

                if (supplierReference != existingSupplierReference)
                {
                    var result = new RuleResult();
                    var error = new StageValidationError()
                    {
                        MeasureReferenceNumber = Model.MeasureReferenceNumber,
                        WhatWasAddedToTheNotificationTemplate = Model.SupplierReference,
                        TestNumber = "GBIS0118",
                    };
                    result.AddError(error);
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Gbis0118.InvokeAsync() failed: {ex.Message}", ex);
                throw;
            }
        }
        return await RuleResult.Nil();
    }

}

