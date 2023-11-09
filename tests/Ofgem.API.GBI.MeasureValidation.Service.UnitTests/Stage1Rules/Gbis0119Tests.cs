using Microsoft.Extensions.Logging;
using Moq;
using MoreLinq;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage1Rules;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage1Rules
{
    public class Gbis0119Tests
    {
        private static readonly string[] BgtSupplierNames = new[] { "BGT", "bgt", "bGt" };
        private static readonly string[] BgtMeasureReferenceNumbers = new[] { "BGT1234567", "bgt1234567", "bGt1234567" };
        private static readonly string[] AbcMeasureReferenceNumbers = new[] { "ABC09174795G", "abc09174795G", "aBc09174795G" };

        [Theory]
        [MemberData(nameof(ValidPairings))]
        public async Task Gbis0119_Pass_ReturnsNilResult(string measureReferenceNumber, string supplierName)
        {
            // Arrange
            var rule = CreateGbis0119Rule(measureReferenceNumber, supplierName);

            // Act
            var result = await rule.InvokeAsync();

            // Assert
            Assert.Null(result);
        }

        public static TheoryData<string, string> ValidPairings()
        {
            var cartesianProduct = BgtMeasureReferenceNumbers.Cartesian(BgtSupplierNames, (x, y) => (x, y));
            var result = CreateTheoryData(cartesianProduct);

            return result;
        }

        private static TheoryData<string, string> CreateTheoryData(IEnumerable<(string x, string y)> items)
        {
            var result = new TheoryData<string, string>();
            foreach (var item in items)
            {
                result.Add(item.Item1, item.Item2);
            }

            return result;
        }

        [Theory]
        [MemberData(nameof(InvalidPairings))]
        public async Task Gbis0119_Fail_ReturnsErrorResult(string measureReferenceNumber, string supplierName)
        {
            // Arrange
            var rule = CreateGbis0119Rule(measureReferenceNumber, supplierName);

            // Act
            var result = await rule.InvokeAsync();

            // Assert
            var stageValidationErrors = (result.Result as IEnumerable<StageValidationError>)!;
            var error = stageValidationErrors.SingleOrDefault();
            Assert.NotNull(error);
            Assert.Equal("GBIS0119", error.TestNumber);
            Assert.Equal(measureReferenceNumber, error.WhatWasAddedToTheNotificationTemplate);
        }

        public static TheoryData<string, string> InvalidPairings()
        {
            var cartesianProduct = AbcMeasureReferenceNumbers.Cartesian(BgtSupplierNames, (x, y) => (x, y));

            var result = CreateTheoryData(cartesianProduct);

            return result;
        }

        [Theory]
        [InlineData("A")]
        [InlineData("AB")]
        public async Task Gbis0119_MeasureReferenceNumberShorterThan3Characters_ReturnsErrorResult(string measureReferenceNumber)
        {
            // Arrange
            var rule = CreateGbis0119Rule(measureReferenceNumber, "ABC");

            // Act
            var result = await rule.InvokeAsync();

            // Assert
            var stageValidationErrors = (result.Result as IEnumerable<StageValidationError>)!;
            var error = stageValidationErrors.SingleOrDefault();
            Assert.NotNull(error);
            Assert.Equal(measureReferenceNumber, error.WhatWasAddedToTheNotificationTemplate);
        }

        private static Gbis0119 CreateGbis0119Rule(string measureReferenceNumber, string supplierName)
        {
            var loggerMock = new Mock<ILogger<Gbis0119>>();

            return new Gbis0119(loggerMock.Object)
            {
                Model = new MeasureModel()
                {
                    MeasureReferenceNumber = measureReferenceNumber,
                    SupplierName = supplierName,
                }
            };
        }
    }
}
