using System;
using System.Linq;
using System.Text.Json;
using Joked.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Joked.Controllers
{
	[BindProperties(SupportsGet = true)]
	public class CuratedRequest
	{
		[FromQuery(Name="curate")]
		public bool IsCurated { get; set; }
		[FromQuery(Name="term")]
		public string Term { get; set; }
	}

	[ApiController]
	[Route("api/[controller]")]
	public class JokesController : ControllerBase
	{
		private readonly IJokeHttpClient _httpClient;
		private readonly ILogger<JokesController> _logger;
		private readonly ICuratedHandler _curatedHandler;
		private readonly IHubContext<JokeHub> _hub;

		internal JokesController(IJokeHttpClient client, ILogger<JokesController> logger, ICuratedHandler curatedHandler, IHubContext<JokeHub> hub)
		{
			_curatedHandler = curatedHandler;
			_httpClient = client;
			_logger = logger;
			_hub = hub;
		}

		public JokesController(IJokeHttpClient client, ILogger<JokesController> logger, IHubContext<JokeHub> hub) : this(client, logger, new CuratedHandler(logger, client), hub)
		{
		
		}

		/// <summary>
		/// Display curated list of jokes for a search term
		/// </summary>
		/// <remarks>
		/// Accept a search term and display the first 30 jokes containing that term,
		/// with the matching term emphasized in some way (upper, bold, color, etc.)
		/// and the matching jokes grouped by length: short  <![CDATA[ (<10 words), medium (<20 words), long (<= 20 words) ]]>
		/// https://icanhazdadjoke.com/api
		/// </remarks>
		[HttpGet("")]
		public ActionResult<CuratedJokes> GetJokes([FromQuery] string term, [FromQuery] bool curate = true, [FromQuery] int limit = 30)
		{
			var (result, isValid) = ValidateRequest(term, curate);
			if (!isValid)
			{
				return result;
			}

			var jokes = _curatedHandler.GetJokes(term, limit);

			var curatedJokes = _curatedHandler.CurateJokes(jokes.Jokes, term);
			
			return Ok(curatedJokes);
		}

		/// <summary>
		/// Display curated list of jokes for a search term
		/// </summary>
		/// <remarks>
		/// Accept a search term and display the first 30 jokes containing that term,
		/// with the matching term emphasized in some way (upper, bold, color, etc.)
		/// and the matching jokes grouped by length: short  <![CDATA[ (<10 words), medium (<20 words), long (<= 20 words) ]]>
		/// https://icanhazdadjoke.com/api
		/// </remarks>
		[HttpGet("curated/{searchTerm}")]
		public IActionResult GetCuratedJokes(string searchTerm)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest();
			}

			if (string.IsNullOrWhiteSpace(searchTerm))
			{
				var errorMessage = "The searchTerm cannot be empty or whitespace";
				_logger.Log(LogLevel.Debug, new Exception(errorMessage), errorMessage);

				return BadRequest(errorMessage);
			}
			
			var result = _httpClient.Get($"search?limit=30&term={searchTerm}").Result.Replace(@"\r\n", " ");
			var jokes = JsonSerializer.Deserialize<JokesIncoming>(result);

			var curatedJokes = _curatedHandler.CurateJokes(jokes.Jokes, searchTerm);
			
			return Ok(curatedJokes);
		}

		public (ActionResult Result, bool IsValid) ValidateRequest(string term, bool curate)
		{
			const string invalidSearchTerm = "The search term cannot be empty or whitespace";
			const string notSupportedErrorMessage = "Request is not implemented, only curated jokes are available at this time";

			if (!ModelState.IsValid)
			{
				_logger.Log(LogLevel.Information, ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage + Environment.NewLine).ToString());
				return (BadRequest(), false);
			}

			if (curate)
			{
				_logger.Log(LogLevel.Information, notSupportedErrorMessage);
				return (StatusCode(501, notSupportedErrorMessage), false);
			}

			if (string.IsNullOrWhiteSpace(term))
			{
				_logger.Log(LogLevel.Information, invalidSearchTerm);
				return (BadRequest(invalidSearchTerm), false);
			}

			return (null, true);
		}
	}
}