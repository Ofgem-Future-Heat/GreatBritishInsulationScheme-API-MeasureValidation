using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using Ofgem.API.GBI.MeasureValidation.Infrastructure.Repositories;
using Ofgem.Database.GBI.Measures.Domain.Entities;
using Ofgem.Database.GBI.Measures.Domain.Persistence;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Services;
using Ofgem.Database.GBI.Measures.Domain.Dtos.MeasureUpload;

namespace Ofgem.API.GBI.MeasureValidation.Infrastructure.UnitTests.Repositories
{
    public class MeasureRepositoryTests
    {
        private const string SupplierName = "SupplierName";
        private const string ErrorStage1 = "Stage1";
        private const string ErrorStage2 = "Stage2";
        private const string ArbitraryTestNumber = "ArbitraryTestNumber";

        private readonly Mock<MeasuresDbContext> _contextMock = new(new DbContextOptions<MeasuresDbContext>());
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<ILogger<MeasureRepository>> _loggerMock = new();
        private readonly Mock<TimeProvider> _timeProviderMock = new();
        private readonly MeasureRepository _repository;

        public MeasureRepositoryTests()
        {
            _repository = new MeasureRepositoryTesting(_loggerMock.Object, _contextMock.Object, _mapperMock.Object, _timeProviderMock.Object);
        }

        [Fact]
        public async Task GetLatestFilesWithErrors_ThrowsException_ExceptionRethrown()
        {
			// Arrange
			_contextMock.Setup(m => m.ValidationErrors).Throws<Exception>();

            // Act
            var act = async () => await _repository.GetLatestFilesWithErrors(SupplierName);

            // Assert
            _ = await Assert.ThrowsAsync<Exception>(act);
        }

        [Fact]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2254:Template should be a static expression", Justification = "Tests do not validate log message")]
        public async Task GetLatestFilesWithErrors_ThrowsException_ErrorLogged()
        {
			// Arrange
			_contextMock.Setup(m => m.ValidationErrors).Throws<Exception>();

            // Act
            var act = async () => await _repository.GetLatestFilesWithErrors(SupplierName);

            // Assert
            _ = await Assert.ThrowsAsync<Exception>(act);
            _loggerMock.VerifyLog(logger => logger.LogError(It.IsAny<string>()), Times.Once);
        }

        [Theory]
        [MemberData(nameof(SingleValidationErrorFound))]
        [MemberData(nameof(NoValidationErrorsFoundForSupplier))]
        [MemberData(nameof(MultipleValidationErrorsAtSingleStageInASingleFile))]
        [MemberData(nameof(AValidationErrorInFiveDifferentFiles))]
        [MemberData(nameof(ValidationErrorsInMoreThanFiveDifferentFiles))]
        [MemberData(nameof(ValidationErrorsAtMoreThanOneStageInSingleFile))]
        [MemberData(nameof(ValidationErrorsAtMoreThanOneStageInMultipleFiles))]
        public async Task GetLatestFilesWithErrors_ValidationErrors_CorrectDataIsReturned(IEnumerable<ValidationError> validationErrors, FilesWithErrorsMetadata expected)
        {
            // Arrange
            var queryableValidationErrors = validationErrors.AsQueryable();
            var mockSet = queryableValidationErrors.BuildMockDbSet();

			_contextMock.Setup(m => m.ValidationErrors).Returns(mockSet.Object);

            // Act
            var filesWithErrorsMetadata = await _repository.GetLatestFilesWithErrors(SupplierName);

            // Assert
            Assert.Equivalent(expected, filesWithErrorsMetadata);
        }

        [Theory]
        [MemberData(nameof(SingleValidationErrorFound))]
        [MemberData(nameof(NoValidationErrorsFoundForSupplier))]
        [MemberData(nameof(MultipleValidationErrorsAtSingleStageInASingleFile))]
        [MemberData(nameof(AValidationErrorInFiveDifferentFiles))]
        [MemberData(nameof(ValidationErrorsInMoreThanFiveDifferentFiles))]
        [MemberData(nameof(ValidationErrorsAtMoreThanOneStageInSingleFile))]
        [MemberData(nameof(ValidationErrorsAtMoreThanOneStageInMultipleFiles))]
        [MemberData(nameof(AValidationErrorInTenDifferentFilesAtMoreThanOneStage))]
        public async Task GetAllFilesWithErrors_ValidationErrors_CorrectDataIsReturned(IEnumerable<ValidationError> validationErrors, FilesWithErrorsMetadata expected )
        {
            // Arrange
            var queryableValidationErrors = validationErrors.AsQueryable();
            var mockSet = queryableValidationErrors.BuildMockDbSet();

			_contextMock.Setup(m => m.ValidationErrors).Returns(mockSet.Object);

            // Act
            var filesWithErrorsMetadata = await _repository.GetAllFilesWithErrors(SupplierName);

            // Assert
            Assert.Equivalent(expected, filesWithErrorsMetadata);
        }

        [Fact]
        public async Task GetTenureTypes_WithData_ReturnsTenureTypes()
        {
            // Arrange
            var tenureTypes = new List<TenureType>
            {
                new()
                {
                    TenureTypeId = 1,
                    Name = "Owner Occupied",
                    SchemeVersion = 1
                }
            }.AsQueryable();

            var mockSet = tenureTypes.BuildMockDbSet();

			_contextMock.Setup(m => m.TenureTypes).Returns(mockSet.Object);

            // Act
            var returnedTenureTypes = await _repository.GetTenureTypes();

            // Assert
            Assert.Single(returnedTenureTypes);
        }

        [Fact]
        public async Task GetInnovationMeasuresAsync_WithData_ReturnsInnovationMeasures()
        {
            // Arrange
            var innovationMeasures = new List<InnovationMeasure>
            {
                new()
                {
                    InnovationMeasureId = 1,
                    MeasureTypeId = 1,
                    InnovationMeasureNumber = "123",
                    MeasureTypeName = "EWI_cavity_0.45_0.21"
                }
            }.AsQueryable();

            var mockSet = innovationMeasures.BuildMockDbSet();

            _contextMock.Setup(m => m.InnovationMeasures).Returns(mockSet.Object);

            // Act
            var returnedInnovationMeasures = await _repository.GetInnovationMeasuresAsync();

            // Assert
            Assert.Single(returnedInnovationMeasures);
        }

        [Fact]
        public async Task GetFlexReferralRoutesAsync_WithData_ReturnsFlexReferralRoutes()
        {
            // Arrange
            var flexReferralRoutes = new List<FlexReferralRoute>
            {
                new()
                {
                    FlexReferralRouteId = 1,
                    Name = "Route 2 Area Validation"
                }
            }.AsQueryable();

            var mockSet = flexReferralRoutes.BuildMockDbSet();

            _contextMock.Setup(m => m.FlexReferralRoutes).Returns(mockSet.Object);

            // Act
            var returnedFlexReferralRoutes = await _repository.GetFlexReferralRoutesAsync();

            // Assert
            Assert.Single(returnedFlexReferralRoutes);
        }

		[Fact]
        public async void GetAssociatedMeasureData_WithMeasureReferenceNumbers_ReturnsAssociatedMeasureData()
        {
            // Arrange
            var measureReferenceNumbers = new List<string> { "ABC0123456781" };

            var measures = new List<Measure>
            {
                new Measure
                {
                    MeasureReferenceNumber = "ABC0123456781",
                    MeasureTypeId = 1,
                    MeasureType = new MeasureType
                    {
                        MeasureTypeId = 1,
                        MeasureCategoryId = 1,
                        Name = "Name"
                    }
				}
            }.AsQueryable();

            var mockSet = measures.BuildMockDbSet();

			_contextMock.Setup(m => m.Measures).Returns(mockSet.Object);

            // Act
            var associatedMeasureData = await _repository.GetAssociatedMeasureData(measureReferenceNumbers);

            // Assert
            Assert.Single(associatedMeasureData);
		}

        [Fact]
		public async void GetAssociatedMeasureData_WithNoMeasureReferenceNumbers_ReturnsEmptyList()
        {
			// Arrange

            // Act
            var associatedMeasureData = await _repository.GetAssociatedMeasureData(new List<string>());

            // Assert
            Assert.Empty(associatedMeasureData);
		}

		public static TheoryData<IEnumerable<ValidationError>, FilesWithErrorsMetadata> SingleValidationErrorFound()
        {
            var documentId = Guid.NewGuid();
            var dateTime = new DateTime(2023, 09, 06, 05, 16, 30, DateTimeKind.Unspecified);

            var validationErrors = new[]
            {
                new ValidationError {
                    TestNumber = ArbitraryTestNumber,
                    CreatedDate = dateTime,
                    DocumentId = documentId,
                    ErrorStage = ErrorStage1,
                    SupplierName = SupplierName,
                },
            };

            var expectedFilesWithErrorsMetadata = new FilesWithErrorsMetadata
            {
                FilesWithErrors = new[] {
                    new FileWithErrorsMetadata
                    {
                        DocumentId = documentId,
                        ErrorStage = ErrorStage1,
                        DateTime = dateTime,
                    }
                }
            };
            
            return new TheoryData<IEnumerable<ValidationError>, FilesWithErrorsMetadata>
            {
                { validationErrors, expectedFilesWithErrorsMetadata }
            };
        }

        public static TheoryData<IEnumerable<ValidationError>, FilesWithErrorsMetadata> NoValidationErrorsFoundForSupplier()
        {
            var documentId = Guid.NewGuid();
            var dateTime = new DateTime(2023, 09, 06, 05, 16, 30, DateTimeKind.Unspecified);

            var validationErrors = new[]
                {
                    new ValidationError {
                        TestNumber = ArbitraryTestNumber,
                        CreatedDate = dateTime,
                        DocumentId = documentId,
                        ErrorStage = ErrorStage1,
                        SupplierName = "SupplierName2",
                    },
                };

            var expectedFilesWithErrorsMetadata = new FilesWithErrorsMetadata
            {
                FilesWithErrors = Array.Empty<FileWithErrorsMetadata>(),
            };

            return new TheoryData<IEnumerable<ValidationError>, FilesWithErrorsMetadata>
            {
                { validationErrors, expectedFilesWithErrorsMetadata }
            };
        }

        public static TheoryData<IEnumerable<ValidationError>, FilesWithErrorsMetadata> AValidationErrorInFiveDifferentFiles()
        {
            var documentDetails = new[]
            {
                new { DateTime = new DateTime(2023, 09, 06, 05, 37, 10, DateTimeKind.Unspecified), DocumentId = Guid.NewGuid() },
                new { DateTime = new DateTime(2023, 08, 16, 18, 04, 20, DateTimeKind.Unspecified), DocumentId = Guid.NewGuid() },
                new { DateTime = new DateTime(2023, 09, 01, 12, 23, 30, DateTimeKind.Unspecified), DocumentId = Guid.NewGuid() },
                new { DateTime = new DateTime(2023, 07, 12, 14, 42, 40, DateTimeKind.Unspecified), DocumentId = Guid.NewGuid() },
                new { DateTime = new DateTime(2023, 10, 25, 23, 16, 50, DateTimeKind.Unspecified), DocumentId = Guid.NewGuid() },
            };

            var validationErrors = documentDetails.Select(x => new ValidationError
            {
                TestNumber = ArbitraryTestNumber,
                CreatedDate = x.DateTime,
                DocumentId = x.DocumentId,
                ErrorStage = ErrorStage1,
                SupplierName = SupplierName,
            });

            var expectedFilesWithErrorsMetadata = new FilesWithErrorsMetadata
            {
                FilesWithErrors = documentDetails.Select(x => new FileWithErrorsMetadata
                {
                    DocumentId = x.DocumentId,
                    ErrorStage = ErrorStage1,
                    DateTime = x.DateTime,
                })
            };

            return new TheoryData<IEnumerable<ValidationError>, FilesWithErrorsMetadata>
            {
                { validationErrors, expectedFilesWithErrorsMetadata }
            };
        }
        public static TheoryData<IEnumerable<ValidationError>, FilesWithErrorsMetadata> AValidationErrorInTenDifferentFilesAtMoreThanOneStage()
        {
            var documentDetails = new[]
            {
                new { DateTime = new DateTime(2023, 09, 06, 05, 37, 10, DateTimeKind.Unspecified), DocumentId = Guid.NewGuid() },
                new { DateTime = new DateTime(2023, 08, 16, 18, 04, 20, DateTimeKind.Unspecified), DocumentId = Guid.NewGuid() },
                new { DateTime = new DateTime(2023, 09, 01, 12, 23, 30, DateTimeKind.Unspecified), DocumentId = Guid.NewGuid() },
                new { DateTime = new DateTime(2023, 07, 12, 14, 42, 40, DateTimeKind.Unspecified), DocumentId = Guid.NewGuid() },
                new { DateTime = new DateTime(2023, 10, 25, 23, 16, 50, DateTimeKind.Unspecified), DocumentId = Guid.NewGuid() },
                new { DateTime = new DateTime(2023, 09, 06, 05, 37, 11, DateTimeKind.Unspecified), DocumentId = Guid.NewGuid() },
                new { DateTime = new DateTime(2023, 08, 16, 18, 09, 20, DateTimeKind.Unspecified), DocumentId = Guid.NewGuid() },
                new { DateTime = new DateTime(2023, 09, 11, 12, 23, 30, DateTimeKind.Unspecified), DocumentId = Guid.NewGuid() },
                new { DateTime = new DateTime(2023, 07, 13, 14, 41, 40, DateTimeKind.Unspecified), DocumentId = Guid.NewGuid() },
                new { DateTime = new DateTime(2023, 10, 25, 13, 16, 10, DateTimeKind.Unspecified), DocumentId = Guid.NewGuid() },
            };

            var validationErrors = documentDetails.SelectMany(x => new[] {
                new ValidationError {
                    TestNumber = ArbitraryTestNumber,
                    CreatedDate = x.DateTime,
                    DocumentId = x.DocumentId,
                    ErrorStage = ErrorStage1,
                    SupplierName = SupplierName,
                },
                new ValidationError {
                    TestNumber = ArbitraryTestNumber,
                    CreatedDate = x.DateTime,
                    DocumentId = x.DocumentId,
                    ErrorStage = ErrorStage2,
                    SupplierName = SupplierName,
                }
            });

            var expectedFilesWithErrorsMetadata = new FilesWithErrorsMetadata
            {
                FilesWithErrors = documentDetails.SelectMany(x => new[] {
                    new FileWithErrorsMetadata
                    {
                        DocumentId = x.DocumentId,
                        ErrorStage = ErrorStage1,
                        DateTime = x.DateTime,
                    },
                    new FileWithErrorsMetadata
                    {
                        DocumentId = x.DocumentId,
                        ErrorStage = ErrorStage2,
                        DateTime = x.DateTime,
                    },
                })
            };

            return new TheoryData<IEnumerable<ValidationError>, FilesWithErrorsMetadata>
            {
                { validationErrors, expectedFilesWithErrorsMetadata }
            };
        }

        public static TheoryData<IEnumerable<ValidationError>, FilesWithErrorsMetadata> MultipleValidationErrorsAtSingleStageInASingleFile()
        {
            var documentId = Guid.NewGuid();
            var dateTime = new DateTime(2023, 09, 06, 05, 16, 30, DateTimeKind.Unspecified);

            var validationErrors = Enumerable.Range(0, 5).Select(x => new ValidationError {
                    TestNumber = ArbitraryTestNumber + x,
                    CreatedDate = dateTime,
                    DocumentId = documentId,
                    ErrorStage = ErrorStage1,
                    SupplierName = SupplierName,
            });

            var expectedFilesWithErrorsMetadata = new FilesWithErrorsMetadata
            {
                FilesWithErrors = new[]
                {
                    new FileWithErrorsMetadata
                    {
                        DocumentId = documentId,
                        ErrorStage = ErrorStage1,
                        DateTime = dateTime,
                    }
                }
            };

            return new TheoryData<IEnumerable<ValidationError>, FilesWithErrorsMetadata>
            {
                { validationErrors, expectedFilesWithErrorsMetadata },
            };
        }

        public static TheoryData<IEnumerable<ValidationError>, FilesWithErrorsMetadata> ValidationErrorsInMoreThanFiveDifferentFiles()
        {
            var oldestValidationErrorData = new { DateTime = new DateTime(2023, 01, 25, 23, 16, 50, DateTimeKind.Unspecified), DocumentId = Guid.NewGuid() };
            var documentDetails = new[]
            {
                oldestValidationErrorData,
                new { DateTime = new DateTime(2023, 09, 06, 05, 37, 10, DateTimeKind.Unspecified), DocumentId = Guid.NewGuid() },
                new { DateTime = new DateTime(2023, 08, 16, 18, 04, 20, DateTimeKind.Unspecified), DocumentId = Guid.NewGuid() },
                new { DateTime = new DateTime(2023, 09, 01, 12, 23, 30, DateTimeKind.Unspecified), DocumentId = Guid.NewGuid() },
                new { DateTime = new DateTime(2023, 07, 12, 14, 42, 40, DateTimeKind.Unspecified), DocumentId = Guid.NewGuid() },
                new { DateTime = new DateTime(2023, 10, 25, 23, 16, 50, DateTimeKind.Unspecified), DocumentId = Guid.NewGuid() },
            };

            var validationErrors = documentDetails.Select(x => new ValidationError {
                TestNumber = ArbitraryTestNumber,
                CreatedDate = x.DateTime,
                DocumentId = x.DocumentId,
                ErrorStage = ErrorStage1,
                SupplierName = SupplierName,
            });

            var expectedFilesWithErrorsMetadata = new FilesWithErrorsMetadata
            {
                FilesWithErrors = documentDetails.Skip(1).Select(x => new FileWithErrorsMetadata
                {
                    DocumentId = x.DocumentId,
                    ErrorStage = ErrorStage1,
                    DateTime = x.DateTime,
                })
            };

            return new TheoryData<IEnumerable<ValidationError>, FilesWithErrorsMetadata>
            {
                { validationErrors, expectedFilesWithErrorsMetadata }
            };
        }

        public static TheoryData<IEnumerable<ValidationError>, FilesWithErrorsMetadata> ValidationErrorsAtMoreThanOneStageInSingleFile()
        {
            var dateTime = new DateTime(2023, 07, 12, 14, 42, 40, DateTimeKind.Unspecified);
            var documentId = Guid.NewGuid();

            var validationErrors = new[]
                {
                    new ValidationError {
                        TestNumber = ArbitraryTestNumber,
                        CreatedDate = dateTime,
                        DocumentId = documentId,
                        ErrorStage = ErrorStage1,
                        SupplierName = SupplierName,
                    },
                    new ValidationError {
                        TestNumber = ArbitraryTestNumber,
                        CreatedDate = dateTime,
                        DocumentId = documentId,
                        ErrorStage = ErrorStage2,
                        SupplierName = SupplierName,
                    }
                };

            var expectedFilesWithErrorsMetadata = new FilesWithErrorsMetadata
            {
                FilesWithErrors = new[] {
                    new FileWithErrorsMetadata
                    {
                        DocumentId = documentId,
                        ErrorStage = ErrorStage1,
                        DateTime = dateTime,
                    },
                    new FileWithErrorsMetadata
                    {
                        DocumentId = documentId,
                        ErrorStage = ErrorStage2,
                        DateTime = dateTime,
                    },
                }
            };

            return new TheoryData<IEnumerable<ValidationError>, FilesWithErrorsMetadata>
            {
                { validationErrors, expectedFilesWithErrorsMetadata }
            };
        }

        public static TheoryData<IEnumerable<ValidationError>, FilesWithErrorsMetadata> ValidationErrorsAtMoreThanOneStageInMultipleFiles()
        {
            var documentDetails = new[]
            {
                new { DateTime = new DateTime(2023, 09, 06, 05, 37, 10, DateTimeKind.Unspecified), DocumentId = Guid.NewGuid() },
                new { DateTime = new DateTime(2023, 08, 16, 18, 04, 20, DateTimeKind.Unspecified), DocumentId = Guid.NewGuid() },
            };

            var validationErrors = documentDetails.SelectMany(x => new[] {
                new ValidationError {
                    TestNumber = ArbitraryTestNumber,
                    CreatedDate = x.DateTime,
                    DocumentId = x.DocumentId,
                    ErrorStage = ErrorStage1,
                    SupplierName = SupplierName,
                },
                new ValidationError {
                    TestNumber = ArbitraryTestNumber,
                    CreatedDate = x.DateTime,
                    DocumentId = x.DocumentId,
                    ErrorStage = ErrorStage2,
                    SupplierName = SupplierName,
                }
            });

            var expectedFilesWithErrorsMetadata = new FilesWithErrorsMetadata
            {
                FilesWithErrors = documentDetails.SelectMany(x => new[] {
                    new FileWithErrorsMetadata
                    {
                        DocumentId = x.DocumentId,
                        ErrorStage = ErrorStage1,
                        DateTime = x.DateTime,
                    },
                    new FileWithErrorsMetadata
                    {
                        DocumentId = x.DocumentId,
                        ErrorStage = ErrorStage2,
                        DateTime = x.DateTime,
                    },
                })
            };

            return new TheoryData<IEnumerable<ValidationError>, FilesWithErrorsMetadata>
            {
                { validationErrors, expectedFilesWithErrorsMetadata }
            };
        }

        [Fact]
        public async Task SaveMeasures_SetsPurposeOfNotificationId()
        {
            // Arrange
            var purposeOfNotificationText = "Notification Purpose Text";
            var purposeOfNotificationId = 1;
            var purposeOfNotifications = new[]
            {
                new PurposeOfNotification
                    { Name = purposeOfNotificationText, PurposeOfNotificationId = purposeOfNotificationId },
            };
            SetupMockContextForDbSets(purposeOfNotifications: purposeOfNotifications);

            var measureDto = new MeasureDto{ PurposeOfNotification = purposeOfNotificationText };
            SetupMapperForMeasureDto(measureDto);

            var measureModels = new[] { new MeasureModel { PurposeOfNotification = purposeOfNotificationText } };

            // Act
            await _repository.SaveMeasures(measureModels);

            // Assert
            Assert.Equal(purposeOfNotificationId, measureDto.PurposeOfNotificationId);
        }

        [Fact]
        public async Task SaveMeasures_SetsMeasureTypeId()
        {
            // Arrange
            var measureTypeText = "Measure Type Text";
            var measureTypeId = 1;
            var measureTypes = new[] { new MeasureType { MeasureTypeId = measureTypeId, Name = measureTypeText } };
            SetupMockContextForDbSets(measureTypes: measureTypes);

            var measureDto = new MeasureDto { MeasureType = measureTypeText };
            SetupMapperForMeasureDto(measureDto);

            var measureModels = new[] { new MeasureModel { MeasureType = measureTypeText } };

            // Act
            await _repository.SaveMeasures(measureModels);

            // Assert
            Assert.Equal(measureTypeId, measureDto.MeasureTypeId);
        }

        [Fact]
        public async Task SaveMeasures_SetsEligibilityTypeId()
        {
            // Arrange
            SetupMockContextForDbSets();
            var eligibilityTypeText = "Eligibility Type Text";
            var eligibilityTypeId = 1;
            var eligibilityTypes = new[]
                { new EligibilityType { EligibilityTypeId = eligibilityTypeId, Name = eligibilityTypeText } };
            SetupMockContextForDbSets(eligibilityTypes: eligibilityTypes);

            var measureDto = new MeasureDto { EligibilityType = eligibilityTypeText };
            SetupMapperForMeasureDto(measureDto);

            var measureModels = new[] { new MeasureModel { EligibilityType = eligibilityTypeText } };

            // Act
            await _repository.SaveMeasures(measureModels);

            // Assert
            Assert.Equal(eligibilityTypeId, measureDto.EligibilityTypeId);
        }

        [Fact]
        public async Task SaveMeasures_SetsTenureTypeId()
        {
            // Arrange
            var tenureTypeText = "Tenure Type Text";
            var tenureTypeId = 1;
            var tenureTypes = new[] { new TenureType { TenureTypeId = tenureTypeId, Name = tenureTypeText } };
            SetupMockContextForDbSets(tenureTypes: tenureTypes);

            var measureDto = new MeasureDto { TenureType = tenureTypeText };
            SetupMapperForMeasureDto(measureDto);

            var measureModels = new[] { new MeasureModel { TenureType = tenureTypeText } };

            // Act
            await _repository.SaveMeasures(measureModels);

            // Assert
            Assert.Equal(tenureTypeId, measureDto.TenureTypeId);
        }

        [Fact]
        public async Task SaveMeasures_SetsModifiedDate()
        {
            // Arrange
            SetupMockContextForDbSets();

            var measureDto = new MeasureDto();
            SetupMapperForMeasureDto(measureDto);

            var dateTimeOffset = new DateTimeOffset(2023, 10, 13, 11, 22, 33, TimeSpan.Zero);
            _timeProviderMock.Setup(x => x.GetLocalNow()).Returns(dateTimeOffset);

            var measureModels = new[] { new MeasureModel() };

            // Act
            await _repository.SaveMeasures(measureModels);

            // Assert
            Assert.Equal(dateTimeOffset.DateTime, measureDto.ModifiedDate);
        }

        private void SetupMapperForMeasureDto(MeasureDto measureDto)
        {
            _mapperMock.Setup(x => x.Map<List<MeasureDto>>(It.IsAny<IEnumerable<MeasureModel>>()))
                .Returns(new List<MeasureDto> { measureDto });
        }

        private void SetupMockContextForDbSets(IEnumerable<TenureType>? tenureTypes = null,
            IEnumerable<EligibilityType>? eligibilityTypes = null,
            IEnumerable<MeasureType>? measureTypes = null,
            IEnumerable<PurposeOfNotification>? purposeOfNotifications = null,
            IEnumerable<VerificationMethodType>? verificationMethodTypes = null)
        {
            purposeOfNotifications ??= Array.Empty<PurposeOfNotification>();
            var purposeOfNotificationMockDbSet = purposeOfNotifications.AsQueryable().BuildMockDbSet();
            _contextMock.Setup(dbContext => dbContext.PurposeOfNotifications)
                .Returns(purposeOfNotificationMockDbSet.Object);

            measureTypes ??= Array.Empty<MeasureType>();
            var measureTypesMockDbSet = measureTypes.AsQueryable().BuildMockDbSet();
            _contextMock.Setup(dbContext => dbContext.MeasureTypes).Returns(measureTypesMockDbSet.Object);

            eligibilityTypes ??= Array.Empty<EligibilityType>();
            var eligibilityTypesMockDbSet = eligibilityTypes.AsQueryable().BuildMockDbSet();
            _contextMock.Setup(dbContext => dbContext.EligibilityTypes).Returns(eligibilityTypesMockDbSet.Object);

            tenureTypes ??= Array.Empty<TenureType>();
            var tenureTypesMockDbSet = tenureTypes.AsQueryable().BuildMockDbSet();
            _contextMock.Setup(dbContext => dbContext.TenureTypes).Returns(tenureTypesMockDbSet.Object);

            verificationMethodTypes ??= Array.Empty<VerificationMethodType>();
            var verificationMethodTypesMockDbSet = verificationMethodTypes.AsQueryable().BuildMockDbSet();
            _contextMock.Setup(dbContext => dbContext.VerificationMethodType).Returns(verificationMethodTypesMockDbSet.Object);
        }


        private class MeasureRepositoryTesting : MeasureRepository
        {
            public MeasureRepositoryTesting(ILogger<MeasureRepository> logger, MeasuresDbContext context,
                IMapper mapper, TimeProvider timeProvider) : base(logger, context, mapper, timeProvider)
            {
            }

            protected override Task BulkInsertOrUpdateAsync(List<Measure> measures)
            {
                return Task.CompletedTask;
            }

            protected override Task BulkInsertAsync(List<MeasureHistory> measureHistory)
            {
                return Task.CompletedTask;
            }
        }
    }
}