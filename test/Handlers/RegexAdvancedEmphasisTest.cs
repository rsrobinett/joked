using FluentAssertions;
using Joked.Handlers;
using NUnit.Framework;

namespace Joked.Test.Handlers
{
	class RegexAdvancedEmphasisTest
	{

		//[Test, Ignore("attempting a better regex replace, but still working on the details")]
		[TestCase("hello", "hello", "*", "*", "*hello*")]
		[TestCase("Hello", "hello", "*", "*", "*Hello*")]
		[TestCase("hello", "Hello", "*", "*", "*hello*")]
		[TestCase("hello", "hello", "<tag>", "</tag>", "<tag>hello</tag>")]
		[TestCase("Hello", "hello", "<tag>", "</tag>", "<tag>Hello</tag>")]
		[TestCase("hello", "Hello", "<tag>", "</tag>", "<tag>hello</tag>")]
		[TestCase("1 2 3 4", "2", "*", "*", "1 *2* 3 4")]
		[TestCase("Hello today is a good day to say hello!", "hello", "*", "*", "*Hello* today is a good day to say *hello*!")]
		[TestCase("Well hello there.", "Hello", "*", "*", "Well *hello* there.")]
		[TestCase("What a wonderful day.", "a", "*", "*", "What *a* wonderful day.")]
		[TestCase("I'm Okay, I say.", "I", "*", "*", "I'm Okay, *I* say.")]
		[TestCase("I'm Okay, I say.", "I'm", "*", "*", "*I'm* Okay, I say.")]
		[TestCase("www.degreed.com degreed.", "degreed", "*", "*", "www.degreed.com *degreed*.")]
		[TestCase("Me enjoy a good pick-me-up first thing in the morning", "me", "*", "*", "*Me* enjoy a good pick-me-up first thing in the morning")]
		[TestCase("Hello", "hello hel llo", "*", "*", "*Hello*")]
		[TestCase("Hello", "llo", "*", "*", "He*llo*")]
		[TestCase("Hello", "Hel", "*", "*", "*Hel*lo")]
		[TestCase("Hello", "ello", "*", "*", "H*ello*")]
		[TestCase("Hello my name is", "nam is llo m hello", "*", "*", "*Hello* my *nam*e *is*")]
		[TestCase("Hello my name is", "", "*", "*", "Hello my name is")]
		[TestCase("Hello!", "llo", "*", "*", "He*llo*!")]
		[TestCase("I!", "i", "*", "*", "*I*!")]
		[TestCase("Password", "pass sword", "*", "*", "*Password*")]
		[TestCase("Password", "pass ord", "*", "*", "*Pass*w*ord*")]
		[TestCase("Passwords", "pass ord", "*", "*", "*Pass*words")]
		[TestCase("Passwordsverylongstringofwords", "pass ords", "*", "*", "*Pass*wordsverylongstringofw*ords*", Ignore = "Very edge case. Only happens when a search term is at the end or beginning of a word and in the middle of the word.")]
		public void ShouldEmphasizeSearchTerm(string givenJoke, string givenTerm, string beginEmph, string endEmph, string expectedEmphasizedJoke)
		{

			WhenEmphasizeTermsCalled(givenJoke, givenTerm, beginEmph, endEmph);
			ThenExpectedStringIsReturned(expectedEmphasizedJoke, givenTerm);
		}


		private void WhenEmphasizeTermsCalled(string jokeText, string term, string beginEmphasis, string endEmphasis)
		{
			_thenEmphasizedTerm = _emphasizer.Emphasize(jokeText, term, beginEmphasis, endEmphasis);
		}

		private void ThenExpectedStringIsReturned(string expectedEmphasizedJoke, string because)
		{
			_thenEmphasizedTerm.Should().Be(expectedEmphasizedJoke, because);
		}

		[SetUp]
		public void Setup()
		{
			_emphasizer = new RegexAdvancedEmphasis();
		}

		private RegexAdvancedEmphasis _emphasizer;
		private string _thenEmphasizedTerm;
	}
}
