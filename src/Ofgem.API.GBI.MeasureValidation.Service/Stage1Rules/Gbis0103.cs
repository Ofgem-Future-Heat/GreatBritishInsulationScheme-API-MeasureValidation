using DotNetRuleEngine.Interface;
using DotNetRuleEngine.Models;
using DotNetRuleEngine;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using System.Globalization;
using Microsoft.Extensions.Options;
using Ofgem.API.GBI.MeasureValidation.Application.Common;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage1Rules
{
    public class Gbis0103 : RuleAsync<MeasureModel>
    {
        private readonly DateTime schemeStartDate;
        private readonly DateTime schemeEndDate;

        public Gbis0103(IOptions<SchemeDetailsOptions> scheduleOptions)
        {
            schemeStartDate = scheduleOptions.Value.StartDate;
            schemeEndDate = scheduleOptions.Value.EndDate;
        }

        public override async Task<IRuleResult> InvokeAsync()
        {
            try
            {
                _ = DateTime.TryParseExact(Model.DateOfCompletedInstallation, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var doci) ? doci : null as DateTime?;

                if (doci < schemeStartDate || doci > schemeEndDate)
                {
                    var result = new RuleResult();
                    var error = new StageValidationError()
                    {
                        MeasureReferenceNumber = Model.MeasureReferenceNumber,
                        WhatWasAddedToTheNotificationTemplate = Model.DateOfCompletedInstallation,
                        TestNumber = "GBIS0103",
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