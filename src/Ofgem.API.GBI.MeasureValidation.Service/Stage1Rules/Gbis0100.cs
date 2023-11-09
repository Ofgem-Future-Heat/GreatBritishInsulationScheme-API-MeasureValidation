using DotNetRuleEngine.Interface;
using DotNetRuleEngine.Models;
using DotNetRuleEngine;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage1Rules
{
    public class Gbis0100 : RuleAsync<MeasureModel>
    {
        public override async Task<IRuleResult> InvokeAsync()
        {
            try
            {
                var acceptedPon = Model.ReferenceDataDetails.PurposeOfNotificationsList!;

                if (!acceptedPon.CaseInsensitiveContainsInList(Model.PurposeOfNotification!))
                {
                    var result = new RuleResult();
                    var error = new StageValidationError()
                    {
                        MeasureReferenceNumber = Model.MeasureReferenceNumber,
                        WhatWasAddedToTheNotificationTemplate = Model.PurposeOfNotification,
                        TestNumber = "GBIS0100",
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