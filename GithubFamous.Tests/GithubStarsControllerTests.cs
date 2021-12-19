using System.Collections.Generic;
using GithubFamous.Models.Github;
using GithubFamous.Services;
using Xunit;
using FluentAssertions;
using System.Linq;
using Moq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using Moq.Protected;
using System.Threading;
using Newtonsoft.Json;
using GithubFamous.Tests.Models;
using GithubFamous.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace GithubFamous.Tests.Controllers
{
    public class GithubStarsControllerTests
    {
        private Mock<IGithubApi> _githubApiMock;
        private List<Repository> _repos;

        public GithubStarsControllerTests()
        {
            _githubApiMock = new Mock<IGithubApi>();
            _repos = new List<Repository> {
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
                },
                new Repository
                {
                    Id = 6,
                    Stargazers_Count = 150
                }
            };
        }

        [Fact]
        public async void GithubStarsController_Get_Returns_Repos_When_AllIsWell()
        {
            // Arrange
            _githubApiMock.Setup(x => x.GetMostStarredRepositories(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(_repos);
            GithubStarsController controller = new GithubStarsController(_githubApiMock.Object);

            // Act
            var result = await controller.Get("python", 6);

            // Assert
            var jsonResult = result.Result as JsonResult;
            var repos = (List<Repository>)jsonResult.Value;
            repos.Count.Should().Be(6);
        }

        [Fact]
        public async void GithubStarsController_Get_Returns_InternalServerError_When_ApiResultIsNull()
        {
            // Arrange
            _githubApiMock.Setup(x => x.GetMostStarredRepositories(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(_repos);
            GithubStarsController controller = new GithubStarsController(_githubApiMock.Object);

            // Act
            var result = await controller.Get("python", 0);

            // Assert
            var test = result.Result;
            //test.
        }
    }
}
