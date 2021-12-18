using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using GithubFamous.Models.Github;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GithubFamous.Services
{
    public class GithubApi : IGithubApi
    {
        private const string githubSearchBaseUri = "https://api.github.com/search/repositories";
        private const string githubAcceptHeader = "application/vnd.github.v3+json";
        private static HttpClient client;

        public GithubApi() {
            client = new HttpClient();
        }

        public async Task<List<Repository>> GetMostStarredRepositories(string programmingLanguage, int resultLimit)
        {
            client.BaseAddress = new Uri(githubSearchBaseUri);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(githubAcceptHeader));
            client.DefaultRequestHeaders.UserAgent.TryParseAdd("request");

            string queryParams = $"q=language:{programmingLanguage}&sort=stars&order=desc";

            var response = await client.GetAsync($"?{queryParams}");

            if (response.IsSuccessStatusCode)
            {
                var stringJson = await response.Content.ReadAsStringAsync();
                JObject fullResponseJson = JObject.Parse(stringJson);
                List<Repository> repositoriesFromResponse = JsonConvert.DeserializeObject<List<Repository>>(fullResponseJson["items"].ToString());
                var result = repositoriesFromResponse.GetRange(0, resultLimit);
                return result;
            }
            else
            {
                return null;
            }
        }
    }
}
