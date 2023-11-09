using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;
using Ofgem.API.GBI.MeasureValidation.Application.Extensions;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Services;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Repositories;
using Ofgem.Database.GBI.Measures.Domain.Entities;
using Ofgem.Database.GBI.Measures.Domain.Persistence;

namespace OfGem.API.GBI.MeasureValidation.Infrastructure.UnitTests.Repositories;

public class GetExistingMeasureDataTests
{

    [Theory]
    [MemberData(nameof(MeasureRefNumsCaseInsensitiveSearchData))]
    public async void CaseInsensitiveGetExistingMeasureData_Returns_CaseInsensitiveResult(List<Measure> dbMeasures, List<string> mrns)
    {

        // Arrange
        var queryableMeasures = dbMeasures.AsQueryable();
        var mockSet = queryableMeasures.BuildMockDbSet();

        var mockContext = new Mock<MeasuresDbContext>(new DbContextOptions<MeasuresDbContext>());
        mockContext.Setup(m => m.Measures).Returns(mockSet.Object);

        var mapperMock = new Mock<IMapper>();
        var loggerMock = new Mock<ILogger<MeasureRepository>>();
        var timeProviderMock = new Mock<TimeProvider>();

        var repository = new MeasureRepository(loggerMock.Object, mockContext.Object, mapperMock.Object, timeProviderMock.Object);

        // simulates PreProcessingService.GetExistingSupplierReferencesAsync()
        for (int i = 0; i < mrns.Count; i++)
        {
            mrns[i] = mrns[i].ToUpperInvariant();
        }

        // Act
        var result = await repository.GetExistingMeasureData(mrns);

        // Assert
        var existingMeasureDetailsForMeasureModels = result.ToList();
        Assert.Equal(3, existingMeasureDetailsForMeasureModels.Count);

        foreach (string mrn in mrns)
        {
            var measureData = existingMeasureDetailsForMeasureModels.FirstOrDefault(a => a.MeasureReferenceNumber.CaseInsensitiveEquals(mrn));
            Assert.NotNull(measureData);
            Assert.False(string.IsNullOrEmpty(measureData.ExistingSupplierReference)); 
        }

    }

    [Theory]
    [MemberData(nameof(MeasureRefNumsWithStatusIds))]
    public async void GetExistingMeasureData_GivenMeasuresWithMatchingRecordsinDb_ReturnsStatusIds(List<Measure>dbMeasures, List<string>mrns, List<int> expectedStatusIds)
    {
        var mockMeasuresData = dbMeasures.AsQueryable().BuildMockDbSet();
        var mockContext = new Mock<MeasuresDbContext>(new DbContextOptions<MeasuresDbContext>());
        mockContext.Setup(m => m.Measures).Returns(mockMeasuresData.Object);

        var mapperMock = new Mock<IMapper>();
        var loggerMock = new Mock<ILogger<MeasureRepository>>();
        var timeProviderMock = new Mock<TimeProvider>();
        var repository = new MeasureRepository(loggerMock.Object, mockContext.Object, mapperMock.Object, timeProviderMock.Object);

        for (var i = 0; i < mrns.Count; i++)
        {
            mrns[i] = mrns[i].ToUpperInvariant();
        }

        // Act
        var result = await repository.GetExistingMeasureData(mrns);
        var existingMeasureDetailsForMeasureModels = result.ToList();
        for (var i = 0; i < mrns.Count; i++)
        {
            var measureData = existingMeasureDetailsForMeasureModels.FirstOrDefault(a => a.MeasureReferenceNumber.CaseInsensitiveEquals(mrns[i]));
            var actualMeasureStatusId = measureData?.MeasureStatusId;
            //Assert
            Assert.Equal(expectedStatusIds[i], actualMeasureStatusId);
        }
    }


    public static TheoryData<IEnumerable<Measure>, IEnumerable<string>> MeasureRefNumsCaseInsensitiveSearchData()
    {
        var dbMeasures = new List<Measure> 
        {
            new() { MeasureReferenceNumber = "FOX9899001", SupplierReference = "XEN08053154G" },
            new() { MeasureReferenceNumber = "FOX9899002", SupplierReference = "SSESC288231G" },
            new() { MeasureReferenceNumber = "FOX9899003", SupplierReference = "AVR09174794E" }
        };

        var measureRefNums = new List<string>
        {
            "FOX9899001",
            "Fox9899002",
            "fox9899003",
        };

        return new TheoryData<IEnumerable<Measure>, IEnumerable<string>>
        {
            { dbMeasures, measureRefNums }
        };

    }

    public static TheoryData<IEnumerable<Measure>, IEnumerable<string>, IEnumerable<int>> MeasureRefNumsWithStatusIds()
    {
        var dbMeasures = new List<Measure>
        {
            new() { MeasureReferenceNumber = "FOX0123456781", SupplierReference = "XEN08053154G", MeasureStatusId = 1 },
            new() { MeasureReferenceNumber = "FOX0123456782", SupplierReference = "SSESC288231G", MeasureStatusId = 2 },
            new() { MeasureReferenceNumber = "FOX0123456783", SupplierReference = "", MeasureStatusId = 3 }
        };

        var measureRefNums = new List<string>
        {
           "FOX0123456781",
           "FOX0123456782",
           "FOX0123456783",
        };

        var statusIds = new List<int>
        { 1, 2, 3 };


        return new TheoryData<IEnumerable<Measure>, IEnumerable<string>, IEnumerable<int>>
        {
            { dbMeasures, measureRefNums, statusIds }
        };

    }

    //[Theory]
    //[MemberData(nameof(MeasureRefNumsWithoutExsistingSupplierRefData))]
    //public async void NoExsistingSupplierRefsForMeasureRefNums_Returns_EmptyList(List<Measure> dbMeasures, List<string> mrns)
    //{
    //    // Arrange 
    //    var queryableMeasures = dbMeasures.AsQueryable();
    //    var mockSet = queryableMeasures.BuildMockDbSet();

    //    var mockContext = new Mock<MeasuresDbContext>(new DbContextOptions<MeasuresDbContext>());
    //    mockContext.Setup(m => m.Measures).Returns(mockSet.Object);

    //    var mapperMock = new Mock<IMapper>();
    //    var loggerMock = new Mock<ILogger<MeasureRepository>>();

    //    var repository = new MeasureRepository(loggerMock.Object, mockContext.Object, mapperMock.Object);

    //    // Act
    //    var result = await repository.GetExistingMeasureData(mrns);

    //    // Assert
    //    Assert.Empty(result);

    //}

    public static TheoryData<IEnumerable<Measure>, IEnumerable<string>> MeasureRefNumsWithoutExsistingSupplierRefData()
    {
        var dbMeasures = new List<Measure>
        {
            new Measure { MeasureReferenceNumber = "FOX9899006" },
            new Measure { MeasureReferenceNumber = "FOX9899007" }
        };

        var measureRefNums = new List<string>
        {
            "FOX9899006",
            "FOX9899007",
        };

        return new TheoryData<IEnumerable<Measure>, IEnumerable<string>>
        {
            { dbMeasures, measureRefNums }
        };

    }

}