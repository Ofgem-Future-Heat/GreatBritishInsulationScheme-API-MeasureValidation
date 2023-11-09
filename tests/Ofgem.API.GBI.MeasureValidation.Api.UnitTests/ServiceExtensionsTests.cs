using DotNetRuleEngine;
using DotNetRuleEngine.Interface;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Ofgem.API.GBI.MeasureValidation.Api.Extensions;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Services;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Services;

namespace Ofgem.API.GBI.MeasureValidation.Api.UnitTests
{
    public class ServiceExtensionsTests
    {
        private readonly Func<IServiceProvider, Type, IGeneralRule<MeasureModel>> _ruleCreatorFunction;

        public ServiceExtensionsTests()
        {
            var generalRuleMock = new Mock<IGeneralRule<MeasureModel>>();
            _ruleCreatorFunction = (_, _) => generalRuleMock.Object;
        }

        private class TestRule : RuleAsync<MeasureModel>
        {
            public override Task<IRuleResult> InvokeAsync()
            {
                throw new NotImplementedException();
            }
        }

        [Fact]
        public void RegisterMeasureStageServices_NullRulesParameter_ThrowsArgumentNullException()
        {
            // Arrange
            var servicesMock = new Mock<IServiceCollection>();

            // Act
            var exception = Record.Exception(() =>
                ServiceExtensions
                    .RegisterMeasureStageServices<IMeasureStage1RulesService, MeasureStage1RulesService,
                        IGeneralRule<MeasureModel>>(servicesMock.Object, null!, _ruleCreatorFunction));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<ArgumentNullException>(exception);
        }

        [Theory]
        [MemberData(nameof(AllRulesAreIncorrectRuleTypesTestData))]
        [MemberData(nameof(SomeRulesAreIncorrectRuleTypesTestData))]
        public void RegisterMeasureStageServices_IncorrectTypes_ThrowsNotSupportedException(IEnumerable<Type> types)
        {
            // Arrange
            var servicesMock = new Mock<IServiceCollection>();

            // Act
            var exception = Record.Exception(() =>
                ServiceExtensions
                    .RegisterMeasureStageServices<IMeasureStage1RulesService, MeasureStage1RulesService,
                        IGeneralRule<MeasureModel>>(servicesMock.Object, types, _ruleCreatorFunction));

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<NotSupportedException>(exception);
        }

        public static TheoryData<IEnumerable<Type>> AllRulesAreIncorrectRuleTypesTestData()
        {
            return new TheoryData<IEnumerable<Type>>()
            {
                new[] { typeof(ServiceExtensionsTests) },
                new[] { typeof(IGeneralRule<ServiceExtensionsTests>) },
                new[] { typeof(ServiceExtensionsTests), typeof(IGeneralRule<ServiceExtensionsTests>) },
            };
        }

        public static TheoryData<IEnumerable<Type>> SomeRulesAreIncorrectRuleTypesTestData()
        {
            return new TheoryData<IEnumerable<Type>>()
            {
                new[] { typeof(TestRule), typeof(IGeneralRule<ServiceExtensionsTests>) },
            };
        }

        [Fact]
        public void RegisterMeasureStageServices_CorrectRuleTypes_DoesNotThrowNotSupportedException()
        {
            // Arrange
            var servicesMock = new Mock<IServiceCollection>();
            var types = new[] { typeof(TestRule) };

            // Act
            var exception = Record.Exception(() =>
                ServiceExtensions
                    .RegisterMeasureStageServices<IMeasureStage1RulesService, MeasureStage1RulesService,
                        IGeneralRule<MeasureModel>>(servicesMock.Object, types, _ruleCreatorFunction));

            // Assert
            Assert.Null(exception);
        }
    }
}
