using DotNetRuleEngine.Interface;
using DotNetRuleEngine.Models;
using DotNetRuleEngine;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage1Rules
{
    public class Gbis0107 : RuleAsync<MeasureModel>
    {
        public override async Task<IRuleResult> InvokeAsync()
        {
            try
            {
                var acceptedMeasureType = Model.ReferenceDataDetails.MeasureTypesList?.Select(m => m.Name!) ?? new List<string>();

                if (!acceptedMeasureType.CaseInsensitiveContainsInList(Model.MeasureType!))
                {
                    var result = new RuleResult();
                    var error = new StageValidationError()
                    {
                        MeasureReferenceNumber = Model.MeasureReferenceNumber,
                        WhatWasAddedToTheNotificationTemplate = Model.MeasureType,
                        TestNumber = "GBIS0107",
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