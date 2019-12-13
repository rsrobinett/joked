using System;
using System.Text.Json;
using Joked.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Joked.Controllers
{

	[ApiController]
	[Route("[controller]")]
	public class CuratedController : ControllerBase
	{
		private readonly IJokeHttpClient _httpClient;
		private readonly ILogger<CuratedController> _logger;
		private readonly ICuratedHandler _curatedHandler;

		internal CuratedController(IJokeHttpClient client, ILogger<CuratedController> logger, ICuratedHandler curatedHandler)
		{
			_curatedHandler = curatedHandler;
			_httpClient = client;
			_logger = logger;
		}

		public CuratedController(IJokeHttpClient client, ILogger<CuratedController> logger) : this(client, logger, new CuratedHandler(logger))
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
		[HttpGet("{searchTerm}")]
		public IActionResult GetCuratedJokes(string searchTerm)
		{
			if (string.IsNullOrWhiteSpace(searchTerm))
			{
				var errorMessage = "The searchTerm cannot be empty or whitespace";
				_logger.Log(LogLevel.Debug, new Exception(errorMessage), errorMessage);

				return BadRequest(errorMessage);
			}

			var result = _httpClient.Get($"search?limit=30&term={searchTerm}").Result.Replace(@"\r\n", " ");
			var jokes = JsonSerializer.Deserialize<JokesIncoming>(result);

			var curatedJokes = _curatedHandler.CurateJokes(jokes.Jokes, searchTerm);
			
			_logger.Log(LogLevel.Critical,  JsonSerializer.Serialize(curatedJokes));

			return Ok(curatedJokes);
		}

	}
}