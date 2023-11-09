﻿using DotNetRuleEngine;
using DotNetRuleEngine.Interface;
using DotNetRuleEngine.Models;
using Microsoft.Extensions.Logging;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage1Rules;

public class Gbis0116 : RuleAsync<MeasureModel>
{
    private readonly ILogger<Gbis0116> _logger;

    public Gbis0116(ILogger<Gbis0116> logger)
    {
        _logger = logger;
    }

    public override async Task<IRuleResult> InvokeAsync()
    {
        try
        {
            bool inList = Model.SupplierLicences.Select(c => c.SupplierLicenceReference).CaseInsensitiveContainsInList(Model.SupplierReference);

            if (!inList)
            {
                var result = new RuleResult();
                var error = new StageValidationError()
                {
                    MeasureReferenceNumber = Model.MeasureReferenceNumber,
                    WhatWasAddedToTheNotificationTemplate = Model.SupplierReference,
                    TestNumber = "GBIS0116",
                };

                result.AddError(error);
                return result;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Gbis0116.InvokeAsync() failed: {exMessage}", ex.Message);
            throw;
        }

        return await RuleResult.Nil();
    }
}
