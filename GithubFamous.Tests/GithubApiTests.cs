using System.Collections.Generic;
using GithubFamous.Models.Github;
using GithubFamous.Services;
using Xunit;
using FluentAssertions;
using Moq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using Moq.Protected;
using System.Threading;
using Newtonsoft.Json;
using GithubFamous.Tests.Models;
using Microsoft.Extensions.Logging;

namespace GithubFamous.Tests.Services
{
    public class GithubApiTests
    {
        private SearchResponse _searchResponse;
        private ILogger<GithubApi> _logger;

        public GithubApiTests()
        {
            _logger = Mock.Of<ILogger<GithubApi>>();
            _searchResponse = new SearchResponse()
            {
                total_count = 5,
                incomplete_results = false,
                items = new List<Repository> {
                    new Repository
                    {
                        Id = 1,
                        Stargazers_Count = 25
                    },
                    new Repository
                    {
                        Id = 2,
                        Stargazers_Count = 50
                    },
                    new Repository
                    {
                        Id = 3,
                        Stargazers_Count = 75
                    },
                    new Repository
                    {
                        Id = 4,
                        Stargazers_Count = 100
                    },
                    new Repository
                    {
                        Id = 5,
                        Stargazers_Count = 125
                    }
                }
            };
        }

        [Fact]
        public async void GithubApi_GetMostStarredRepositories_Returns_Repos_When_AllIsWell()
        {
            // Arrange
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(_searchResponse))
                });
            GithubApi api = new GithubApi(new HttpClient(mockMessageHandler.Object), _logger);

            // Act
            var result = await api.GetMostStarredRepositories("python", 5);

            // Assert
            result.Count.Should().Be(5);
        }

        [Fact]
        public async void GithubApi_GetMostStarredRepositories_Returns_Null_When_CallToGithubIsNotSuccessful()
        {
            // Arrange
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound
                });
            GithubApi api = new GithubApi(new HttpClient(mockMessageHandler.Object), _logger);

            // Act
            var result = await api.GetMostStarredRepositories("python", 5);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async void GithubApi_GetMostStarredRepositories_Returns_Null_When_ExceptionOccurs()
        {
            // Arrange
            var mockMessageHandler = new Mock<HttpMessageHandler>();
            mockMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new System.Exception());
            GithubApi api = new GithubApi(new HttpClient(mockMessageHandler.Object), _logger);

            // Act
            var result = await api.GetMostStarredRepositories("python", 5);

            // Assert
            result.Should().BeNull();
        }
    }
}
