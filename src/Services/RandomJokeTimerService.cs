using Joked.Model;
using Microsoft.Extensions.Hosting;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;


namespace Joked.Services
{
	public class RandomJokeTimerService : IHostedService, IDisposable
	{
		private Timer _timer;
		private const int TimerFrequencySeconds = 1000;
		private const string FallbackJoke = "Why did the chicken cross the street? To get to the other side.";

		private readonly IJokeHttpClient _httpClient;
		public RandomJokeTimerService(IJokeHttpClient client)
		{
			_httpClient = client;
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			_timer = new Timer(o => RandomJokeTask(o, cancellationToken), null, TimeSpan.Zero, TimeSpan.FromSeconds(TimerFrequencySeconds));

			return Task.CompletedTask;
		}

		private void RandomJokeTask(object state, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested) return;

			var result = GetRandomJoke().Result;

			var joke = JsonSerializer.Deserialize<JokeIncoming>(result) as IJoke;

			DisplayRandomJoke(joke?.Text);
		}

		private async Task<string> GetRandomJoke()
		{
			return await _httpClient.Get("/");
		}

		private void DisplayRandomJoke(string text)
		{
			var joke = HttpUtility.UrlDecode(text ?? FallbackJoke);
			Console.WriteLine(joke);
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}

		public void Dispose()
		{
			_timer?.Dispose();
		}
	}
}
