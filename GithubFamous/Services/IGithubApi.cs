using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GithubFamous.Models.Github;

namespace GithubFamous.Services
{
    public interface IGithubApi
    {
        public Task<List<Repository>> GetMostStarredRepositories(string programmingLanguage, int resultLimit);
    }
}
