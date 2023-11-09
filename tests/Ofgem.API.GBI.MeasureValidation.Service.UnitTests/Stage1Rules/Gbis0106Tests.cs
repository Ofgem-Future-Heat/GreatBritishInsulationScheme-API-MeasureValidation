using Microsoft.Extensions.Logging;
using Moq;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Constants;
using Ofgem.API.GBI.MeasureValidation.Service.Stage1Rules;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage1Rules
{
    public class Gbis0106Tests
    {
        private readonly Mock<ILogger<Gbis0106>> _loggerMock;

        public Gbis0106Tests()
        {
            _loggerMock = new Mock<ILogger<Gbis0106>>();
        }

        public static TheoryData<int> PassCaseStatusIds =>
            new()
            {
                MeasureStatusConstants.NotifiedIncomplete,
                MeasureStatusConstants.NotifiedPending,
                MeasureStatusConstants.WithSupplier
            };

        [Theory]
        [MemberData(nameof(PassCaseStatusIds))]
        public async Task Gbis0106_MrnStatusUserEditable_PassReturnNil(int measureStatus)
        {
            Gbis0106 rule = new Gbis0106(_loggerMock.Object)
            {
                Model = new MeasureModel()
                {
                    MeasureStatusId = measureStatus,
                    MeasureReferenceNumber = "BGT0123456789",
                }
            };

            var result = await rule.InvokeAsync();
            Assert.Null(result);
        }

        public static TheoryData<int> FailureCaseStatusIds =>
            new()
            {
                MeasureStatusConstants.MeasureAwaitingVerification,
                MeasureStatusConstants.OnHold,
                MeasureStatusConstants.BeingAssessed,
                MeasureStatusConstants.InternalQuery,
                MeasureStatusConstants.Approved,
                MeasureStatusConstants.Rejected
            };

        [Theory]
        [MemberData(nameof(FailureCaseStatusIds))]
        public async Task Gbis0106_MrnStatusNonUserEditable_ReturnError(int measureReferenceNumberStatus)
        {
            Gbis0106 rule = new Gbis0106(_loggerMock.Object)
            {
                Model = new MeasureModel()
                {
                    MeasureStatusId = measureReferenceNumberStatus,
                    MeasureReferenceNumber = "BGT0123456789"
                }
            };
            var result = await rule.InvokeAsync();
            Assert.NotNull(result);
        }

        [Theory]
        [MemberData(nameof(FailureCaseStatusIds))]
        public async Task Gbis0106_MrnStatusNonUserEditable_ReturnMrn(int measureReferenceNumberStatus)
        {
            var actual =  await CreateAndInvokeRule(measureReferenceNumberStatus);
            var expected = "BGT0123456789";
            Assert.Equal(actual.MeasureReferenceNumber, expected);
        }

        [Theory]
        [MemberData(nameof(FailureCaseStatusIds))]
        public async Task Gbis0106_MrnStatusNonUserEditable_ReturnsWhatWasAdded( int measureReferenceNumberStatus)
        {
            var actual = await CreateAndInvokeRule(measureReferenceNumberStatus);
            var expected = "BGT0123456789";
            Assert.Equal(actual.WhatWasAddedToTheNotificationTemplate, expected);
        }

        [Theory]
        [MemberData(nameof(FailureCaseStatusIds))]
        public async Task Gbis0106_MrnStatusNonUserEditable_ReturnsTestNumber(int measureReferenceNumberStatus)
        {
            var actual = await CreateAndInvokeRule(measureReferenceNumberStatus);
            var expected = "GBIS0106";
            Assert.Equal(actual.TestNumber, expected);
        }


        private async Task<StageValidationError> CreateAndInvokeRule(int measureReferenceNumberStatus)
        {
            Gbis0106 rule = new Gbis0106(_loggerMock.Object)
            {
                Model = new MeasureModel()
                {
                    MeasureReferenceNumber = "BGT0123456789",
                    MeasureStatusId = measureReferenceNumberStatus
                }
            };

            var result = await rule.InvokeAsync();
            var error = ((IEnumerable<StageValidationError>)result.Result).First();
            return error;
        }
    }
}
