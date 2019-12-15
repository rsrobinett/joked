using System.Linq;
using Joked.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Joked.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class JokesController : ControllerBase
	{
		private readonly IJokeHttpClient _httpClient;
		private readonly ILogger<JokesController> _logger;
		private readonly IJokesHandler _jokesHandler;
		private readonly IHubContext<JokeHub> _hub;

		internal JokesController(IJokeHttpClient client, ILogger<JokesController> logger, IJokesHandler jokesHandler, IHubContext<JokeHub> hub)
		{
			_jokesHandler = jokesHandler;
			_httpClient = client;
			_logger = logger;
			_hub = hub;
		}

		public JokesController(IJokeHttpClient client, ILogger<JokesController> logger, IHubContext<JokeHub> hub) : this(client, logger, new JokesHandler(logger, client), hub)
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

			var jokes = _jokesHandler.GetJokes(term, limit);

			var curatedJokes = _jokesHandler.CurateJokes(jokes.Jokes, term);
			
			return Ok(curatedJokes);
		}

		private (ActionResult Result, bool IsValid) ValidateRequest(string term, bool curate)
		{
			const string invalidSearchTerm = "The search term cannot be empty or whitespace";
			const string notSupportedErrorMessage = "Request is not implemented, only curated jokes are available at this time";

			if (!ModelState.IsValid)
			{
				_logger.Log(LogLevel.Information, ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage + " ").ToString());
				return (BadRequest(), false);
			}

			if (!curate)
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