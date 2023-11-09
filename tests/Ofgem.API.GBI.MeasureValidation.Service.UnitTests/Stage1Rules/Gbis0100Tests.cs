using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage1Rules;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage1Rules
{
    public class Gbis0100Tests
    {
        [Theory]
        [InlineData("new notification")]
        [InlineData("Edited Notification")]
        [InlineData("extended Notification")]
        [InlineData("Automatic late extension")]
        public async Task Gbis0100_Pass(string purposeOfNotification)
        {
            var rule = new Gbis0100
            {
                Model = new MeasureModel()
                {
                    MeasureReferenceNumber = "BGT1234567",
                    PurposeOfNotification = purposeOfNotification,
                    ReferenceDataDetails = new ReferenceDataDetails
                    {
                        PurposeOfNotificationsList = GetPurposeOfNotifications()
                    }
                }
            };

            var result = await rule.InvokeAsync();
            Assert.Null(result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("xyz notification")]
        public async Task Gbis0100_Fail(string purposeOfNotification)
        {
            var rule = new Gbis0100
            {
                Model = new MeasureModel()
                {
                    MeasureReferenceNumber = "BGT1234567",
                    PurposeOfNotification = purposeOfNotification,
                    ReferenceDataDetails = new ReferenceDataDetails
                    {
                        PurposeOfNotificationsList = GetPurposeOfNotifications()
                    }
                }
            };

            var result = await rule.InvokeAsync();
            var error = ((IEnumerable<StageValidationError>)result.Result).First();

            Assert.Single((IEnumerable<StageValidationError>)result.Result);
            Assert.Equal("BGT1234567", error.MeasureReferenceNumber);
            Assert.Equal("GBIS0100", error.TestNumber);
            Assert.Equal(purposeOfNotification, error.WhatWasAddedToTheNotificationTemplate);
        }

        private static IEnumerable<string> GetPurposeOfNotifications()
        {
            return new List<string>
            {
                "New Notification",
                "Edited Notification",
                "Extended Notification",
                "Automatic Late Extension"
            };
        }
    }
}