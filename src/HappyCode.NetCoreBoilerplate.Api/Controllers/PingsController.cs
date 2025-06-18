using System.Net;
using HappyCode.NetCoreBoilerplate.Api.BackgroundServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Linq; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HappyCode.NetCoreBoilerplate.Api.Controllers
{
    [AllowAnonymous]
    [Route("api/pings")]
    public class PingsController : ApiControllerBase
    {
        private readonly IPingService _pingService;
        private readonly ILogger<PingsController> _logger;

        public PingsController(IPingService pingService,ILogger<PingsController> logger)
        {
            _pingService = pingService;
            _logger = logger;
        }

        [HttpGet("website")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public Task<IActionResult> GetWebsitePingStatusCodeAsync(
            CancellationToken cancellationToken = default)
        {
            var result = _pingService.WebsiteStatusCode;
            return Task.FromResult<IActionResult>(Ok($"{(int)result} ({result})"));
        }

        [HttpGet("random")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public Task<IActionResult> GetRandomStatusCodeAsync(
            CancellationToken cancellationToken = default)
        {
            var random = new Random(Guid.NewGuid().GetHashCode());
            int pretender;
            do
            {
                pretender = random.Next(100, 600);
            } while (!Enum.IsDefined(typeof(HttpStatusCode), pretender));
            return Task.FromResult<IActionResult>(Ok($"{pretender} ({(HttpStatusCode)pretender})"));
        }

        /// <summary>
        /// Retrieves the nth string from a provided array of strings.
        /// </summary>
        /// <param name="strings">An array of strings.</param>
        /// <param name="n">The zero-based index of the string to retrieve.</param>
        /// <returns>The nth string if valid, otherwise an error message.</returns>
        [HttpPost("get-nth-string")] // Use POST as we're sending data in the body (the array)
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetNthString([FromBody] List<string> strings, [FromQuery] int n)
        {
            if (n < 0 || n >= strings.Count)
            {
                return BadRequest("Index out of range");
            }
            try{
                string result = strings[n];
                return Ok(result);
            }
            catch(Exception ex){
                _logger.LogError(ex, "Something went wrong in PingsController.cs");
            }
            return Ok("Something went wrong");
        }
    }
}