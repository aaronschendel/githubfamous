using System.Collections.Generic;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using GithubFamous.Models.Github;
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
        public async Task<ActionResult<List<Repository>>> Get([FromQuery] string programmingLanguage, [FromQuery] int resultLimit)
        {
            if (string.IsNullOrWhiteSpace(programmingLanguage))
            {
                this.ModelState.AddModelError(nameof(programmingLanguage), "ProgrammingLanguage cannot be null or empty.");
            }

            if (resultLimit < 1 || resultLimit > 100)
            {
                this.ModelState.AddModelError(nameof(resultLimit), "ResultLimit must be between 1 and 100.");
            }

            if (!this.ModelState.IsValid)
            {
                return this.ValidationProblem(this.ModelState);
            }

            var result = await _githubApi.GetMostStarredRepositories(programmingLanguage, resultLimit);

            if (result == null)
            {
                var problemDetails = new ProblemDetails()
                {
                    Detail = "Unexpected problem occurred, please try again later.",
                    Title = "Internal Server Error",
                    Status = (int)HttpStatusCode.InternalServerError,
                    Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1"
                };

                return new JsonResult(problemDetails)
                {
                    StatusCode = 500
                };
            }

            return new JsonResult(result);
        }
    }
}