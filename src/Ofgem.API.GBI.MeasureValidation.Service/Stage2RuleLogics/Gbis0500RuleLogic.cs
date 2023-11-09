using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Common;

namespace Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics
{
    public class Gbis0500RuleLogic : IRuleLogic
    {
        public Predicate<MeasureModel> FailureCondition { get; } = measureModel =>
            !measureModel.AddressIsVerified;

        public Func<MeasureModel, string?> FailureFieldValueFunction { get; } = FailedAddressFields;

        public string TestNumber { get; } = "GBIS0500";


        public static string FailedAddressFields(MeasureModel measureModel)
        {
            var addressList = new List<string>
            {
                new(measureModel.FlatNameNumber),
                new(measureModel.BuildingName),
                new(measureModel.BuildingNumber),
                new(measureModel.StreetName),
                new(measureModel.PostCode)
            };

            var buildList = (from address in addressList where address != null where !string.IsNullOrWhiteSpace(address) select address).ToList();
            var result = string.Join(",", buildList);

            return result;
        }
    }
}