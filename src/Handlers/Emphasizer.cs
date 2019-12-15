using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static System.String;

namespace Joked.Handlers
{
	interface IEmphasize
	{
		string Emphasize(string jokeText, string term);
	}
	internal class Emphasizer : IEmphasize
	{
		string BeginEmphasis = @"*";
		string EndEmphasis = @"&";

		public string Emphasize(string jokeText, string term)
		{
			return Emphasize(jokeText, term, BeginEmphasis, EndEmphasis);
		}

		internal string Emphasize(string jokeText, string term, string beginEmphasis,
	string endEmphasis)
		{
			//search algorithm works like this
			//1 or 2 letters only full words ie(I, a, ah, we)
			//3 + beginning or end(no middle of word)
			//if multiple terms, all are searched for individually

			if (IsNullOrWhiteSpace(term) ||
				IsNullOrWhiteSpace(beginEmphasis) && IsNullOrWhiteSpace(endEmphasis)) return jokeText;

			var emphasizedString = EmphasizeJoke(jokeText, term, beginEmphasis, endEmphasis);

			return Join(" ", emphasizedString);
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

			char[] punctuationToTrim = { ',', '.', '"', '?', '!', '\'' };
			foreach (var term in presentTerms)
				if (term.Length < 3)
				{
					if (String.Equals(word.Trim(punctuationToTrim), term, StringComparison.InvariantCultureIgnoreCase))
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
}
