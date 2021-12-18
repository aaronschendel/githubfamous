using System.Net.Mime;
using System.Threading.Tasks;
using GithubFamous.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GithubFamous.Controllers
{
    /// <summary>
    /// Executes tasks related to calling Github's official API
    /// </summary>
    [ApiController, Route("githubstars"), Produces(MediaTypeNames.Application.Json)]
    public class GithubStarsController : ControllerBase
    {
        private readonly IGithubApi _githubApi;

        public GithubStarsController(IGithubApi githubApi)
        {
            _githubApi = githubApi;
        }

        /// <summary>
        /// Gets a list of Github repositories ordered by number of stars descending in the programming language entered by the user
        /// </summary>
        /// <param name="programmingLanguage">The programming language the returned respositories are written in. A list of valid languages <a href="https://github.com/search/advanced">can be found here.</a></param>
        /// <param name="resultLimit">Desired number of results. Max = 100 </param>
        /// <returns>A list of Github repository objects ordered by most stars to least</returns>
        [HttpGet,
            ProducesResponseType(StatusCodes.Status200OK),
            ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get([FromQuery] string programmingLanguage, [FromQuery] int resultLimit)
        {
            return new JsonResult(await _githubApi.GetMostStarredRepositories(programmingLanguage, resultLimit));

            //return new BadRequestObjectResult(new ProblemDetails()
            //{
            //    Detail = "",
            //    Title = "",
            //    Status = (int) HttpStatusCode.BadRequest,
            //    Type = ""
            //});
        }
    }
}



// Build a Web Service that calls GitHub's API and returns the top 5 starred repos for a user-supplied language.