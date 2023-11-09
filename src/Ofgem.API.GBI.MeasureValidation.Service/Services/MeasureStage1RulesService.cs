using DotNetRuleEngine;
using DotNetRuleEngine.Extensions;
using DotNetRuleEngine.Interface;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Services;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;

namespace Ofgem.API.GBI.MeasureValidation.Service.Services
{
    public class MeasureStage1RulesService : IMeasureStage1RulesService
    {
        private readonly IEnumerable<IGeneralRule<MeasureModel>> _stage1Rules;
        private readonly TimeProvider _timeProvider;

        public MeasureStage1RulesService(IEnumerable<IGeneralRule<MeasureModel>> stage1Rules, TimeProvider timeProvider)
        {
             _stage1Rules = stage1Rules;
            _timeProvider = timeProvider;
        }

        public async Task<Stage1ValidationResultModel> ValidateMeasures(IEnumerable<MeasureModel> measures)
        {
            var currentDateTime = _timeProvider.GetUtcNow().DateTime;
            var measureModels = measures.ToList();
            var stage1Result = new Stage1ValidationResultModel
            {
                TotalRowCount = measureModels.Count,
                FileName = measureModels.FirstOrDefault()?.FileName,
                CreatedDate = currentDateTime,
                PassedMeasures = new List<MeasureModel>()
            };

            foreach (var measure in measureModels)
            {
                var ruleResults = await RuleEngine<MeasureModel>.GetInstance(measure).ApplyRules(_stage1Rules.ToArray()).ExecuteAsync();

                var ruleResultsList = ruleResults.ToList();
                if (ruleResultsList.Any())
                {
                    foreach (var measureValidationErrors in ruleResultsList.Select(item => item.Result as IList<StageValidationError>))
                    {
                        stage1Result.FailedMeasureErrors.AddRange(measureValidationErrors!);
                        stage1Result.FailedMeasureErrors.ForEach(x =>
                        {
                            x.ErrorStage = "Stage 1";
                            x.MeasureStatus = "Failed Notification";
                            x.DocumentId = measure.DocumentId;
                            x.CreatedDate = currentDateTime;
                            x.SupplierName = measure.SupplierName;
                        });
                    }

                    stage1Result.FailedCount++;
                }
                else
                {
                    stage1Result.SuccessCount++;
                    stage1Result.PassedMeasures.Add(measure);
                }
            }

            return stage1Result;
        }
    }
}
