using DotNetRuleEngine.Interface;
using DotNetRuleEngine.Models;
using DotNetRuleEngine;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using System.Globalization;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Services;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage1Rules
{
    public class Gbis0102 : RuleAsync<MeasureModel>
    {
        private readonly TimeProvider _timeProvider;
        public Gbis0102(TimeProvider timeProvider)
        {
            _timeProvider = timeProvider;
        }
        public override async Task<IRuleResult> InvokeAsync()
        {
            try
            {
                _ = DateTime.TryParseExact(Model.DateOfCompletedInstallation, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var doci);

                var timeNow = _timeProvider.GetLocalNow();
                if (doci > timeNow)
                {
                    var result = new RuleResult();
                    var error = new StageValidationError()
                    {
                        MeasureReferenceNumber = Model.MeasureReferenceNumber,
                        WhatWasAddedToTheNotificationTemplate = Model.DateOfCompletedInstallation,
                        TestNumber = "GBIS0102",
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