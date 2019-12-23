using System.Collections.Generic;

namespace Joked.Model
{

	public class CuratedJokes : ICuratedJokes
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

	/// <summary>
	/// List of jokes curated in to 3 groups short medium and long
	/// </summary>
	public interface ICuratedJokes
	{
		/// <summary>
		/// Short jokes <![CDATA[<10 words]]>
		/// </summary>
		List<string> Short { get; set; }
		/// <summary>
		/// Medium Joke <![CDATA[<20 words ]]>
		/// </summary>
		List<string> Medium { get; set; }
		/// <summary>
		/// Long jokes >= 20 words
		/// </summary>
		List<string> Long { get; set; }
	}

	public interface ICurateLongJokes
	{
		List<string> ExtraLong { get; set; }
	}
}
