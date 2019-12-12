using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Web;
using Joked.Model;
using Microsoft.AspNetCore.Mvc;

namespace Joked.Controllers
{

	[ApiController]
	[Route("[controller]")]
	public class CuratedController : ControllerBase
	{
		private const int MediumJokeMinLength = 10;
		private const int LongJokeMinLength = 20;
		private const string BeginEmphasis = "*";
		private const string EndEmphasis = "*";

		private readonly IJokeHttpClient _httpClient;
		public CuratedController(IJokeHttpClient client)
		{
			_httpClient = client;
		}

		/// <summary>
		/// Accept a search term and display the first 30 jokes containing that term,
		/// with the matching term emphasized in some way (upper, bold, color, etc.)
		/// and the matching jokes grouped by length: short (<10 words), medium (<20 words), long (>= 20 words)
		/// https://icanhazdadjoke.com/api
		/// </summary>
		[HttpGet("{searchTerm}")]
		public IActionResult GetCuratedJokes(string searchTerm)
		{
			var result = _httpClient.Get($"search?limit=30&term={searchTerm}").Result;
			
			var jokes = JsonSerializer.Deserialize<JokesIncoming>(result);

			var emphasizedJokes = EmphasizeSearchTerm(searchTerm, jokes);

			var curatedJokes = CurateJokes(emphasizedJokes);
			
			return Ok(curatedJokes);
		}

		private static CuratedJokes CurateJokes(List<string> emphasizedJokes)
		{
			var curatedJokes = new CuratedJokes
			{
				ShortJokes = emphasizedJokes.Where(x => x.Split(' ').Length < MediumJokeMinLength).ToList(),
				MediumJokes = emphasizedJokes
					.Where(x => x.Split(' ').Length >= MediumJokeMinLength && x.Split(' ').Length < LongJokeMinLength).ToList(),
				LongJokes = emphasizedJokes.Where(x => x.Split(' ').Length < MediumJokeMinLength).ToList()
			};
			return curatedJokes;
		}

		private static List<string> EmphasizeSearchTerm(string searchTerm, JokesIncoming jokes)
		{
			var term = HttpUtility.UrlDecode(searchTerm);
			var highlightedTerms =
				jokes.Jokes.Select(x => x.Text.Replace(term, $"{BeginEmphasis}{term}{EndEmphasis}")).ToList();
			return highlightedTerms;
		}
	}
}