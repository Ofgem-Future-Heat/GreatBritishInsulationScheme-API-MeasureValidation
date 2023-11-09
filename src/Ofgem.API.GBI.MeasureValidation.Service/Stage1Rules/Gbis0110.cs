using DotNetRuleEngine.Interface;
using DotNetRuleEngine.Models;
using DotNetRuleEngine;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage1Rules
{
    public class Gbis0110 : RuleAsync<MeasureModel>
    {
        public override async Task<IRuleResult> InvokeAsync()
        {
            try
            {
                var acceptedEligibilityTypes = Model.ReferenceDataDetails.EligibilityTypesList!;

                if (!acceptedEligibilityTypes.CaseInsensitiveContainsInList(Model.EligibilityType!))
                {
                    var result = new RuleResult();
                    var error = new StageValidationError()
                    {
                        MeasureReferenceNumber = Model.MeasureReferenceNumber,
                        WhatWasAddedToTheNotificationTemplate = Model.EligibilityType,
                        TestNumber = "GBIS0110",
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