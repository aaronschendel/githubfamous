using System.Collections.Generic;
using GithubFamous.Models.Github;

namespace GithubFamous.Tests.Models
{
    public class SearchResponse
    {
        public int total_count { get; set; }
        public bool incomplete_results { get; set; }
        public List<Repository> items { get; set; }
    }
}
