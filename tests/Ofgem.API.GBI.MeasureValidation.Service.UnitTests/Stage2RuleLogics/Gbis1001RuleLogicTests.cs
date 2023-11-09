using Microsoft.Extensions.Options;
using Ofgem.API.GBI.MeasureValidation.Application.Common;
using Ofgem.API.GBI.MeasureValidation.Domain.Dtos;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Constants;
using Ofgem.API.GBI.MeasureValidation.Service.Stage2RuleLogics;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage2RuleLogics
{
    public class Gbis1001RuleLogicTests
    {
        private readonly IOptions<SchemeDetailsOptions> _mockSchemeDetailsOptions = Options.Create(new SchemeDetailsOptions()
        {
            FinalNotificationDate = new DateTime(2026, 6, 30, 23, 59, 59),
        });

        [Theory]
        [MemberData(nameof(PassCaseData))]
        public void Gbis1001RuleLogic_GivenValidData_ReturnsNoError(string purposeOfNotification, DateTime createdDate,
            DateTime threeMonthEndDate)
        {
            
            var measureModel = new MeasureModel()
            {
                PurposeOfNotification = purposeOfNotification,
                CreatedDate = createdDate,
                FivePercentExtensionDto = new FivePercentExtensionQuotaDto()
                {
                    ThreeMonthEndDate = threeMonthEndDate
                }
            };
            var rule = new Gbis1001RuleLogic(_mockSchemeDetailsOptions);
            var result = rule.FailureCondition(measureModel);
            Assert.False(result);
        }

        [Theory]
        [MemberData(nameof(FailCaseData))]
        public void Gbis1001RuleLogic_GivenInvalidData_ReturnsError(string purposeOfNotification, DateTime createdDate,
            DateTime threeMonthEndDate)
        {

            var measureModel = new MeasureModel()
            {
                PurposeOfNotification = purposeOfNotification,
                CreatedDate = createdDate,
                FivePercentExtensionDto = new FivePercentExtensionQuotaDto()
                {
                    ThreeMonthEndDate = threeMonthEndDate
                }
            };
            var rule = new Gbis1001RuleLogic(_mockSchemeDetailsOptions);
            var result = rule.FailureCondition(measureModel);
            Assert.True(result);
        }

        public static TheoryData<string, DateTime, DateTime> PassCaseData => new TheoryData<string, DateTime, DateTime>
        {
            { PurposeOfNotificationConstants.EditedNotification, new DateTime(2024, 01, 31), new DateTime(2023, 12, 31) },
            { PurposeOfNotificationConstants.AutomaticLateExtension, new DateTime(2023, 10, 01), new DateTime(2023, 12, 31) },
            { PurposeOfNotificationConstants.AutomaticLateExtension, new DateTime(2023, 12, 31), new DateTime(2023, 12, 31) }
        };

        public static TheoryData<string, DateTime, DateTime> FailCaseData => new TheoryData<string, DateTime, DateTime>
        {
            { PurposeOfNotificationConstants.AutomaticLateExtension, new DateTime(2024, 01, 31), new DateTime(2023, 12, 31) },
            { PurposeOfNotificationConstants.AutomaticLateExtension, new DateTime(2024, 10, 01), new DateTime(2023, 12, 31) },
            { PurposeOfNotificationConstants.AutomaticLateExtension, new DateTime(2026, 07, 01), new DateTime(2026, 08, 01) }
        };

    }
}
