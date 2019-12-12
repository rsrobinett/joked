using System.Collections.Generic;
using System.Data.Common;
using System.Text.Json.Serialization;

namespace Joked.Model
{
	internal interface IJokes
	{
		[JsonPropertyName("jokes")]
		IJoke[] Jokes { get; set; }
	}
	public class JokesIncoming 
	{
		[JsonPropertyName("results")]
		//[JsonConverter(typeof(JokeConverter))] //failed attempt
		public JokeIncoming[] Jokes { get; set; }
	}

	public class JokesWrapper : IJokes
	{
		public IJoke[] Jokes { get; set; }
	}
	
	public class CuratedJokes
	{
		public List<string> ShortJokes { get; set; }
		public List<string> MediumJokes { get; set; }
		public List<string> LongJokes { get; set; }
	}
}
