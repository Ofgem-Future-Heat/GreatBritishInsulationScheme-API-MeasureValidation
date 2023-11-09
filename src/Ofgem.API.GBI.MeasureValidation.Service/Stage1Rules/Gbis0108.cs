using DotNetRuleEngine;
using DotNetRuleEngine.Interface;
using DotNetRuleEngine.Models;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage1Rules;

public class Gbis0108 : RuleAsync<MeasureModel>
{
    private const int MaxLength = 50;

    public override async Task<IRuleResult> InvokeAsync()
    {
        try
        {
            bool buildingNumberIsBlankOrNa = string.IsNullOrWhiteSpace(Model.BuildingNumber) || Model.BuildingNumber.CaseInsensitiveEquals(CommonTypesConstants.NotApplicable);

            if (!buildingNumberIsBlankOrNa && (Model.BuildingNumber?.Length < 1 || Model.BuildingNumber?.Length > MaxLength))
            {
                var measureCoreValidationError = new StageValidationError
                {
                    MeasureReferenceNumber = Model.MeasureReferenceNumber,
                    WhatWasAddedToTheNotificationTemplate = Model.BuildingNumber,
                    TestNumber = "GBIS0108",
                };
                var result = new RuleResult();
                result.AddError(measureCoreValidationError);

                return result;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }

        return await RuleResult.Nil();
    }
}