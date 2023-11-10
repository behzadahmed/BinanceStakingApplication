using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using BinanceWatcherFunction;
using BinanceWatcherFunction.Configuration;
using BinanceWatcherFunction.Interfaces;
using BinanceWatcherFunction.Models;
using Microsoft.Azure.Amqp.Framing;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Timers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using NUnit.Framework;

namespace BinanceWatcherFunction.Tests
{
    [TestFixture]
    public class BinanceWatcherFunctionTests
    {
        private IFixture _fixture;
        private Mock<IMongoDbService> _mongoDbServiceMock;
        private Mock<ILogger<BinanceWatcherFunction>> _loggerMock;
        private Mock<IOptions<BinanceApiConfiguration>> _configMock;
        private Mock<IDataHandlerService> _handlerServiceMock;
        private BinanceWatcherFunction _binanceWatcherFunction;
        private TimerInfo _timerInfo;
        private Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private HttpClient _httpClient;
        private HttpResponseMessage _httpResponseMessage;



        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _mongoDbServiceMock = _fixture.Freeze<Mock<IMongoDbService>>();
            _loggerMock = _fixture.Freeze<Mock<ILogger<BinanceWatcherFunction>>>();
            _configMock = _fixture.Freeze<Mock<IOptions<BinanceApiConfiguration>>>();
            _handlerServiceMock = _fixture.Freeze<Mock<IDataHandlerService>>();

            var result = _fixture.Create<Result>();
            _httpResponseMessage = new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(result), Encoding.UTF8, "application/json")
            };

            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpMessageHandlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(_httpResponseMessage)
               .Verifiable();

            _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };

            _binanceWatcherFunction = new BinanceWatcherFunction(_mongoDbServiceMock.Object, _httpClient, _loggerMock.Object, _configMock.Object, _handlerServiceMock.Object);
            _timerInfo = new TimerInfo(null, new ScheduleStatus());
        }

        [Test]
        public async Task Run_WhenCalled_ShouldCallFindDataAsyncForEachDataItem()
        {
            var result = _fixture.Create<Result>();
      
      
            _binanceWatcherFunction = new BinanceWatcherFunction(_mongoDbServiceMock.Object, _httpClient, _loggerMock.Object, _configMock.Object, _handlerServiceMock.Object);

            await _binanceWatcherFunction.Run(_timerInfo);

            _mongoDbServiceMock.Verify(x => x.FindDataAsync(It.IsAny<FilterDefinition<ProjectData>>()), Times.Exactly(result.Data.Count));
        }

        [Test]
        public async Task Run_WhenDataExists_ShouldCallHandleExistingData()
        {
            var result = _fixture.Create<Result>();       

         
            var httpClient = new HttpClient(_httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };

            _binanceWatcherFunction = new BinanceWatcherFunction(_mongoDbServiceMock.Object, httpClient, _loggerMock.Object, _configMock.Object, _handlerServiceMock.Object);
            _mongoDbServiceMock.Setup(x => x.FindDataAsync(It.IsAny<FilterDefinition<ProjectData>>())).ReturnsAsync(_fixture.Create<ProjectData>());

            await _binanceWatcherFunction.Run(_timerInfo);

            _handlerServiceMock.Verify(x => x.HandleExistingData(It.IsAny<ProjectData>(), It.IsAny<ProjectData>(), It.IsAny<FilterDefinition<ProjectData>>(), It.IsAny<FilterDefinition<CryptoList>>()), Times.Exactly(result.Data.Count));
        }

        [Test]
        public async Task Run_WhenDataDoesNotExist_ShouldCallHandleNewData()
        {
            var result = _fixture.Create<Result>();

          
            _binanceWatcherFunction = new BinanceWatcherFunction(_mongoDbServiceMock.Object, _httpClient, _loggerMock.Object, _configMock.Object, _handlerServiceMock.Object);
            _mongoDbServiceMock.Setup(x => x.FindDataAsync(It.IsAny<FilterDefinition<ProjectData>>())).ReturnsAsync((ProjectData)null);

            await _binanceWatcherFunction.Run(_timerInfo);

            _handlerServiceMock.Verify(x => x.HandleNewData(It.IsAny<ProjectData>()), Times.Exactly(result.Data.Count));
        }



    }
}
