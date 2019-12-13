using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using Joked.Controllers;
using Joked.Model;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Joked.Test.Controllers
{
	public class CuratedHandlerTests
	{
		private CuratedHandler _curatedHandler;
		private ILogger<CuratedHandler> _logger;
		private List<JokeIncoming> _givenJokes;
		private CuratedJokes _thenCuratedJokes;
		private string _givenTerm = "default";
		private string _thenEmphasizedTerm;

		[SetUp]
		public void Setup()
		{
			_givenJokes = new List<JokeIncoming>();
			_logger = new Mock<ILogger<CuratedHandler>>().Object;
			_curatedHandler = new CuratedHandler(_logger);
		}

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

		//[Test]
		//[TestCaseSource(nameof(_jokeGroupingTestCases2))]
		//public void ShouldCurateJokesShouldGroupList2(int expectdShortJokeCount, int expextedMediumJokeCount, int expectedLongJokeCount, params JokeIncoming[] jokesText)
		//{
		//	GivenJokes(jokesText);
		//	WhenJokesAreCurated();
		//	ThenCuratedJokesArGroupedAsExpected(expectdShortJokeCount, expextedMediumJokeCount, expectedLongJokeCount);
		//}

		[Test]
		[TestCase(" ")]
		[TestCase("*")]
		[TestCase("why")]
		[TestCase("a")]
		[TestCase("one")]
		[TestCase("9")]
		public void ShouldGroupWithAnyReasonableTerm(string term)
		{
			GivenSearchTerm(term);
			GivenJokes(ShortJoke,MediumJoke, LongJoke);
			WhenJokesAreCurated();
			ThenCuratedJokesArGroupedAsExpected(1, 1, 1);
		}

		[Test]
		[TestCase("hello","hello","*","*","*hello*")]
		[TestCase("Hello", "hello", "*", "*", "*Hello*")]
		[TestCase("hello", "Hello", "*", "*", "*hello*")]
		[TestCase("hello", "hello", "<tag>", "</tag>", "<tag>hello</tag>")]
		[TestCase("Hello", "hello", "<tag>", "</tag>", "<tag>Hello</tag>")]
		[TestCase("hello", "Hello", "<tag>", "</tag>", "<tag>hello</tag>")]
		[TestCase("1 2 3 4", "2", "*", "*", "1 *2* 3 4")]
		[TestCase("Hello today is a good day to say hello!", "hello", "*", "*", "*Hello* today is a good day to say *hello*!")]
		[TestCase("Well hello there.","Hello","*","*", "Well *hello* there.")]
		[TestCase("What a wonderful day.", "a", "*", "*", "What *a* wonderful day.")]
		[TestCase("Everything is a-okay.", "a", "*", "*", "Everything is a-okay.")]
		public void ShouldEmphasizeSearchTerm(string givenJoke, string givenTerm, string beginEmph, string endEmph, string expectedEmphasizedJoke)
		{
			GivenSearchTerm(givenTerm);
			WhenEmphasizeTermsCalled(givenJoke, givenTerm, beginEmph, endEmph);
			ThenExpectedStringIsReturned(expectedEmphasizedJoke);

		}

		private void ThenExpectedStringIsReturned(string expectedEmphasizedJoke)
		{
			_thenEmphasizedTerm.Should().Be(expectedEmphasizedJoke);
		}

		private void WhenEmphasizeTermsCalled(string jokeText, string term, string beginEmphasis, string endEmphasis)
		{
			_thenEmphasizedTerm = _curatedHandler.EmphasizeTerm(jokeText, term, beginEmphasis, endEmphasis);
		}

		private void GivenSearchTerm(string term)
		{
			_givenTerm = term;
		}

		private void ThenCuratedJokesArGroupedAsExpected(int expectdShortJokeCount, int expextedMediumJokeCount,
			int expectedLongJokeCount)
		{
			_thenCuratedJokes.Short.Count.Should().Be(expectdShortJokeCount);
			_thenCuratedJokes.Medium.Count.Should().Be(expextedMediumJokeCount);
			_thenCuratedJokes.Long.Count.Should().Be(expectedLongJokeCount);
		}


		private void GivenJokes(params string[] jokes)
		{
			_givenJokes = new List<JokeIncoming>();
			foreach (var joke in jokes)
			{
				_givenJokes.Add( new JokeIncoming { Text = joke});
			}
		}

		private void GivenJokes(params JokeIncoming[] jokes)
		{
			_givenJokes.AddRange(jokes.ToList());
		}


		//private void GivenJokes(params JokeIncoming[] jokes)
		//{
		//	//_givenJokes = jokes;
		//}

		//private void GivenJokes(string jokesJson)
		//{
		//	_givenJokes = JsonSerializer.Deserialize<JokeIncoming[]>(jokesJson);
		//}

		private void GivenNullJokes()
		{
			_givenJokes = null;
		}
		private void WhenJokesAreCurated()
		{
			_thenCuratedJokes = _curatedHandler.CurateJokes(_givenJokes?.ToArray(), _givenTerm);
		}
		private void ThenCuratedJokesIsEmpty()
		{
			_thenCuratedJokes.Should().BeEquivalentTo(new CuratedJokes());
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
		//private static object[] _jokeGroupingTestCases2 =
		//{

		//	//new object[] {1, 0, 0, jokeilength1 },
		//	new object[] {1, 0, 0, new JokeIncoming {Text=JokeLength1}},
		//	new object[] {1, 0, 0, new JokeIncoming {Text=JokeLength9}},
		//	new object[] {0, 1, 0, new JokeIncoming {Text=JokeLength10}},
		//	new object[] {0, 1, 0, new JokeIncoming {Text=JokeLength11}},
		//	new object[] {0, 1, 0, new JokeIncoming {Text=JokeLength19}},
		//	new object[] {0, 0, 1, new JokeIncoming {Text=JokeLength20}},
		//	new object[] {0, 0, 1, new JokeIncoming {Text=JokeLength21}}
		//	//new object[] {2, 3, 2, new JokeIncoming[] { JokeLength1, JokeLength9, JokeLength10, JokeLength11, JokeLength19, JokeLength20, JokeLength21 } },
		//};						   

		//private static JokeIncoming jokeilength1 = new JokeIncoming{Text = "one"};
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
		private JokeIncoming ShortJoke = new JokeIncoming{Text = JokeLength9};
		private JokeIncoming MediumJoke = new JokeIncoming { Text = JokeLength11 };
		private JokeIncoming LongJoke = new JokeIncoming { Text =JokeLength21 };

		
		//const string NewLineCharJokes = new JokeIncoming { Text = "Why did Mozart kill all his chickens?\r\nBecause when he asked them who the best composer was, they'd all say \"Bach bach bach!\"\r\n" };


	}

}
