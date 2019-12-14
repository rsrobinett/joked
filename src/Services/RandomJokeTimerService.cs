using Joked.Model;
using Microsoft.Extensions.Hosting;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.SignalR;
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
		private readonly IHubContext<JokeHub> _hubContext;

		public RandomJokeTimerService(IJokeHttpClient client, ILogger<RandomJokeTimerService> logger, IHubContext<JokeHub> hub)
		{
			_httpClient = client;
			_logger = logger;
			_hubContext = hub;
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			_timer = new Timer(o => RandomJokeTask(o, cancellationToken), null, TimeSpan.Zero, TimeSpan.FromSeconds(TimerFrequencySeconds));

			return Task.CompletedTask;
		}

		public void RandomJokeTask(object state, CancellationToken cancellationToken = default)
		{
			if (cancellationToken.IsCancellationRequested) return;

			var result = GetRandomJoke().Result.Replace(@"\r\n", " ");

			var joke = JsonSerializer.Deserialize<JokeIncoming>(result) as IJoke;

			DisplayRandomJoke(HttpUtility.UrlDecode(joke?.Text ?? FallbackJoke)).Wait(cancellationToken);
		}

		public async Task<string> GetRandomJoke()
		{
			return await _httpClient.Get("/");
		}

		private async Task DisplayRandomJoke(string joke)
		{
			_logger.Log(LogLevel.Critical, "sending:" + joke);
			
			await _hubContext.Clients.All.SendAsync("randomJoke", joke);
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
