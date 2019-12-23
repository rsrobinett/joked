using System;
using System.Linq;
using System.Threading.Tasks;
using Joked.Handlers;
using Joked.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Joked.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class JokesController : ControllerBase
	{
		private readonly ILogger<JokesController> _logger;
		private readonly IJokesHandler _jokesHandler;

		internal JokesController(IJokeHttpClient client, ILogger<JokesController> logger, IJokesHandler jokesHandler)
		{
			_jokesHandler = jokesHandler;
			_logger = logger;
		}

		public JokesController(IJokeHttpClient client, ILogger<JokesController> logger) : this(client, logger, new JokesHandler(logger, client))
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
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		[ProducesResponseType(StatusCodes.Status501NotImplemented)]
		public async Task<ActionResult<ICuratedJokes>> GetJokes([FromQuery] string term, [FromQuery] bool curate = true, [FromQuery] int limit = 30, [FromQuery] bool emphasize = false)
		{
			var (result, isValid) = ValidateRequest(term, curate);
			if (!isValid)
			{
				return result;
			}

			try
			{
				var jokes = await _jokesHandler.GetJokes(term, limit);
				var ensureLimit = jokes.Jokes?.Take(limit).ToArray();
				var curatedJokes = _jokesHandler.CurateJokes(ensureLimit, term, emphasize);

				return Ok(curatedJokes);
			}
			catch (Exception x)
			{
				_logger.Log(LogLevel.Error, x, "Request Failed For unknown reason");
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
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
				return (StatusCode(StatusCodes.Status501NotImplemented, notSupportedErrorMessage), false);
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