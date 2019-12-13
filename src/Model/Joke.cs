using System;
using System.Collections.Generic;
using System.Text.Json;
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
		//public JokeMetaData MetaData { get; set; }
	}

	//public class JokeMetaData
	//{
	//	public JokeCategory Category { get; set; }
	//}

	//public class JokeCategory
	//{
	//}


	//probably not going to use this
	public class JokeConverter : JsonConverter<IJoke[]>
	{
		public override IJoke[] Read(ref Utf8JsonReader reader, Type typeToConvert,
			JsonSerializerOptions options)
		{
			return JsonSerializer.Deserialize<IJoke[]> (reader.GetString());
		}

		public override void Write(Utf8JsonWriter writer, IJoke[] value, JsonSerializerOptions options)
		{
			writer.WriteStringValue(value.ToString());
		}

		public override bool CanConvert(Type objectType)
		{
			return (objectType == typeof(List<JokeIncoming[]>));
		}
	}
}
