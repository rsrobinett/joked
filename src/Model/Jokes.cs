using System.Text.Json.Serialization;

namespace Joked.Model
{
	/// <summary>
	/// Object received from the joke server 
	/// </summary>
	internal interface IJokes
	{
		/// <summary>
		/// List of Jokes
		/// </summary>
		[JsonPropertyName("results")]
		JokeDto[] Jokes { get; set; }
	}
	
	internal class JokesDto : IJokes
	{
		[JsonPropertyName("results")]
		public JokeDto[] Jokes { get; set; }
	}
}
