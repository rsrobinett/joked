using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Joked.Model;
using Microsoft.Extensions.Logging;

namespace Joked.Controllers
{
	internal class CuratedHandler : ICuratedHandler
	{
		private ILogger _logger;
		private const int MediumJokeMinLength = 10;
		private const int LongJokeMinLength = 20;
		private const string BeginEmphasis = "*";
		private const string EndEmphasis = "*";

		
		public CuratedHandler(ILogger logger)
		{
			_logger = logger;
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
				Short = jokesText?.Where(x => LengthOfJoke(x) < MediumJokeMinLength).Select(x=>EmphasizeTerm(x,term)).ToList(),
				Medium = jokesText?.Where(x => LengthOfJoke(x) >= MediumJokeMinLength && LengthOfJoke(x) < LongJokeMinLength).Select(x => EmphasizeTerm(x, term)).ToList(),
				Long = jokesText?.Where(x => LengthOfJoke(x) >= LongJokeMinLength).Select(x => EmphasizeTerm(x, term)).ToList()
			};
			return curatedJokes;

		}
		
		internal int LengthOfJoke(string phrase)
		{
			const string regexSplitExpression = @"[^[\d|\p{L}]*\p{Z}|--[^[\d|\p{L}]*";
			return Regex.Split(phrase??"", regexSplitExpression).Length;
		}

		internal string EmphasizeTerm(string jokeText, string term,string beginEmphasis = BeginEmphasis, string endEmphasis = EndEmphasis )
		{
			//var regexReplaceTerm = $@"{beginEmphasis}$1{endEmphasis}";
			var regexReplaceTerm = $@"{beginEmphasis}$1{endEmphasis}";
			var regexTermSearch = $@"\b({HttpUtility.UrlDecode(term)})\b";
			return Regex.Replace(jokeText, regexTermSearch, regexReplaceTerm, RegexOptions.IgnoreCase);
		}
	}

	internal interface ICuratedHandler
	{
		CuratedJokes CurateJokes(JokeIncoming[] jokes, string term);
	}
}
