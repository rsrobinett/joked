using System.Text.Json.Serialization;

namespace Joked.Model
{
	internal interface IJoke
	{
		[JsonPropertyName("joke")]
		string Joke { get; set; }
	}

	internal  class JokeDto : IJoke
	{
		[JsonPropertyName("joke")]
		public string Joke { get; set; }
	}
}

