using System;
using System.Text.Json;
using System.Threading.Tasks;
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
		private readonly ICuratedHandler _curatedHandler;
		private readonly IHubContext<JokeHub> _hub;

		internal JokesController(IJokeHttpClient client, ILogger<JokesController> logger, ICuratedHandler curatedHandler, IHubContext<JokeHub> hub)
		{
			_curatedHandler = curatedHandler;
			_httpClient = client;
			_logger = logger;
			_hub = hub;
		}

		public JokesController(IJokeHttpClient client, ILogger<JokesController> logger, IHubContext<JokeHub> hub) : this(client, logger, new CuratedHandler(logger), hub)
		{
		
		}

		//[HttpGet("random")]
		//public IActionResult GetRandomJokes()
		//{
		//	_hub.Clients.All.SendAsync("transferjokedata", GetRandomJoke());
		//	return Ok(new { Message = "Request Completed" });
		//}

		//public async Task<string> GetRandomJoke()
		//{
		//	return await _httpClient.Get("/");
		//}

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
			
			_logger.Log(LogLevel.Critical,  JsonSerializer.Serialize(curatedJokes));

			return Ok(curatedJokes);
		}

	}
}