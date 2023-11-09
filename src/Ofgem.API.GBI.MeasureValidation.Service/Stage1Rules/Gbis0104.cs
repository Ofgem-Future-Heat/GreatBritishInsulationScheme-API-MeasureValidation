using DotNetRuleEngine;
using DotNetRuleEngine.Interface;
using DotNetRuleEngine.Models;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Constants;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage1Rules
{
    public class Gbis0104 : RuleAsync<MeasureModel>
    {
        public override async Task<IRuleResult> InvokeAsync()
        {
            try
            {
                if (Model.MeasureReferenceNumber != null && !GbisRegexes.MeasureReferenceNumberRegex().IsMatch(input: Model.MeasureReferenceNumber))
                {
                    var result = new RuleResult();
                    var error = new StageValidationError()
                    {
                        MeasureReferenceNumber = Model.MeasureReferenceNumber,
                        WhatWasAddedToTheNotificationTemplate = Model.MeasureReferenceNumber,
                        TestNumber = "GBIS0104",
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
}
