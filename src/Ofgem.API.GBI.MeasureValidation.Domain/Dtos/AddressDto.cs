namespace Ofgem.API.GBI.MeasureValidation.Domain.Dtos;

public class AddressDto
{
    public string? BuildingNumber { get; set; }
    public string? BuildingName { get; set; }
    public string? FlatNameNumber { get; set; }
    public string? StreetName { get; set; }
    public string? Town { get; set; }
    public string? PostCode { get; set; }
}