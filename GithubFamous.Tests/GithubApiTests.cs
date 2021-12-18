using System.Collections.Generic;
using GithubFamous.Models.Github;
using GithubFamous.Services;
using Xunit;
using FluentAssertions;
using System.Linq;

namespace GithubFamous.Tests
{
    public class GithubApiTests
    {

        public GithubApiTests()
        {
            List<Repository> reposList = new List<Repository>
            {
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
        public async void GithubApi_GetMostStarredRepo_Returns_MostStarredRepos_WhenAllIsWell()
        {
            GithubApi api = new GithubApi();
            var result = await api.GetMostStarredRepositories("python", 5);
            result.Count.Should().Be(5);
            result[0].Id.Should().Be(6);
            result[1].Id.Should().Be(5);
            result[2].Id.Should().Be(4);
            result[3].Id.Should().Be(3);
            result[4].Id.Should().Be(2);
            result.FirstOrDefault(x => x.Id == 1).Should().BeNull();
        }
    }
}
