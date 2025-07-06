using Moq;
using Newtonsoft.Json;
using SharpTrooper.Core;
using SharpTrooper.Entities;
using System.Net;
using System.Runtime.CompilerServices;

namespace SharpTrooper.Tests
{
    [TestClass]
    public class TestRequests
    {
        private readonly string jsonFilmData;

        public TestRequests()
        {
            jsonFilmData = File.ReadAllText("./Data/film-sample-data.json");
        }

        public class DelegatingHandlerStub : DelegatingHandler
        {
            private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _handlerFunc;

            public DelegatingHandlerStub()
            {
                _handlerFunc = (request, cancellationToken) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
            }

            public DelegatingHandlerStub(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handlerFunc)
            {
                _handlerFunc = handlerFunc;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return _handlerFunc(request, cancellationToken);
            }
        }

        [TestMethod]
        public async Task Should_Return_Six_Films()
        {
            // Arrange
            var expected = jsonFilmData;

            var clientHandlerStub = new DelegatingHandlerStub((request, cancellationToken) => 
            {
                var response = new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new StringContent(expected) };
                return Task.FromResult(response);
            });

            var mockClientFactory = new Mock<IHttpClientFactory>();
            mockClientFactory.Setup(m => m.CreateClient(It.IsAny<string>())).Returns(() => new HttpClient(clientHandlerStub));

            // Act
            var sharpTrooperCore = new SharpTrooperCore(mockClientFactory.Object);
            var films = await sharpTrooperCore.GetAllFilmsAsync();

            // Assert
            Assert.AreEqual(6, films.Count);
        }

        private static readonly HttpClient _httpClient = new HttpClient();

        [TestMethod]
        public async Task First_Film_Should_Be_A_New_Hope()
        {
            try
            {
                string response = await _httpClient.GetStringAsync("https://swapi.info/api/films");

                var films = JsonConvert.DeserializeObject<IList<Film>>(response);

                Assert.IsTrue(films?[0].title.Equals("A New Hope"));
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("Exception caught");
                Console.WriteLine("Message :{0}", e.Message);
            }
        }
    }
}