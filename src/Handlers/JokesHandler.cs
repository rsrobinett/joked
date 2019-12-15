using System;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Web;
using Joked.Model;
using Microsoft.Extensions.Logging;

namespace Joked.Handlers
{
	internal class JokesHandler : IJokesHandler
	{
		private const int MediumJokeMinLength = 10;
		private const int LongJokeMinLength = 20;
		private readonly IJokeHttpClient _httpClient;
		private readonly ILogger _logger;
		private readonly IEmphasize _emphasizer;

		public JokesHandler(ILogger logger, IJokeHttpClient client, IEmphasize emphasizer = null)
		{
			_logger = logger;
			_httpClient = client;
			_emphasizer = emphasizer ?? new Emphasizer();
		}

		public CuratedJokes CurateJokes(JokeDto[] jokes, string term, bool shouldEmphasize = false)
		{
			if (jokes == null)
			{
				var ErrorMessage = "jokes parameter cannot be null";
				_logger.Log(LogLevel.Error, new ArgumentNullException(nameof(jokes)), ErrorMessage);

				return new CuratedJokes();
			}

			var jokesText = jokes?.Select(x => x?.Joke).ToArray();

			term = HttpUtility.UrlDecode(term);

			var curatedJokes = new CuratedJokes
			{
				Short = jokesText?.Where(x => LengthOfJoke(x) < MediumJokeMinLength)
					.Select(x =>TryEmphasize(term, shouldEmphasize, x)).ToList(),
				Medium = jokesText?.Where(x => LengthOfJoke(x) >= MediumJokeMinLength && LengthOfJoke(x) < LongJokeMinLength)
					.Select(x => TryEmphasize(term, shouldEmphasize, x)).ToList(),
				Long = jokesText?.Where(x => LengthOfJoke(x) >= LongJokeMinLength).
					Select(x => TryEmphasize(term, shouldEmphasize, x))
					.ToList()
			};
			return curatedJokes;
		}

		private string TryEmphasize(string term, bool shouldEmphasize, string x)
		{
			return shouldEmphasize ? _emphasizer.Emphasize(x, term) : x;
		}

		public JokesDto GetJokes(string term, int limit)
		{
			var request = _httpClient.Get($"search?limit={limit}&term={term}").Result.Replace(@"\r\n", " ");
			var jokes = JsonSerializer.Deserialize<JokesDto>(request);
			return jokes;
		}

		internal int LengthOfJoke(string phrase)
		{
			const string regexSplitExpression = @"[^[\|\d|\p{L}|']*\p{Z}[^[\d|\p{L}|']*|--*/gmiXx";
			return Regex.Split(phrase ?? "", regexSplitExpression).Length;
		}
	}

	internal interface IJokesHandler
	{
		CuratedJokes CurateJokes(JokeDto[] jokes, string term, bool shouldEmphasize);
		JokesDto GetJokes(string term, int limit);
	}
}