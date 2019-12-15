using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Joked.Controllers;
using Joked.Model;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Joked.Test.Controllers
{
	public class CuratedHandlerTests
	{

		[Test]
		public void ShouldHandleNullJokes()
		{
			GivenNullJokes();
			WhenJokesAreCurated();
			ThenCuratedJokesIsEmpty();
		}
		
		[Test]
		[TestCaseSource(nameof(_jokeGroupingTestCases))]
		public void ShouldGroupList(int expectdShortJokeCount, int expextedMediumJokeCount, int expectedLongJokeCount, params string[] jokesText)
		{
			GivenJokes(jokesText);
			WhenJokesAreCurated();
			ThenCuratedJokesArGroupedAsExpected(expectdShortJokeCount, expextedMediumJokeCount, expectedLongJokeCount);
		}
		
		[Test]
		[TestCase(" ")]
		[TestCase("*")]
		[TestCase("why")]
		[TestCase("a")]
		[TestCase("one")]
		[TestCase("9")]
		[TestCase("one two")]
		public void ShouldGroupWithAnyReasonableTerm(string term)
		{
			GivenSearchTerm(term);
			GivenJokes(_shortJoke,_mediumJoke, _longJoke);
			WhenJokesAreCurated();
			ThenCuratedJokesArGroupedAsExpected(1, 1, 1);
		}

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

		public void ShouldEmphasizeSearchTerm(string givenJoke, string givenTerm, string beginEmph, string endEmph, string expectedEmphasizedJoke)
		{
			GivenSearchTerm(givenTerm);
			WhenEmphasizeTermsCalled(givenJoke, givenTerm, beginEmph, endEmph);
			ThenExpectedStringIsReturned(expectedEmphasizedJoke, givenTerm);
		}

		[Test]
		[TestCase("this is my first joke.", 5)]
		[TestCase("this is my joke 2.", 5)]
		[TestCase("this is my joke # 3.", 5)]
		[TestCase("this is my 4th joke.", 5)]
		[TestCase("this is my @$#^$ grawlix joke.", 5)]
		[TestCase("this is my pike-me-up joke.", 5)]
		[TestCase("this is my exclamatory !!!!! joke.", 5, Ignore = "Edge case: Should !!!!! count as a word?")]
		[TestCase("this is my emphasis--dash joke.", 6, Ignore= "Edge case: Should emphasis dash make 2 words count as one?")]
		[TestCase("I'm joking.", 2)]
		public void ShouldCountWords(string joke, int expectedLength)
		{
			GivenJoke(joke);
			WhenMeasuringJokeLength();
			ThenJokeLengthIs(expectedLength, joke);
		}
		
		#region givens
		private void GivenJoke(string thisIsMyJoke)
		{
			_givenJoke = thisIsMyJoke;
		}
		
		private void GivenSearchTerm(string term)
		{
			_givenTerm = term;
		}

		private void GivenJokes(params string[] jokes)
		{
			_givenJokes = new List<IJoke>();
			foreach (var joke in jokes)
			{
				_givenJokes.Add( new JokeDto { Joke = joke});
			}
		}

		private void GivenJokes(params IJoke[] jokes)
		{
			_givenJokes.AddRange(jokes.ToList());
		}

		private void GivenNullJokes()
		{
			_givenJokes = null;
		}

		#endregion

		#region whens
		private void WhenMeasuringJokeLength()
		{
			_thenJokeLength = _curatedHandler.LengthOfJoke(_givenJoke);
		}

		private void WhenEmphasizeTermsCalled(string jokeText, string term, string beginEmphasis, string endEmphasis)
		{
			_thenEmphasizedTerm = _curatedHandler.Emphasize(jokeText, term, beginEmphasis, endEmphasis);
		}

		#endregion

		#region thens

		private void WhenJokesAreCurated()
		{
			_thenCuratedJokes = _curatedHandler.CurateJokes(_givenJokes?.ToArray(), _givenTerm);
		}
		private void ThenCuratedJokesIsEmpty()
		{
			_thenCuratedJokes.Should().BeEquivalentTo(new CuratedJokes());
		}
		private void ThenJokeLengthIs(int expectedJokeLength, string joke)
		{
			_thenJokeLength.Should().Be(expectedJokeLength,joke);
		}
		private void ThenExpectedStringIsReturned(string expectedEmphasizedJoke, string because)
		{
			_thenEmphasizedTerm.Should().Be(expectedEmphasizedJoke, because);
		}

		private void ThenCuratedJokesArGroupedAsExpected(int expectdShortJokeCount, int expextedMediumJokeCount,
			int expectedLongJokeCount)
		{
			_thenCuratedJokes.Short.Count.Should().Be(expectdShortJokeCount);
			_thenCuratedJokes.Medium.Count.Should().Be(expextedMediumJokeCount);
			_thenCuratedJokes.Long.Count.Should().Be(expectedLongJokeCount);
		}

		#endregion

		#region setup

		private IJokeHttpClient _httpClient;
		private JokesHandler _curatedHandler;
		private ILogger<JokesHandler> _logger;
		private List<IJoke> _givenJokes;
		private CuratedJokes _thenCuratedJokes;
		private string _givenTerm = "default";
		private string _thenEmphasizedTerm;

		[SetUp]
		public void Setup()
		{
			_givenJokes = new List<IJoke>();
			_logger = new Mock<ILogger<JokesHandler>>().Object;
			_httpClient = new Mock<IJokeHttpClient>().Object;
			_curatedHandler = new JokesHandler(_logger, _httpClient);
		}

		private static object[] _jokeGroupingTestCases =
		{
			
			new object[] {1, 0, 0, new[] {JokeLength1}},
			new object[] {1, 0, 0, new[] {JokeLength9}},
			new object[] {0, 1, 0, new[] {JokeLength10}},
			new object[] {0, 1, 0, new[] {JokeLength11}},
			new object[] {0, 1, 0, new[] {JokeLength19}},
			new object[] {0, 0, 1, new[] {JokeLength20}},
			new object[] {0, 0, 1, new[] {JokeLength21}},
			new object[] {2, 3, 2, new[] { JokeLength1, JokeLength9, JokeLength10, JokeLength11, JokeLength19, JokeLength20, JokeLength21 } },
		};

		private const string JokeLength21 =
			"one two three four five six seven eight nine ten eleven twelve thirteen fourteen fifteen sixteen seventeen eighteen nineteen twenty twenty1";
		private const string JokeLength20 =
			"one two three four five six seven eight nine ten eleven twelve thirteen fourteen fifteen sixteen seventeen eighteen nineteen twenty";
		private const string JokeLength19 =
			"one two three four five six seven eight nine ten eleven twelve thirteen fourteen fifteen sixteen seventeen eihteen nineteen";
		private const string JokeLength11 = "one two three four five six seven eight nine ten eleven";

		private const string JokeLength10 = "one two three four five six seven eight nine ten";
		private const string JokeLength9 = "one two three four five six seven eight nine";
		private const string JokeLength1 = "one";
		
		private readonly IJoke _shortJoke = new JokeDto(){ Joke = JokeLength9};
		private readonly IJoke _mediumJoke = new JokeDto() { Joke = JokeLength11 };
		private readonly IJoke _longJoke = new JokeDto() { Joke = JokeLength21 };
		private string _givenJoke;
		private int _thenJokeLength;

		#endregion
	}
}