using Joked.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Joked.Test.Services
{

	class RandomJokeTimerServiceTests
	{
		private RandomJokeTimerService _randomJokeService;
		private Mock<IJokeHttpClient> _clientMock;
		private Mock<ILogger<RandomJokeTimerService>> _loggerMock;
		private Mock<IHubContext<JokeHub>> _hubMock;

		[Test]
		public void GetMessage()
		{
			_clientMock = new Mock<IJokeHttpClient>();
			_loggerMock = new Mock<ILogger<RandomJokeTimerService>>();
			_hubMock = new Mock<IHubContext<JokeHub>>();
			_randomJokeService = new RandomJokeTimerService(_clientMock.Object,_loggerMock.Object, _hubMock.Object);
		}


	}
}
