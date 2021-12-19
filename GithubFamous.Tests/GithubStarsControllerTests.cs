using System.Collections.Generic;
using GithubFamous.Models.Github;
using GithubFamous.Services;
using Xunit;
using FluentAssertions;
using Moq;
using GithubFamous.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GithubFamous.Tests.Controllers
{
    public class GithubStarsControllerTests
    {
        private Mock<IGithubApi> _githubApiMock;
        private Mock<ProblemDetailsFactory> _problemDetailsFactoryMock;
        private List<Repository> _repos;

        public GithubStarsControllerTests()
        {
            _githubApiMock = new Mock<IGithubApi>();
            _problemDetailsFactoryMock = new Mock<ProblemDetailsFactory>();
            var problemDetails = new ValidationProblemDetails();
            _problemDetailsFactoryMock.Setup(_ => _.CreateValidationProblemDetails(
                    It.IsAny<HttpContext>(),
                    It.IsAny<ModelStateDictionary>(),
                    It.IsAny<int?>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>())
                )
                .Returns(problemDetails);
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

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async void GithubStarsController_Get_Returns_ValidationProblemDetails_When_ProgrammingLanguageIsNullOrEmpty(string programmingLanguage)
        {
            // Arrange
            _githubApiMock.Setup(x => x.GetMostStarredRepositories(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(() => _repos);

            GithubStarsController controller = new GithubStarsController(_githubApiMock.Object);
            controller.ProblemDetailsFactory = _problemDetailsFactoryMock.Object;

            // Act
            var result = await controller.Get(programmingLanguage, 5);

            // Assert
            var resultObject = result.Result as ObjectResult;
            var resultValue = resultObject.Value as ValidationProblemDetails;
            resultValue.Should().BeOfType<ValidationProblemDetails>();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(101)]
        [InlineData(5000)]
        public async void GithubStarsController_Get_Returns_ValidationProblemDetails_When_ResultLimitIsLessThan1OrMoreThan100(int resultLimit)
        {
            // Arrange
            _githubApiMock.Setup(x => x.GetMostStarredRepositories(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(() => _repos);

            GithubStarsController controller = new GithubStarsController(_githubApiMock.Object);
            controller.ProblemDetailsFactory = _problemDetailsFactoryMock.Object;

            // Act
            var result = await controller.Get("python", resultLimit);

            // Assert
            var resultObject = result.Result as ObjectResult;
            var resultValue = resultObject.Value as ValidationProblemDetails;
            resultValue.Should().BeOfType<ValidationProblemDetails>();
        }

        [Fact]
        public async void GithubStarsController_Get_Returns_ProblemDetails_When_ApiResultIsNull()
        {
            // Arrange
            _githubApiMock.Setup(x => x.GetMostStarredRepositories(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(() => null);
            GithubStarsController controller = new GithubStarsController(_githubApiMock.Object);

            // Act
            var result = await controller.Get("python", 5);

            // Assert
            var resultObject = result.Result as JsonResult;
            var resultValue = resultObject.Value as ProblemDetails;
            resultValue.Should().BeOfType<ProblemDetails>();
            resultValue.Status.Should().Be(500);
        }
    }
}
