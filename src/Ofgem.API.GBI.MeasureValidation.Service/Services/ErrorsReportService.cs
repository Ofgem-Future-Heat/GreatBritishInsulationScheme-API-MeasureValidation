using AutoMapper;
using CsvHelper;
using Microsoft.Extensions.Logging;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Infrastructure;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Services;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using System.Globalization;

namespace Ofgem.API.GBI.MeasureValidation.Service.Services
{
    public class ErrorsReportService : IErrorsReportService
    {
        private readonly ILogger<ErrorsReportService> _logger;
        private readonly IErrorReportRepository _errorReportRepository;
        private readonly IMapper _mapper;

        public ErrorsReportService(ILogger<ErrorsReportService> logger, IErrorReportRepository errorReportRepository, IMapper mapper)
        {
            _logger = logger;
            _errorReportRepository = errorReportRepository;
            _mapper = mapper;
        }

        public async Task<string> GetErrorsReport(Guid documentId, string? stage)
        {
            try
            {
                var result = await _errorReportRepository.GetValidationErrors(documentId, stage);
                var errors = _mapper.Map<List<ErrorReportResponse>>(result);

                string errorReport;
                using (var writer = new StringWriter())
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteField(($"{stage} Errors").ToUpper());
                    csv.NextRecord();
                    csv.WriteField(null);
                    csv.NextRecord();
                    if (errors != null)
                    {
                        csv.WriteRecords(errors);
                    }
                    errorReport = writer.ToString();
                }

                return errorReport.Trim();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while generating report for {documentId}. {exMessage}", documentId, ex.Message);
                throw;
            }
        } 
    }
}
