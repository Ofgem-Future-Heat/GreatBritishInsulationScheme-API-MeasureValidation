using Microsoft.Extensions.Logging;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Infrastructure;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Services;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Enums;

namespace Ofgem.API.GBI.MeasureValidation.Service.Services
{
    public class AddressProcessingService : IAddressProcessingService
    {
        private readonly IAddressApiClient _addressApiClient;
        private readonly ILogger<AddressProcessingService> _logger;

        public AddressProcessingService(IAddressApiClient addressApiClient, ILogger<AddressProcessingService> logger)
        {
            _addressApiClient = addressApiClient;
            _logger = logger;
        }

        public async Task<IEnumerable<MeasureModel>> AddressVerificationAsync(IEnumerable<MeasureModel> measureModels)
        {
            try
            {
                var measures = measureModels.ToList();
                var addressesForVerification = measures.Select(measureModel => new AddressValidationModel()
                {
                    AddressReferenceNumber = measureModel.MeasureReferenceNumber,
                    Postcode = measureModel.PostCode,
                    BuildingName = IfNotApplicable(measureModel.BuildingName),
                    BuildingNumber = IfNotApplicable(measureModel.BuildingNumber),
                    FlatNumberOrName = IfNotApplicable(measureModel.FlatNameNumber),
                    Street = measureModel.StreetName,
                    Town = measureModel.Town,
                    Uprn = IfNotApplicable(measureModel.UniquePropertyReferenceNumber)
                });
                var verifiedAddresses = await _addressApiClient.ValidateAddressAsync(addressesForVerification);

                measures.ForEach(record =>
                {
                    var verifiedAddress = verifiedAddresses.FirstOrDefault(c =>
                        c.Address?.AddressReferenceNumber == record.MeasureReferenceNumber);
                    if (verifiedAddress == null) return;
                    record.AddressIsVerified = verifiedAddress.IsValid;
                    record.CountryCode = verifiedAddress.CountryCode;
                    record.VerifiedUprn = verifiedAddress.Uprn;
                });
                return measures;

                static string? IfNotApplicable(string? modelProperty) =>
                    modelProperty.CaseInsensitiveEquals(CommonTypesConstants.NotApplicable) ? null : modelProperty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Address verification failed. ${message}", ex.Message);
                throw;
            }
        }
    }
}
