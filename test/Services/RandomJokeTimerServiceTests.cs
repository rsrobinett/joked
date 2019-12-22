using Joked.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;
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
		
		[Test, Parallelizable, Explicit("These test too long and can be brittle based on how long  the service takes to spin up; however, they are useful for local development.")]
		[TestCase(5, 0, 1)]
		[TestCase(15, 1, 2)]
		[TestCase(25, 2, 3)]
		[TestCase(35, 3, 4)]
		public async Task ShouldDisplayJokeOnARotation(int secondsToRun, int minExpectedCount,int maxExpectedCount)
		{
			GivenTheServiceRunsForSeconds(secondsToRun);
			ThenJokesDisplayedIsBetween(minExpectedCount, maxExpectedCount);
		}

		private void ThenJokesDisplayedIsBetween(int minExpectedCount, int maxExpectedCount)
		{
			_randomJokeServiceMock.Verify(x => x.GetRandomJoke(),
				Times.Between(minExpectedCount, maxExpectedCount, Range.Inclusive));
			_randomJokeServiceMock.Verify(x => x.DisplayRandomJoke(It.IsAny<string>(), It.IsAny<CancellationToken>()),
				Times.Between(minExpectedCount, maxExpectedCount, Range.Inclusive));
		}

		private void GivenTheServiceRunsForSeconds(int secondsToRun)
		{

			_ctxS.CancelAfter(secondsToRun * 1000);
			_randomJokeServiceMock.Object.StartAsync(_ctxS.Token); 
			
			_ctxS.Token.WaitHandle.WaitOne();

		}

		[SetUp]
		public void Setup()
		{
			_ctxS = new CancellationTokenSource();
			_ctxS.CancelAfter(25000);
			
			_clientMock = new Mock<IJokeHttpClient>();
			_clientMock.Setup(x => x.Get("/")).Returns(Task.FromResult<string>(""));

			_loggerMock = new Mock<ILogger<RandomJokeTimerService>>();
			
			_hubMock = new Mock<IHubContext<JokeHub>>();
			
			_randomJokeServiceMock = new Mock<RandomJokeTimerService>(_clientMock.Object,_loggerMock.Object,_hubMock.Object);
		}

		[TearDown]
		public void Teardown()
		{
			_randomJokeServiceMock.Object.Dispose();
		}

	}
}
