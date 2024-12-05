using System.Net;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using RoadStatus;

namespace RoadStatusUnitTests
{

    [TestClass]
    public class RoadStatusServiceTests
    {
        private Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private HttpClient _httpClient;
        private Mock<IConfiguration> _configuration;

    [TestInitialize]
    public void SetUp()
    {
        // Mock the HttpMessageHandler
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();

        _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("https://api.tfl.gov.uk")
        };
        _configuration = new Mock<IConfiguration>();
    }

        [TestMethod]
        public void GetRoadStatusAsync_Valid_RoadId_Returns_RoadStatus_With_DisplayName()
        {
        string roadId = "A2";
        string expectedDisplayName ="A3";
        string expectedResponse = $"[{{\"displayName\": \"{expectedDisplayName}\", \"statusSeverity\": \"Good\", \"statusSeverityDescription\": \"No Exceptional Delays\"}}]";

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(expectedResponse)
            });
        var service = new RoadStatusService(_configuration.Object, _httpClient);

        var result =  service.GetRoadStatusAsync(roadId).Result;

        Assert.AreEqual(expectedDisplayName, result.DisplayName);
        
        }

        [TestMethod]
        public void GetRoadStatusAsync_Valid_RoadId_Returns_RoadStatus_With_StatusSeverity()
        {
            string roadId = "A2";
            string expectedStatusSeverity ="XXXX";
            string expectedResponse = $"[{{\"statusSeverity\": \"{expectedStatusSeverity}\", \"statusSeverityDescription\": \"No Exceptional Delays\"}}]";

            _httpMessageHandlerMock
                .Protected()
                 .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
            )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(expectedResponse)
                });

            var service = new RoadStatusService(_configuration.Object, _httpClient);

            var result =  service.GetRoadStatusAsync(roadId).Result;

            Assert.AreEqual(expectedStatusSeverity, result.StatusSeverity);
        }
        [TestMethod]
        public void GetRoadStatusAsync_Valid_RoadId_Returns_RoadStatus_With_StatusSeverityDescription()
        {
            string roadId = "A2";
            string expectedStatusSeverityDescription ="YYYY";
            string expectedResponse = $"[{{\"statusSeverityDescription\": \"{expectedStatusSeverityDescription}\"}}]";

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(expectedResponse)
                });

            var service = new RoadStatusService(_configuration.Object, _httpClient);

            var result =  service.GetRoadStatusAsync(roadId).Result;

            Assert.AreEqual(expectedStatusSeverityDescription, result.StatusSeverityDescription);
        }
        [TestMethod]
        public void GetRoadStatusAsync_InValid_RoadId_Returns_RoadStatus_With_Informative_Error_Code()
        {
            string roadId = "A233";
            var expectedErroMessage="The following road id is not recognised: A233";
            string expectedResponse = $"[{{\"message\": \"{expectedErroMessage}\"}}]";

            _httpMessageHandlerMock
                .Protected()
                 .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                 )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Content = new StringContent(expectedResponse)
                });

            var service = new RoadStatusService(_configuration.Object, _httpClient);

            var result =  service.GetRoadStatusAsync(roadId).Result;

            Assert.Equals(expectedErroMessage, result.ErroCode);
        }
         [TestMethod]
        public void GetRoadStatusAsync_InValid_RoadId_Returns_RoadStatus_With_Non_Zero_Exit_Code()
        {
            string roadId = "A233";
            string expectedResponse = "[{\"message\": \"Random Text....\"}]";

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Content = new StringContent(expectedResponse)
                });

            var service = new RoadStatusService(_configuration.Object, _httpClient);

            var result =  service.GetRoadStatusAsync(roadId).Result;

            Assert.IsTrue(result.ExitCode != 0);
        }

        [TestMethod]
        public void GetRoadStatusAsync_For_Invalid_Response_Returns_RoadStatus_With_Non_Zero_Exit_Code()
        {
            string roadId = "A233";
            string expectedResponse = "[{\"message\": \"RandomText\"}]";

            _httpMessageHandlerMock
                .Protected()
                 .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                 )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent(expectedResponse)
                });

            var service = new RoadStatusService(_configuration.Object, _httpClient);

            var result =  service.GetRoadStatusAsync(roadId).Result;

            Assert.IsTrue(result.ExitCode != 0);
        }
    }
}