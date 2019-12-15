using System.Text.Json.Serialization;

namespace Joked.Model
{

	public interface IJoke
	{
		[JsonPropertyName("text")]
		string Text { get; set; }
	}
	
	public class JokeIncoming : Joke, IJoke
	{
		[JsonPropertyName("joke")]
		public new string Text { get; set; }
	}

	public class Joke : IJoke
	{
		public string Text { get; set; }
	}
}
