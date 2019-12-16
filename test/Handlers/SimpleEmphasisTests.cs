using FluentAssertions;
using Joked.Handlers;
using NUnit.Framework;

namespace Joked.Test.Handlers
{
	class SimpleEmphasisTests
	{
	
		[Test]
		[TestCase("hello", "hello", "*", "*", "*hello*")]
		[TestCase("Hello", "hello", "*", "*", "*Hello*")]
		[TestCase("hello", "Hello", "*", "*", "*hello*")]
		[TestCase("hello", "hello", "<tag>", "</tag>", "<tag>hello</tag>")]
		[TestCase("Hello", "hello", "<tag>", "</tag>", "<tag>Hello</tag>")]
		[TestCase("hello", "Hello", "<tag>", "</tag>", "<tag>hello</tag>")]
		[TestCase("1 2 3 4", "2", "*", "*", "1 *2* 3 4")]
		[TestCase("Hello today is a good day to say hello!", "hello", "*", "*", "*Hello* today is a good day to say *hello*!")]
		[TestCase("Well hello there.", "Hello", "*", "*", "Well *hello* there.")]
		[TestCase("What a wonderful day.", "a", "*", "*", "Wh*a*t *a* wonderful d*a*y.")]
		[TestCase("I'm Okay, I say.", "I", "*", "*", "*I*'m Okay, *I* say.")]
		[TestCase("I'm Okay, I say.", "I'm", "*", "*", "*I'm* Okay, I say.")]
		[TestCase("www.degreed.com degreed.", "degreed", "*", "*", "www.*degreed*.com *degreed*.")]
		[TestCase("Me enjoy a good pick-me-up first thing in the morning", "me", "*", "*", "*Me* enjoy a good pick-*me*-up first thing in the morning")]
		[TestCase("Hello", "llo", "*", "*", "He*llo*")]
		[TestCase("Hello", "Hel", "*", "*", "*Hel*lo")]
		[TestCase("Hello", "ello", "*", "*", "H*ello*")]
		[TestCase("Hello my name is", "", "*", "*", "Hello my name is")]
		[TestCase("Hello!", "llo", "*", "*", "He*llo*!")]
		[TestCase("I!", "i", "*", "*", "*I*!")]
		[TestCase("Long phrases should be matched also", "phrases shoulD be matched", "*", "*", "Long *phrases should be matched* also")]
		[TestCase("Near matches won't emphasize", "near matches wont emphasize", "*", "*", "Near matches won't emphasize")]
		public void ShouldEmphasizeSearchTerm(string givenJoke, string givenTerm, string beginEmph, string endEmph, string expectedEmphasizedJoke)
		{

			WhenEmphasizeTermsCalled(givenJoke, givenTerm, beginEmph, endEmph);
			ThenExpectedStringIsReturned(expectedEmphasizedJoke, givenTerm);
		}


		private void WhenEmphasizeTermsCalled(string jokeText, string term, string beginEmphasis, string endEmphasis)
		{
			_thenEmphasizedTerm = _simpleEmphasis.Emphasize(jokeText, term, beginEmphasis, endEmphasis);
		}

		private void ThenExpectedStringIsReturned(string expectedEmphasizedJoke, string because)
		{
			_thenEmphasizedTerm.Should().Be(expectedEmphasizedJoke, because);
		}

		[SetUp]
		public void Setup()
		{ 
			_simpleEmphasis = new SimpleEmphasis();
		}

		private SimpleEmphasis _simpleEmphasis;
		private string _thenEmphasizedTerm;
	}
}
