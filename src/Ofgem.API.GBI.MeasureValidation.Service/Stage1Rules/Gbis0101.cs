using DotNetRuleEngine.Interface;
using DotNetRuleEngine.Models;
using DotNetRuleEngine;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using System.Globalization;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage1Rules
{
    public class Gbis0101 : RuleAsync<MeasureModel>
    {
        public override async Task<IRuleResult> InvokeAsync()
        {
            try
            {
                var dateFormatIsDdMmYyyy = DateTime.TryParseExact(Model.DateOfCompletedInstallation, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
                if (!dateFormatIsDdMmYyyy)
                {
                    var result = new RuleResult();
                    var error = new StageValidationError()
                    {
                        MeasureReferenceNumber = Model.MeasureReferenceNumber,
                        WhatWasAddedToTheNotificationTemplate = Model.DateOfCompletedInstallation,
                        TestNumber = "GBIS0101",
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
}