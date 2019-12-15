using System.Text.Json.Serialization;

namespace Joked.Model
{
	internal interface IJokes
	{
		[JsonPropertyName("results")]
		JokeDto[] Jokes { get; set; }
	}
	internal class JokesDto : IJokes
	{
		[JsonPropertyName("results")]
		public JokeDto[] Jokes { get; set; }
	}
}
