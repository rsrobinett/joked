using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Joked.Model;

namespace Joked.Test
{
	internal class JokesDtoBuilder : IJokesBuilder
	{
		private readonly int _totalJokes = 5;
		private readonly int _shortJokeCount;
		private readonly int _mediumJokeCount;
		private readonly int _longJokeCount;
		private readonly bool _thisBuilderCaresAboutJokeLength = false;
		private readonly string word = "word";

		private const int MediumJokeMinLength = 10;
		private const int LongJokeMinLength = 20;
		private JokesDtoBuilder()
		{
			
		}

		public JokesDtoBuilder(int totalJokesCount)
		{
			_totalJokes = totalJokesCount;
		}

		public JokesDtoBuilder(int shortJokeCount, int mediumJokeCount, int longJokeCount)
		{
			_shortJokeCount = shortJokeCount;
			_mediumJokeCount = mediumJokeCount;
			_longJokeCount = longJokeCount;
			_thisBuilderCaresAboutJokeLength = true;
		}

		private string JokeString(int minLength = 5, int maxLength = 30)
		{
			var length = new Random(DateTime.Now.Second).Next(minLength,maxLength);
			var jokeString = word;

			for (var i = 1; i < length; i++)
			{
				jokeString += " " + word;
			}
			
			return jokeString;
		}
		
		public JokesDto Build()
		{
			return !_thisBuilderCaresAboutJokeLength ? CreateJokes() : CreateJokesWithSpecifiedLengthCategories();
		}

		private JokesDto CreateJokesWithSpecifiedLengthCategories()
		{
			var jokeDtoList = new List<JokeDto>();

			for (var i = 0; i < _shortJokeCount; i++)
			{
				jokeDtoList.Add(new JokeDto() {Joke = JokeString(0, MediumJokeMinLength - 1)});
			}

			for (var i = 0; i < _mediumJokeCount; i++)
			{
				jokeDtoList.Add(new JokeDto() {Joke = JokeString(MediumJokeMinLength, LongJokeMinLength - 1)});
			}

			for (var i = 0; i < _longJokeCount; i++)
			{
				jokeDtoList.Add(new JokeDto() {Joke = JokeString(LongJokeMinLength, LongJokeMinLength + 20)});
			}

			return new JokesDto
			{
				Jokes = jokeDtoList.ToArray()
			};
		}

		private JokesDto CreateJokes()
		{
			var jokeDtoList = new List<JokeDto>();

			for (var i = 0; i < _totalJokes; i++)
			{
				jokeDtoList.Add(new JokeDto() {Joke = JokeString()});
			}

			return new JokesDto
			{
				Jokes = jokeDtoList.ToArray()
			};
		}
	}

	internal interface IJokesBuilder
	{
		JokesDto Build();
	}
}
