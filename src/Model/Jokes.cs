using System.Collections.Generic;
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
		public List<string> Short { get; set; }
		public List<string> Medium { get; set; }
		public List<string> Long { get; set; }

		public CuratedJokes()
		{
			Short = new List<string>();
			Medium = new List<string>();
			Long = new List<string>();
		}
	}
}
