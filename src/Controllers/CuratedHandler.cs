using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Joked.Model;
using Microsoft.EntityFrameworkCore.Query;
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
			//search algorithm works like this
			//1 or 2 letters only full words ie (I, a, ah, we)
			//3+ beginning or end (no middle of word)
			//if multiple terms, all are searched for individually

			//choosing to emphasize full words (includes quotation attached to a word).  

			var termList = term.Split(' ').OrderByDescending(x => x.Length).ToList();
			var splitJoke = jokeText.Split(' ');
			var emphasizedString = new List<string>();

			foreach (var s in splitJoke)
			{
				foreach (var t in termList)
				{
					if (t.Length < 3)
					{
						if (s.Equals( t, StringComparison.CurrentCultureIgnoreCase))
						{
							emphasizedString.Add(beginEmphasis + s + endEmphasis);
							break;
						}

						emphasizedString.Add(s);
						break;
					}

					if (s.StartsWith(t, StringComparison.CurrentCultureIgnoreCase))
					{
						emphasizedString.Add(beginEmphasis + s + endEmphasis);
						break;
					}
					if (s.EndsWith(t, StringComparison.CurrentCultureIgnoreCase))
					{
						emphasizedString.Add(beginEmphasis + s + endEmphasis);
						break;
					}

					emphasizedString.Add(s);
				}
			}

			return string.Join(" ", emphasizedString);

			//var t = HttpUtility.UrlDecode(term);
			//var x = jokeText.Split(' ');
			//var y = x.Select(z =>
			//{
			//	//if (z.ToLower().Contains(t.ToLower()))
			//	if (z.ToLower()==(t.ToLower()))
			//	{
			//		//var regexTermSearch = $@"\b({t})\b";
			//		//var w = Regex.Replace(jokeText, regexTermSearch, z, RegexOptions.IgnoreCase);
			//		//var w = string.Replace(x, beginEmphasis + z + endEmphasis);
			//		var w = beginEmphasis + z + endEmphasis;
			//		return w;
			//	}
			//	else
			//	{
			//		return z;
			//	}
			//});

			//return string.Join(" ", y);


			//const string regexSplitExpression = @"[^[\d|\p{L}]*\p{Z}|--[^[\d|\p{L}]*";
			//Regex.Split(phrase ?? "", regexSplitExpression);
			//var regexReplaceTerm = $@"{beginEmphasis}$1{endEmphasis}";

			//var regexReplaceTerm = $@"{beginEmphasis}$1{endEmphasis}";
			//var regexTermSearch = $@"\b({t})\b";
			//var regexTermSearch = $@"(?=\S*['-])([{t}'-]+)";
			//var regexTermSearch = $@"(?=\S*['-])([{t}]+)";
			//return Regex.Replace(jokeText, regexTermSearch, regexReplaceTerm, RegexOptions.IgnoreCase);

			//var termList = term.Split().OrderByDescending(x=>x.Length);

			//foreach (var t in termList)
			//{ 
			//	jokeText = jokeText.Replace(t, beginEmphasis + t + endEmphasis);
			//}

			//return jokeText;
		}
	}

	internal interface ICuratedHandler
	{
		CuratedJokes CurateJokes(JokeIncoming[] jokes, string term);
	}
}
