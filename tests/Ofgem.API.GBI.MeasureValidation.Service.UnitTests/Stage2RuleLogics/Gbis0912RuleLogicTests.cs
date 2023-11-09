﻿using Ofgem.API.GBI.MeasureValidation.Application.Constants;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Constants;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis0912RuleLogicTests
    {
        [Theory]
        [InlineData(EligibilityTypes.LISupplierEvidence, TenureTypes.PrivateRentedSector, ReferenceDataConstants.CountryCode.Scotland, "38.5")]
        [InlineData(EligibilityTypes.LISupplierEvidence, TenureTypes.PrivateRentedSector, ReferenceDataConstants.CountryCode.Scotland, "54.4")]
        [InlineData(EligibilityTypes.LISupplierEvidence, TenureTypes.PrivateRentedSector, ReferenceDataConstants.CountryCode.England, "33")]
        [InlineData(EligibilityTypes.LISupplierEvidence, TenureTypes.SocialHousing, ReferenceDataConstants.CountryCode.Scotland, "60")]
        [InlineData(EligibilityTypes.LISocialHousing, TenureTypes.PrivateRentedSector, ReferenceDataConstants.CountryCode.Scotland, "60")]
        public void Gbis0912Rule_WithValidInput_PassValidation(string eligibilityType, string tenureType, string countryCode, string startingSapRating)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "BGT123456789",
                EligibilityType = eligibilityType,
                TenureType = tenureType,
                CountryCode = countryCode,
                StartingSAPRating = startingSapRating
            };
            var rule = new Gbis0912RuleLogic();
            var result = rule.FailureCondition(measureModel);

            Assert.False(result);
        }

        [Theory]
        [InlineData(EligibilityTypes.LISupplierEvidence, TenureTypes.PrivateRentedSector, ReferenceDataConstants.CountryCode.Scotland, "37")]
        [InlineData(EligibilityTypes.LISupplierEvidence, TenureTypes.PrivateRentedSector, ReferenceDataConstants.CountryCode.Scotland, "54.5")]
        public void Gbis0912Rule_WithInvalidInput_FailsValidation(string eligibilityType, string tenureType, string countryCode, string startingSapRating)
        {
            var measureModel = new MeasureModel
            {
                MeasureReferenceNumber = "BGT0123456789",
                EligibilityType = eligibilityType,
                TenureType = tenureType,
                CountryCode = countryCode,
                StartingSAPRating = startingSapRating
            };
            var rule = new Gbis0912RuleLogic();
            
            var result = rule.FailureCondition(measureModel);
            var observedValue = rule.FailureFieldValueFunction(measureModel);

            Assert.True(result);
            Assert.Equal("GBIS0912", rule.TestNumber);
            Assert.Equal(measureModel.StartingSAPRating, observedValue);
        }
    }
}

