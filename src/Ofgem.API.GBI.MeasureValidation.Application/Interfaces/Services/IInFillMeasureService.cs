namespace Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Services;

public interface IInFillMeasureService
{
    Task<bool> IsInfillMeasureAssigned(string measureReferenceNumber, string infillMeasureReferenceNumber);
}