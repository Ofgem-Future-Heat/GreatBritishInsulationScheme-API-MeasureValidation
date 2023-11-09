using Microsoft.Extensions.Logging;
using Moq;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Service.Stage1Rules;

namespace Ofgem.API.GBI.MeasureValidation.Service.UnitTests.Stage1Rules;

public class Gbis0117Tests
{
    private readonly Mock<ILogger<Gbis0117>> _loggerMock;
    private IEnumerable<SupplierLicenceResponse> _supplierLicences;

    public Gbis0117Tests()
    {
        _loggerMock = new Mock<ILogger<Gbis0117>>();
        _supplierLicences = new List<SupplierLicenceResponse>()
        {
            new SupplierLicenceResponse() { SupplierLicenceReference = "EON1234332G", LicenceName = "TEST UK Gas Limited", LicenceNo = "5432332", SupplierName = "TST" } ,
            new SupplierLicenceResponse() { SupplierLicenceReference = "EON9934332G", LicenceName = "TEST UK Gas Limited", LicenceNo = "5436332", SupplierName = "BNN" },
            new SupplierLicenceResponse() { SupplierLicenceReference = "EON9934333G", LicenceName = "TEST UK Gas Limited", LicenceNo = "5436333", SupplierName = "BNN" },
            new SupplierLicenceResponse() { SupplierLicenceReference = "EON9934334G", LicenceName = "TEST UK Gas Limited", LicenceNo = "5436334", SupplierName = "BNN" }
        };
    }

    [Theory]
    [InlineData("TST", "EON1234332G")]
    [InlineData("BNN", "EON9934333G")]
    public async void Gbis0117_Pass_ReturnsNil(string supplierName, string supplierReference)
    {
        // given
        Gbis0117 rule = CreateGbis0117(supplierName, supplierReference);
        // when
        var result = await rule.InvokeAsync();
        // then
        Assert.Null(result);

    }

    [Theory]
    [InlineData("00")]
    [InlineData("BMN")]
    [InlineData(null)]
    public async void Gbis0117_Fail_ReturnsError(string supplierName)
    {
        // given
        Gbis0117 rule = CreateGbis0117(supplierName, "ABC09174795G");
        // when
        var result = await rule.InvokeAsync();
        var error = ((IEnumerable<StageValidationError>)result.Result).First();
        // then
        Assert.NotNull(error);
        Assert.Equal("GBIS0117", error.TestNumber);
        Assert.Equal("ABC09174795G", error.WhatWasAddedToTheNotificationTemplate);
    }

    private Gbis0117 CreateGbis0117(string supplierName, string supplierReference, bool hasSupplierLicences = true)
    {
        return new Gbis0117(_loggerMock.Object)
        {
            Model = new MeasureModel()
            {
                SupplierName = supplierName,
                SupplierLicences = hasSupplierLicences ? _supplierLicences : new List<SupplierLicenceResponse>(),
                SupplierReference = supplierReference
            }
        };
    }
}

