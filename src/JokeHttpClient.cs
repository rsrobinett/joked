using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Joked
{
	public class JokeHttpClient : HttpClient, IJokeHttpClient
	{
		private readonly HttpClient _client;

		public JokeHttpClient(HttpClient httpClient)
		{
			httpClient.BaseAddress = new Uri("https://icanhazdadjoke.com/");
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			httpClient.DefaultRequestHeaders.Add("User-Agent", "github.com/rsrobinett");
			_client = httpClient;
		}
		public async Task<string> Get(string uriPath = "/")
		{
			return await _client.GetStringAsync(uriPath);
		}
	}

	public interface IJokeHttpClient : IDisposable
	{
		Task<string> Get(string uriPath = "/");
	}
}
