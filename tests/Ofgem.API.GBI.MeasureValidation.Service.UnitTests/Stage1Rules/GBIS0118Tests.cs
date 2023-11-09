using Microsoft.Extensions.Logging;
using Moq;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage1Rules;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage1Rules;

public class Gbis0118Tests
{
    private readonly Mock<ILogger<Gbis0118>> _loggerMock;

    public Gbis0118Tests()
    {
        _loggerMock = new Mock<ILogger<Gbis0118>>();
    }

    [Theory]
    [MemberData(nameof(SupplierReferencePassData))]
    public async void Gbis0118_Pass_ReturnsNil(string supplierReference, string existingSupplierReference)
    {
        // Arrange 
        Gbis0118 rule = new Gbis0118(_loggerMock.Object)
        {
            Model = new MeasureModel()
            {
                SupplierReference = supplierReference,
                ExistingSupplierReference = existingSupplierReference
            }
        };

        // Act
        var result = await rule.InvokeAsync();

        // Assert
        Assert.Null(result);

    }

    [Theory]
    [MemberData(nameof(SupplierReferenceErrorData))]
    public async void Gbis0118_Fail_ReturnsError(string supplierReference, string existingSupplierReference)
    {
        // Arrange 
        string measureReferenceNumber = "ATX1234567";

        Gbis0118 rule = new Gbis0118(_loggerMock.Object)
        {
            Model = new MeasureModel()
            {
                MeasureReferenceNumber = measureReferenceNumber,
                SupplierReference = supplierReference,
                ExistingSupplierReference = existingSupplierReference
            }
        };

        // Act
        var result = await rule.InvokeAsync();
        var error = ((IEnumerable<StageValidationError>)result.Result).First();

        // Assert
        Assert.NotNull(error);
        Assert.Equal(measureReferenceNumber, error.MeasureReferenceNumber);
        Assert.Equal("GBIS0118", error.TestNumber);
        Assert.Equal(supplierReference, error.WhatWasAddedToTheNotificationTemplate);

    }

    public static TheoryData<string?, string?> SupplierReferencePassData()
    {
        return new TheoryData<string?, string?>
            {
                { null, null },
                { null, "XEN08053202G" },
                { "XEN08053202G", null },
                { "", "" },
                { "mrn", "mrn" },
                { "XEN08053202G",   "XEN08053202G" },
                { "xen08053202g",   "XEN08053202G" },
                { " XEN08053202G",  "XEN08053202G" },
                { "XEN08053202G  ", "XEN08053202G" },
                { " XEN08053202G ", "XEN08053202G" },
                { "XEN08053202G",   "xen08053202g" },
                { "XEN08053202G",   " XEN08053202G " },
                { " XEN08053202G ", " XEN08053202G " },
                { " XEN08053202G ", "XEN08053202G " },
                { "XEN08053202G",   "XEN08053202G\n" },
                { "XEN08053202G\n", "XEN08053202G" },
                { "XEN08053202G\n", "XEN08053202G\n" },
                { "FOX09689035G",   "FOX09689035G" },
            };
    }

    public static TheoryData<string, string> SupplierReferenceErrorData()
    {
        return new TheoryData<string, string>
            {
                { "", "mrn" },
                { "FOX09689035G",   "" },
                { "\n",   "XEN08053202G" },
                { "FOX09689035G",   "XEN08053202G" },
                { " FOX09689035G ", "XEN08053202G" },
                { "ssesc288230g",   "XEN08053202G" },
                { "ssesc288230g\n", "XEN08053202G" },
                { "XEN08053202G�",  "XEN08053202G" },
                { "XEN08053202G",   "XEN08053202G�" },
            };
    }

}
