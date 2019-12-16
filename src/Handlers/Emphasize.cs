using System.Text.RegularExpressions;
using static System.String;

namespace Joked.Handlers
{
	/// <summary>
	/// Adds Emphasis strings around the search term.
	/// </summary>
	interface IEmphasize
	{
		string Emphasize(string jokeText, string term);
	}

	/// <summary>
	/// Adds Emphasis strings around the search term.
	/// It finds the whole term in the string and replaces the whole term if it exists.
	/// If the string is multiple terms long, it replaces the string only when there is an exact match including whitespace for the search term.
	/// </summary>
	internal class SimpleEmphasis : IEmphasize
	{
		private string _beginEmphasis = @"*";
		private string _endEmphasis = @"&";

		public string Emphasize(string jokeText, string term)
		{
			return Emphasize(jokeText, term, _beginEmphasis, _endEmphasis);
		}

		internal string Emphasize(string jokeText, string term, string beginEmphasis, string endEmphasis)
		{
			if (IsNullOrWhiteSpace(term)) return jokeText;
			
			_beginEmphasis = !IsNullOrWhiteSpace(beginEmphasis) ? beginEmphasis : _beginEmphasis;
			_endEmphasis = !IsNullOrWhiteSpace(endEmphasis) ? endEmphasis : _endEmphasis;
			
			return Regex.Replace(jokeText, term, LocalReplaceMatchCase, RegexOptions.IgnoreCase);
			
			string LocalReplaceMatchCase(Match matchExpression)
			{
				return $"{_beginEmphasis}{matchExpression.Value}{_endEmphasis}";
			}
		}
	}
}
