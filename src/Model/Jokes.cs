using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Joked.Model
{
	internal interface IJokes
	{
		[JsonPropertyName("jokes")]
		IJoke[] Jokes { get; set; }
	}
	internal class JokesIncoming 
	{
		[JsonPropertyName("results")]
		public JokeIncoming[] Jokes { get; set; }
	}

	/// <summary>
	/// List of jokes curated in to 3 groups short medium and long
	/// </summary>
	public class CuratedJokes
	{
		/// <summary>
		/// Short jokes <![CDATA[<10 words]]>
		/// </summary>
		public List<string> Short { get; set; }
		/// <summary>
		/// Medium Joke <![CDATA[<20 words ]]>
		/// </summary>
		public List<string> Medium { get; set; }
		/// <summary>
		/// Long jokes >= 20 words
		/// </summary>
		public List<string> Long { get; set; }

		public CuratedJokes()
		{
			Short = new List<string>();
			Medium = new List<string>();
			Long = new List<string>();
		}
	}
}
