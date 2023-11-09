using DotNetRuleEngine.Interface;
using DotNetRuleEngine;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Services;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using DotNetRuleEngine.Extensions;

namespace Ofgem.API.GBI.MeasureValidation.Service.Services
{
    public class MeasureStage2RulesService : IMeasureStage2RulesService
    {
        private readonly IEnumerable<IGeneralRule<MeasureModel>> _stage2Rules;
        private readonly TimeProvider _timeProvider;
        public MeasureStage2RulesService(IEnumerable<IGeneralRule<MeasureModel>> stage2Rules, TimeProvider timeProvider)
        {
            _stage2Rules = stage2Rules;
            _timeProvider = timeProvider;
        }

        public async Task<Stage2ValidationResultModel> ValidateMeasures(IEnumerable<MeasureModel> measures)
        {
            var currentDateTime = _timeProvider.GetUtcNow().DateTime;
            var measureModels = measures.ToList();
            var stage2Result = new Stage2ValidationResultModel
            {
                FileName = measureModels.FirstOrDefault()?.FileName,
                CreatedDate = currentDateTime,
                PassedMeasures = new List<MeasureModel>()
            };

            foreach (var measure in measureModels)
            {
                var ruleResults = await RuleEngine<MeasureModel>.GetInstance(measure).ApplyRules(_stage2Rules.ToArray()).ExecuteAsync();

                var ruleResultList = ruleResults.ToList();
                if (ruleResultList.Any())
                {
                    foreach (var item in ruleResultList)
                    {
                        var measureValidationErrors = item.Result as IList<StageValidationError>;
                        stage2Result.FailedMeasureErrors.AddRange(measureValidationErrors!);
                        stage2Result.FailedMeasureErrors.ForEach(x =>
                        {
                            x.ErrorStage = "Stage 2";
                            x.MeasureStatus = "Notified Incomplete";
                            x.DocumentId = measure.DocumentId;
                            x.CreatedDate = currentDateTime;
                            x.SupplierName = measure.SupplierName;
                        });
                    }
                }
                else
                {
                    stage2Result.PassedMeasures.Add(measure);
                }
            }

            return stage2Result;
        }
    }
}
