﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using Joked.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Joked.Controllers
{
	internal class CuratedHandler : ICuratedHandler
	{
		private readonly ILogger _logger;
		private readonly IJokeHttpClient _httpClient;
		private const int MediumJokeMinLength = 10;
		private const int LongJokeMinLength = 20;
		private const string BeginEmphasis = "*";
		private const string EndEmphasis = "</strong>";
		
		public CuratedHandler(ILogger logger, IJokeHttpClient client)
		{
			_logger = logger;
			_httpClient = client;
		}
		public CuratedJokes CurateJokes(JokeIncoming[] jokes, string term)
		{
			if (jokes == null)
			{
				var ErrorMessage = "jokes parameter cannot be null";
				_logger.Log(LogLevel.Error,new ArgumentNullException(nameof(jokes)), ErrorMessage);

				return new CuratedJokes();
			}

			var jokesText = jokes?.Select(x => x?.Text).ToArray();

			var curatedJokes = new CuratedJokes
			{
				Short = jokesText?.Where(x => LengthOfJoke(x) < MediumJokeMinLength).Select(x=>Emphasize(x,term)).ToList(),
				Medium = jokesText?.Where(x => LengthOfJoke(x) >= MediumJokeMinLength && LengthOfJoke(x) < LongJokeMinLength).Select(x => Emphasize(x, term)).ToList(),
				Long = jokesText?.Where(x => LengthOfJoke(x) >= LongJokeMinLength).Select(x => Emphasize(x, term)).ToList()
			};
			return curatedJokes;
		}
		
		internal int LengthOfJoke(string phrase)
		{
			const string regexSplitExpression = @"[^[\d|\p{L}]*\p{Z}|--[^[\d|\p{L}]*";
			return Regex.Split(phrase??"", regexSplitExpression).Length;
		}

		internal string Emphasize(string jokeText, string term,string beginEmphasis = BeginEmphasis, string endEmphasis = EndEmphasis )
		{
			//search algorithm works like this
			//1 or 2 letters only full words ie (I, a, ah, we)
			//3+ beginning or end (no middle of word)
			//if multiple terms, all are searched for individually

			if (string.IsNullOrWhiteSpace(term))
			{
				return jokeText;
			}

			//choosing to emphasize full words (includes quotation attached to a word).  
			var termList = term.Split().OrderByDescending(x => x.Length).ToList();
			var splitJoke = jokeText.Split();
			var emphasizedString = new List<string>();

			foreach (var word in splitJoke)
			{
				EmphasizeWord(beginEmphasis, endEmphasis, termList, word, emphasizedString);
			}

			return string.Join(" ", emphasizedString);
		}

		private static void EmphasizeWord(string beginEmphasis, string endEmphasis, IEnumerable<string> termList, string word,
			ICollection<string> emphasizedString)
		{
			foreach (var t in termList)
			{
				if (t.Length < 3)
				{
					if (word.Equals(t, StringComparison.CurrentCultureIgnoreCase))
					{
						emphasizedString.Add(Emphasized(word));
						return;
					}

					emphasizedString.Add(word);
					return;
				}

				if (word.StartsWith(t, StringComparison.CurrentCultureIgnoreCase))
				{
					emphasizedString.Add(Emphasized(word));
					return;
				}

				if (word.EndsWith(t, StringComparison.CurrentCultureIgnoreCase))
				{
					emphasizedString.Add(Emphasized(word));
					return;
				}

			}

			emphasizedString.Add(word);

			string Emphasized(string emphasizeThis)
			{
				return $"{beginEmphasis}{emphasizeThis}{endEmphasis}";
			}
		}

		public JokesIncoming GetJokes(string term, int limit)
		{
			var request = _httpClient.Get($"search?limit={limit}&term={term}").Result.Replace(@"\r\n", Environment.NewLine);
			var jokes = JsonSerializer.Deserialize<JokesIncoming>(request);
			return jokes;
		}
	}

	internal interface ICuratedHandler
	{
		CuratedJokes CurateJokes(JokeIncoming[] jokes, string term);
		JokesIncoming GetJokes(string term, int limit);
	}
}
