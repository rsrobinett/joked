using Joked.Model;
using Microsoft.Extensions.Hosting;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Logging;

namespace Joked.Services
{
	public class RandomJokeTimerService : IHostedService, IDisposable
	{
		private Timer _timer;
		private const int TimerFrequencySeconds = 10;
		private const string FallbackJoke = "Why did the chicken cross the street? To get to the other side.";

		private readonly IJokeHttpClient _httpClient;
		private readonly ILogger<RandomJokeTimerService> _logger;

		public RandomJokeTimerService(IJokeHttpClient client, ILogger<RandomJokeTimerService> logger)
		{
			_httpClient = client;
			_logger = logger;
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			_timer = new Timer(o => RandomJokeTask(o, cancellationToken), null, TimeSpan.Zero, TimeSpan.FromSeconds(TimerFrequencySeconds));

			return Task.CompletedTask;
		}

		private void RandomJokeTask(object state, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested) return;

			var result = GetRandomJoke().Result.Replace(@"\r\n", " ");

			var joke = JsonSerializer.Deserialize<JokeIncoming>(result) as IJoke;

			DisplayRandomJoke(HttpUtility.UrlDecode(joke?.Text ?? FallbackJoke));
		}

		private async Task<string> GetRandomJoke()
		{
			return await _httpClient.Get("/");
		}

		private void DisplayRandomJoke(string joke)
		{
			_logger.Log(LogLevel.Critical, joke);
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
