using Microsoft.EntityFrameworkCore;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Infrastructure;
using Ofgem.Database.GBI.Measures.Domain.Entities;
using Ofgem.Database.GBI.Measures.Domain.Persistence;

namespace Ofgem.API.GBI.MeasureValidation.Infrastructure.Repositories
{
    public class ErrorReportRepository : IErrorReportRepository
    {
        private readonly MeasuresDbContext _context;
        public ErrorReportRepository(MeasuresDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ValidationError>> GetValidationErrors(Guid documentId, string? stage)
        {
            var errors = await _context.ValidationErrors.Where(e => e.DocumentId == documentId && e.ErrorStage == stage)
                .Include(e => e.ErrorMessage).ToListAsync();
            
            return errors.AsEnumerable();
        }
    }
}
