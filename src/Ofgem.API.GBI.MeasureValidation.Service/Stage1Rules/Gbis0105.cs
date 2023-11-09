using DotNetRuleEngine;
using DotNetRuleEngine.Interface;
using DotNetRuleEngine.Models;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Infrastructure;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage1Rules
{
    public class Gbis0105 : RuleAsync<MeasureModel>
    {
        private readonly ISupplierApiClient _supplierApiClient;
        
        public Gbis0105(ISupplierApiClient supplierApiClient)
        {
            _supplierApiClient = supplierApiClient;
        }

        public override async Task<IRuleResult> InvokeAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(Model.MeasureReferenceNumber) || Model.MeasureReferenceNumber?.Length < 3)
                {
                    var result = AddErrorToResult();
                    return result;
                }
                else
                {
                    var mrnSupplierName = Model.MeasureReferenceNumber?[..3]!.ToUpper();
                    var suppliers = await _supplierApiClient.GetSuppliersAsync();
                    if (!(suppliers.Any(supplier => supplier.SupplierName == mrnSupplierName)))
                    {
                        var result = AddErrorToResult();
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
            
            return await RuleResult.Nil();
        }

        private IRuleResult AddErrorToResult()
        {
            var result = new RuleResult();
            var error = new StageValidationError()
            {
                MeasureReferenceNumber = Model.MeasureReferenceNumber,
                WhatWasAddedToTheNotificationTemplate = Model.MeasureReferenceNumber,
                TestNumber = "GBIS0105",
            };

            result.AddError(error);
            return result;
        }
    }
}
