using Microsoft.Extensions.Logging;
using Moq;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage1Rules;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage1Rules;

public class Gbis0116Tests
{
    private readonly Mock<ILogger<Gbis0116>> _loggerMock;
    private IEnumerable<SupplierLicenceResponse> _supplierLicences;

    public Gbis0116Tests()
    {
        _loggerMock = new Mock<ILogger<Gbis0116>>();
        _supplierLicences = new List<SupplierLicenceResponse>()
        {
            new SupplierLicenceResponse() { SupplierLicenceReference = "EON1234332G", LicenceName = "TEST UK Gas Limited", LicenceNo = "5432332", SupplierName = "TST" } ,
            new SupplierLicenceResponse() { SupplierLicenceReference = "EON9934332G", LicenceName = "TEST UK Gas Limited", LicenceNo = "5436332", SupplierName = "TST" }
        };
    }

    [Theory]
    [InlineData("EON1234332G")]
    [InlineData("EON9934332G")]
    public async void Gbis0116_Pass_ReturnsNil(string supplierReference)
    {
        // given
        Gbis0116 rule = CreateGbis0116Rule(supplierReference);
        // when
        var result = await rule.InvokeAsync();
        // then
        Assert.Null(result);

    }

    [Theory]
    [InlineData("00")]
    [InlineData("EON3334332G")]
    public async void Gbis0116_Fail_ReturnsError(string supplierReference)
    {
        Gbis0116 rule = CreateGbis0116Rule(supplierReference);
        // when
        var result = await rule.InvokeAsync();
        var error = ((IEnumerable<StageValidationError>)result.Result).First();
        // then
        Assert.NotNull(error);
        Assert.Equal("GBIS0116", error.TestNumber);
        Assert.Equal(supplierReference, error.WhatWasAddedToTheNotificationTemplate);
    }

    private Gbis0116 CreateGbis0116Rule(string supplierReference, bool hasSupplierLicences = true)
    {
        // given
        return new(_loggerMock.Object)
        {
            Model = new MeasureModel()
            {
                SupplierReference = supplierReference,
                SupplierLicences = hasSupplierLicences ? _supplierLicences : new List<SupplierLicenceResponse>(),
            }
        };
    }
}

