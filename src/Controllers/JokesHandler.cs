using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Web;
using Joked.Model;
using Microsoft.Extensions.Logging;

namespace Joked.Controllers
{
	internal class JokesHandler : IJokesHandler
	{
		private const int MediumJokeMinLength = 10;
		private const int LongJokeMinLength = 20;
		private const string BeginEmphasis = @"*";
		private const string EndEmphasis = @"&";
		private readonly IJokeHttpClient _httpClient;
		private readonly ILogger _logger;

		public JokesHandler(ILogger logger, IJokeHttpClient client)
		{
			_logger = logger;
			_httpClient = client;
		}

		public CuratedJokes CurateJokes(IJoke[] jokes, string term)
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
				Short = jokesText?.Where(x => LengthOfJoke(x) < MediumJokeMinLength).Select(x => Emphasize(x, term))
					.ToList(),
				Medium = jokesText
					?.Where(x => LengthOfJoke(x) >= MediumJokeMinLength && LengthOfJoke(x) < LongJokeMinLength)
					.Select(x => Emphasize(x, term)).ToList(),
				Long = jokesText?.Where(x => LengthOfJoke(x) >= LongJokeMinLength).Select(x => Emphasize(x, term))
					.ToList()
			};
			return curatedJokes;
		}

		public IJokes GetJokes(string term, int limit)
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

		internal string Emphasize(string jokeText, string term, string beginEmphasis = BeginEmphasis,
			string endEmphasis = EndEmphasis)
		{
			//search algorithm works like this
			//1 or 2 letters only full words ie(I, a, ah, we)
			//3 + beginning or end(no middle of word)
			//if multiple terms, all are searched for individually

			if (string.IsNullOrWhiteSpace(term) ||
			    string.IsNullOrWhiteSpace(beginEmphasis) && string.IsNullOrWhiteSpace(endEmphasis)) return jokeText;

			var emphasizedString = EmphasizeJoke(jokeText, term, beginEmphasis, endEmphasis);

			return string.Join(" ", emphasizedString);
		}

		private static List<string> EmphasizeJoke(string jokeText, string term, string beginEmphasis,
			string endEmphasis)
		{
			var termList = term.Split().OrderByDescending(x => x.Length).ToList();
			var splitJoke = jokeText.Split();
			var emphasizedString = new List<string>();

			foreach (var word in splitJoke)
				emphasizedString.Add(TryEmphasizeWord(beginEmphasis, endEmphasis, termList, word));

			return emphasizedString;
		}

		private static string TryEmphasizeWord(string beginEmphasis, string endEmphasis, IEnumerable<string> termList,
			string word)
		{
			var presentTerms = termList.Where(x => word.Contains(x, StringComparison.CurrentCultureIgnoreCase))
				.ToList();
			if (presentTerms.Count == 0) return word;

			var emphasisTrackerArray = BuildArrayToTrackEmphasis(word, presentTerms);

			var emphasizedString = EmphasizeString(beginEmphasis, endEmphasis, word, emphasisTrackerArray);

			return emphasizedString;
		}

		private static string EmphasizeString(string beginEmphasis, string endEmphasis, string word,
			char[] emphasisTrackerArray)
		{
			var isBold = false;
			var newWord = new List<char>();
			for (var i = 0; i < emphasisTrackerArray.Length; i++)
			{
				if (emphasisTrackerArray[i] == '*' && !isBold)
				{
					newWord.AddRange(beginEmphasis.ToCharArray());
					isBold = true;
				}
				else
				{
					if (emphasisTrackerArray[i] != '*' && isBold)
					{
						newWord.AddRange(endEmphasis.ToCharArray());
						isBold = false;
					}
				}

				newWord.Add(word[i]);
			}

			if (isBold) newWord.AddRange(endEmphasis.ToCharArray());

			var emphasizedString = new string(newWord.ToArray());
			return emphasizedString;
		}

		private static char[] BuildArrayToTrackEmphasis(string word, List<string> presentTerms)
		{
			var emphasisTrackerArray = new char[word.Length];

			char[] punctuationToTrim = {',', '.', '"', '?', '!', '\''};
			foreach (var term in presentTerms)
				if (term.Length < 3)
				{
					if (string.Equals(word.Trim(punctuationToTrim), term, StringComparison.InvariantCultureIgnoreCase))
						BuildEmphasisCharArray(Regex.Match(word, term, RegexOptions.IgnoreCase));
				}
				else
				{
					if (word.Trim(punctuationToTrim).StartsWith(term, StringComparison.InvariantCultureIgnoreCase))
						BuildEmphasisCharArray(Regex.Match(word, term, RegexOptions.IgnoreCase));

					if (word.Trim(punctuationToTrim).EndsWith(term, StringComparison.InvariantCultureIgnoreCase))
						BuildEmphasisCharArray(Regex.Match(word, term, RegexOptions.IgnoreCase));
				}

			void BuildEmphasisCharArray(Match matchExpression)
			{
				for (var i = 0; i < matchExpression.Length; i++) emphasisTrackerArray[matchExpression.Index + i] = '*';
			}

			return emphasisTrackerArray;
		}
	}

	internal interface IJokesHandler
	{
		CuratedJokes CurateJokes(IJoke[] jokes, string term);
		IJokes GetJokes(string term, int limit);
	}
}