using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using Ofgem.API.GBI.MeasureValidation.Application.Interfaces.Services;
using Ofgem.API.GBI.MeasureValidation.Domain.Models;
using System.Net;
using System.Text;

namespace Ofgem.API.GBI.MeasureValidation.Api.UnitTests
{
    public class MeasureValidationApiTests
    {
        private readonly Mock<IStage1ProcessingService> _stage1ProcessingService;
        private readonly Mock<IStage2ProcessingService> _stage2ProcessingService;
        public MeasureValidationApiTests()
        {
            _stage1ProcessingService = new Mock<IStage1ProcessingService>();
            _stage1ProcessingService.Setup(x => x.ProcessStage1Validation(It.IsAny<MeasureDocumentDetails>()));
            _stage2ProcessingService = new Mock<IStage2ProcessingService>();
            _stage2ProcessingService.Setup(x => x.ProcessStage2Validation(It.IsAny<MeasureDocumentDetails>()));
        }

        [Fact]
        public async void Start_ProcessStage1Validation_Success()
        {
            await using var application = new TestApplicationFactory(x =>
            {
                x.AddSingleton(_stage1ProcessingService.Object);
            });

            var client = application.CreateClient();

            var measureDetails = new StringContent(JsonConvert.SerializeObject(new MeasureDocumentDetails()
            {
                DocumentId = new Guid()
            }), encoding: Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/ProcessStage1Validation", measureDetails);
            await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async void Start_ProcessStage1Validation_ReturnsBadRequest()
        {
            await using var application = new TestApplicationFactory(x =>
            {
                x.AddSingleton(_stage1ProcessingService.Object);
            });

            var client = application.CreateClient();

            var measureDetails = new StringContent(JsonConvert.SerializeObject(null), encoding: Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/ProcessStage1Validation", measureDetails);
            await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async void GetStage1ValidationResult_Returns_Success()
        {
            var docId = Guid.NewGuid();
            await using var application = new TestApplicationFactory(x =>
            {
                x.AddSingleton(_stage1ProcessingService.Object);
            });
            _stage1ProcessingService.Setup(x => x.GetStage1ValidationResult(docId)).ReturnsAsync(new Stage1ValidationResultResponse() { DocumentId = docId });

            var client = application.CreateClient();

            var response = await client.GetAsync($"/GetStage1ChecksResult/{docId}");
            var data = response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(data);
        }

        [Fact]
        public async void GetStage1ValidationResult_Returns_NotFound()
        {
            var docId = Guid.NewGuid();
            await using var application = new TestApplicationFactory(x =>
            {
                x.AddSingleton(_stage1ProcessingService.Object);
            });
            _stage1ProcessingService.Setup(x => x.GetStage1ValidationResult(docId)).ReturnsAsync(It.IsAny<Stage1ValidationResultResponse>);

            var client = application.CreateClient();
            var response = await client.GetAsync($"/GetStage1ChecksResult/{docId}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async void Start_ProcessStage2Validation_Success()
        {
            await using var application = new TestApplicationFactory(x =>
            {
                x.AddSingleton(_stage2ProcessingService.Object);
            });

            var client = application.CreateClient();

            var measureDetails = new StringContent(JsonConvert.SerializeObject(new MeasureDocumentDetails()
            {
                DocumentId = new Guid()
            }), encoding: Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/ProcessStage2Validation", measureDetails);
            await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }


        [Fact]
        public async void Start_ProcessStage2Validation_ReturnsBadRequest()
        {
            await using var application = new TestApplicationFactory(x =>
            {
                x.AddSingleton(_stage2ProcessingService.Object);
            });

            var client = application.CreateClient();

            var measureDetails = new StringContent(JsonConvert.SerializeObject(null), encoding: Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/ProcessStage2Validation", measureDetails);
            await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

    }
}