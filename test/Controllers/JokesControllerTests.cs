using System.Threading.Tasks;
using FluentAssertions;
using Joked.Controllers;
using Joked.Handlers;
using Joked.Model;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Joked.Test.Controllers
{
	class JokesControllerTests
	{
		private JokesController _jokesController;
		private Mock<IJokeHttpClient> _clientMock;
		private Mock<ILogger<JokesController>> _loggerMock;
		private Mock<JokesHandler> _jokesHandlerMock;

		[Test]
		public void ShouldNotAllowMoreThanTheLimit()
		{
			var limit = 30;
			var jokesResult = _jokesController.GetJokes("a", true, limit, false);
			var value = (ICuratedJokes)jokesResult.Value;

			TotalCuratedJokes(value).Should().Be(limit); 
		}

		private static int TotalCuratedJokes(ICuratedJokes curatedJokes)
		{
			return curatedJokes.Long.Count + curatedJokes.Medium.Count + curatedJokes.Short.Count;
		}

		[SetUp]
		public void Setup()
		{
			var jokes = new JokesDtoBuilder(45).Build();
			string joskesJson = System.Text.Json.JsonSerializer.Serialize<JokesDto>(jokes);
			
			_clientMock = new Mock<IJokeHttpClient>();
			_clientMock.Setup(x => x.Get(It.IsAny<string>())).Returns(Task.FromResult<string>(joskesJson));

			_loggerMock = new Mock<ILogger<JokesController>>();
			_jokesHandlerMock = new Mock<JokesHandler>(_loggerMock.Object, _clientMock.Object, null);
			_jokesHandlerMock.Setup(x => x.GetJokes(It.IsAny<string>(),It.IsAny<int>())).Returns(jokes);
			//_jokesHandlerMock.CallBase = true;
			//_jokesHandlerMock.Setup(x => x.CurateJokes(It.IsAny<JokeDto[]>(), It.IsAny<string>(), It.IsAny<bool>())).CallBase();
			//var _jokesHandler = new JokesHandler(_loggerMock.Object, _clientMock.Object);

			_jokesController = new JokesController(_clientMock.Object, _loggerMock.Object, _jokesHandlerMock.Object);//_jokesHandlerMock.Object);
		}

	}
	
}

