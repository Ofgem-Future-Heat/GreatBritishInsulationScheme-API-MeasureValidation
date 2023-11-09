using DotNetRuleEngine;
using DotNetRuleEngine.Interface;
using DotNetRuleEngine.Models;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage1Rules;

public class Gbis0112 : RuleAsync<MeasureModel>
{
    private const int _maxLength = 50;

    public override async Task<IRuleResult> InvokeAsync()
    {
        try
        {
            if (!string.IsNullOrEmpty(Model.Town) && Model?.Town?.Length > _maxLength)
            {
                var result = new RuleResult();
                var error = new StageValidationError()
                {
                    MeasureReferenceNumber = Model.MeasureReferenceNumber,
                    WhatWasAddedToTheNotificationTemplate = Model.Town,
                    TestNumber = "GBIS0112"
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