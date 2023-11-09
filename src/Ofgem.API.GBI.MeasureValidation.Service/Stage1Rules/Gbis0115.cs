using DotNetRuleEngine;
using DotNetRuleEngine.Interface;
using DotNetRuleEngine.Models;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage1Rules;

public class Gbis0115 : RuleAsync<MeasureModel>
{
    public override async Task<IRuleResult> InvokeAsync()
    {
        try
        {
            bool numberIsBlank = string.IsNullOrWhiteSpace(Model.BuildingNumber) || Model.BuildingNumber.CaseInsensitiveEquals(CommonTypesConstants.NotApplicable);
            bool nameIsBlank = string.IsNullOrWhiteSpace(Model.BuildingName) || Model.BuildingName.CaseInsensitiveEquals(CommonTypesConstants.NotApplicable);

            if (numberIsBlank && nameIsBlank)
            {
                var result = new RuleResult();
                var error = new StageValidationError()
                {
                    MeasureReferenceNumber = Model.MeasureReferenceNumber,
                    WhatWasAddedToTheNotificationTemplate = Model.BuildingNumber + ", " + Model.BuildingName,
                    TestNumber = "GBIS0115",
                };
                result.AddError(error);

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