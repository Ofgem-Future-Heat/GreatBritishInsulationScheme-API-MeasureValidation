using DotNetRuleEngine.Interface;
using DotNetRuleEngine.Models;
using DotNetRuleEngine;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using System.Text.RegularExpressions;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage1Rules
{
    public class Gbis0114 : RuleAsync<MeasureModel>
    {
        private const string _postCodeRegex = "^[A-Z]{1,2}[0-9]{1,2}[A-Z]?(\\s *[0-9][A-Z]{1,2})?$";

        public override async Task<IRuleResult> InvokeAsync()
        {
            try
            {
                Model.PostCode = Model.PostCode?.Trim();
                var regex = new Regex(_postCodeRegex);

                if (Model.PostCode == null || !regex.IsMatch(Model.PostCode))
                {
                    var result = new RuleResult();
                    var error = new StageValidationError()
                    {
                        MeasureReferenceNumber = Model.MeasureReferenceNumber,
                        WhatWasAddedToTheNotificationTemplate = Model.PostCode,
                        TestNumber = "GBIS0114",
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