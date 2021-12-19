using System;
using System.Collections.Generic;
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
        private static HttpClient _client = new HttpClient();

        public GithubApi(HttpClient httpClient) {
            _client = httpClient;
        }

        public async Task<List<Repository>> GetMostStarredRepositories(string programmingLanguage, int resultLimit)
        {
            try
            {
                _client.BaseAddress = new Uri(githubSearchBaseUri);
                _client.DefaultRequestHeaders.Accept.Clear();
                _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(githubAcceptHeader));
                _client.DefaultRequestHeaders.UserAgent.TryParseAdd("request");

                string queryParams = $"q=language:{programmingLanguage}&sort=stars&order=desc&per_page={resultLimit}";

                var response = await _client.GetAsync($"?{queryParams}");

                if (response.IsSuccessStatusCode)
                {
                    var stringJson = await response.Content.ReadAsStringAsync();
                    JObject fullResponseJson = JObject.Parse(stringJson);
                    List<Repository> repositoriesFromResponse = JsonConvert.DeserializeObject<List<Repository>>(fullResponseJson["items"].ToString());
                    return repositoriesFromResponse;
                }
                
                return null;
                
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
