using DotNetRuleEngine;
using DotNetRuleEngine.Interface;
using DotNetRuleEngine.Models;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage1Rules;

public class Gbis0111 : RuleAsync<MeasureModel>
{
    public override async Task<IRuleResult> InvokeAsync()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(Model.Town) || Model.Town.CaseInsensitiveEquals(CommonTypesConstants.NotApplicable))
            {
                var result = new RuleResult();
                var error = new StageValidationError()
                {
                    MeasureReferenceNumber = Model.MeasureReferenceNumber,
                    WhatWasAddedToTheNotificationTemplate = Model.Town,
                    TestNumber = "GBIS0111"
                };

                result.AddError(error);
                return result;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            throw;
        }

        return await RuleResult.Nil();
    }


}