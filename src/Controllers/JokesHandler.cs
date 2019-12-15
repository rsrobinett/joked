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
		private readonly ILogger _logger;
		private readonly IJokeHttpClient _httpClient;
		private const int MediumJokeMinLength = 10;
		private const int LongJokeMinLength = 20;
		private const string BeginEmphasis = @"*";
		private const string EndEmphasis = @"&";

		public JokesHandler(ILogger logger, IJokeHttpClient client)
		{
			_logger = logger;
			_httpClient = client;
		}

		public CuratedJokes CurateJokes(JokeIncoming[] jokes, string term)
		{
			if (jokes == null)
			{
				var ErrorMessage = "jokes parameter cannot be null";
				_logger.Log(LogLevel.Error, new ArgumentNullException(nameof(jokes)), ErrorMessage);

				return new CuratedJokes();
			}

			var jokesText = jokes?.Select(x => x?.Text).ToArray();

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

			if (string.IsNullOrWhiteSpace(term) || (string.IsNullOrWhiteSpace(beginEmphasis) && string.IsNullOrWhiteSpace(endEmphasis)))
			{
				return jokeText;
			}

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
			{
				emphasizedString.Add(TryEmphasizeWord(beginEmphasis, endEmphasis, termList, word));
			}

			return emphasizedString;
		}

		private static string TryEmphasizeWord(string beginEmphasis, string endEmphasis, IEnumerable<string> termList,
			string word)
		{
			char[] punctuationToTrim = { ',', '.', '"', '?', '!', '\'' };
			var presentTerms = termList.Where(x=>word.Contains(x,StringComparison.CurrentCultureIgnoreCase)).ToList();

			if (presentTerms.Count == 0)
			{
				return word;
			}

			if (presentTerms.Count == 1)
			{
				var singleTerm = presentTerms.First();

				if (singleTerm.Length < 3)
				{
					
					if (String.Equals(word.Trim(punctuationToTrim), singleTerm, StringComparison.InvariantCultureIgnoreCase))
					{
						return Regex.Replace(word, singleTerm, LocalReplaceMatchCase, RegexOptions.IgnoreCase);
					}

					return word;
				}
				
				if (word.Trim(punctuationToTrim).StartsWith(singleTerm, StringComparison.InvariantCultureIgnoreCase))
				{
					return Regex.Replace(word, singleTerm, LocalReplaceMatchCase, RegexOptions.IgnoreCase);
				}

				if (word.Trim(punctuationToTrim).EndsWith(singleTerm, StringComparison.InvariantCultureIgnoreCase))
				{
					return Regex.Replace(word, singleTerm, LocalReplaceMatchCase, RegexOptions.IgnoreCase);
				}

				return word;
			}
			
			var charWordArray = word.ToCharArray();
			
			foreach (var t in presentTerms)
			{
				Regex.Replace(word, t, BuildEmphasisCharArray, RegexOptions.IgnoreCase);
			}

			var isBold = false;

			List<char> newWord = new List<char>();
			for (int i = 0; i < charWordArray.Length; i++)
			{
				if (charWordArray[i] == '*' && !isBold)
				{
					newWord.AddRange(beginEmphasis.ToCharArray());
					isBold = true;
				}
				else
				{
					if (charWordArray[i] != '*' && isBold)
					{
						newWord.AddRange(endEmphasis.ToCharArray());
						isBold = false;
					}
				}

				newWord.Add(word[i]);
			}

			if (isBold)
			{
				newWord.AddRange(endEmphasis.ToCharArray());
			}

			return new string(newWord.ToArray());

			
			string BuildEmphasisCharArray(Match matchExpression)
			{
				for (var i = 0; i < matchExpression.Length; i++)
				{
					charWordArray[matchExpression.Index + i] = '*';
				}
				
				return matchExpression.Value;
			}

			string LocalReplaceMatchCase(Match matchExpression)
			{
				return $"{beginEmphasis}{matchExpression.Value}{endEmphasis}";
			}
			
		}
	
		
		public JokesIncoming GetJokes(string term, int limit)
		{
			var request = _httpClient.Get($"search?limit={limit}&term={term}").Result.Replace(@"\r\n", " ");
			var jokes = JsonSerializer.Deserialize<JokesIncoming>(request);
			return jokes;
		}

	}

	internal interface IJokesHandler
	{
		CuratedJokes CurateJokes(JokeIncoming[] jokes, string term);
		JokesIncoming GetJokes(string term, int limit);
	}
}
