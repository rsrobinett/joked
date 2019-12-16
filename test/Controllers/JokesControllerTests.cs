using System.Net.Http;
using FluentAssertions;
using Joked.Controllers;
using Joked.Handlers;
using Joked.Model;
using Microsoft.AspNetCore.Mvc;
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
		private Mock<IJokesHandler> _jokesHandlerMock;

		[Test, Ignore("Integration Test.  I'd love suggestions for testing a controller with asyncronous code'")]
		public void ShouldAllowMoreThanTheLimit()
		{
			var limit = 30;
			var loggerFactory = new LoggerFactory();
			var logger = new Logger<JokesController>(loggerFactory);
			var httpClient = new HttpClient();
			var jokeHttpClient = new JokeHttpClient(httpClient);
			var jokeHandler = new JokesHandler(logger, jokeHttpClient, new SimpleEmphasis());

			var jokesController = new JokesController(jokeHttpClient,logger,jokeHandler);

			var x = jokesController.GetJokes("a",true,limit,false);

			TotalCuratedJokes(x).Should().Be(limit); 
		}

		private static int TotalCuratedJokes(ActionResult<ICuratedJokes> x)
		{
			return x.Value.Long.Count + x.Value.Medium.Count + x.Value.Short.Count;
		}

		[SetUp]
		public void Setup()
		{
			_clientMock = new Mock<IJokeHttpClient>();
			_loggerMock = new Mock<ILogger<JokesController>>();
			_jokesHandlerMock = new Mock<IJokesHandler>();
			
			_loggerMock.Setup(x => x.Log(It.IsAny<LogLevel>(), It.IsAny<string>()));
			_jokesController = new JokesController(_clientMock.Object, _loggerMock.Object, _jokesHandlerMock.Object);
		}

	}
	
}

