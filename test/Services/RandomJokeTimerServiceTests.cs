using System;
using System.Threading;
using System.Threading.Tasks;
using Joked.Model;
using Joked.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Range = Moq.Range;

namespace Joked.Test.Services
{

	class RandomJokeTimerServiceTests
	{
		private Mock<RandomJokeTimerService> _randomJokeServiceMock;
		private Mock<IJokeHttpClient> _clientMock;
		private Mock<ILogger<RandomJokeTimerService>> _loggerMock;
		private Mock<IHubContext<JokeHub>> _hubMock;
		private CancellationTokenSource _ctxS;
		
		[Test, Ignore("Tasks are tough to test.  I'd love suggestions on how you would test something like this.  I know it works from manual testing.")]
		public async Task ShouldDisplayJoke2Or3TimesIn25Seconds()
		{
			_ctxS = new CancellationTokenSource();
			//_ctxS.CancelAfter(25000);
			
			var task = _randomJokeServiceMock.Object.StartAsync(_ctxS.Token);

			task.Wait(new TimeSpan(0, 0, 0, seconds: 25));
			
			_randomJokeServiceMock.Verify(x=>x.GetRandomJoke(),Times.Between(2,3,Range.Inclusive));
			_randomJokeServiceMock.Verify(x => x.DisplayRandomJoke(It.IsAny<string>()), Times.Between(2, 3, Range.Inclusive));
		}

		[SetUp]
		public void setup()
		{
			_clientMock = new Mock<IJokeHttpClient>();
			_loggerMock = new Mock<ILogger<RandomJokeTimerService>>();
			_hubMock = new Mock<IHubContext<JokeHub>>();

			_randomJokeServiceMock = new Mock<RandomJokeTimerService>(_clientMock.Object,_loggerMock.Object,_hubMock.Object);
			_randomJokeServiceMock.Setup(x => x.GetRandomJoke()).Returns(new JokeDto() { Joke = "Random Joke" });
			_randomJokeServiceMock.Setup(x => x.DisplayRandomJoke(It.IsAny<string>()));

		}

	}
}
