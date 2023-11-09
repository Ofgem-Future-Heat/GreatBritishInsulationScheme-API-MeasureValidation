using DotNetRuleEngine;
using DotNetRuleEngine.Interface;
using DotNetRuleEngine.Models;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage1Rules;

public class Gbis0109 : RuleAsync<MeasureModel>
{
    private const int MaxLength = 100;
    public override async Task<IRuleResult> InvokeAsync()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(Model.BuildingName) || Model.BuildingName.CaseInsensitiveEquals(CommonTypesConstants.NotApplicable)) return await RuleResult.Nil();

            if (Model.BuildingName.Length < 1 || Model.BuildingName.Length > MaxLength)
            {
                var result = new RuleResult();
                var error = new StageValidationError()
                {
                    MeasureReferenceNumber = Model.MeasureReferenceNumber,
                    WhatWasAddedToTheNotificationTemplate = Model.BuildingName,
                    TestNumber = "GBIS0109"
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