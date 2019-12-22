using System;
using System.Threading.Tasks;
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
		private Mock<JokesHandler> _jokesHandlerMock;
		private ICuratedJokes _thenCuratedJokes;

		[Test]
		[TestCase(30, 45)]
		[TestCase(25, 45)]
		[TestCase(30, 10)]
		[TestCase(5, 0)]
		public void ShoulAllowUptoButNotMoreThanTheLimit(int limit, int jokesCountFromServer)
		{
			GivenJokesFromTheServer(jokesCountFromServer);
			WhenTheJokesAreCuratedWithALimit(limit);
			ThenTheTotalCuratedJokes().Should().Be(Math.Min(limit, jokesCountFromServer)); 
		}
		
		[SetUp]
		public void Setup()
		{
			
			_clientMock = new Mock<IJokeHttpClient>();

			_loggerMock = new Mock<ILogger<JokesController>>();
			_jokesHandlerMock = new Mock<JokesHandler>(_loggerMock.Object, _clientMock.Object, null);
		
			_jokesController = new JokesController(_clientMock.Object, _loggerMock.Object, _jokesHandlerMock.Object);
		}

		private void GivenJokesFromTheServer(int jokesCountFromServer)
		{
			var jokes = new JokesDtoBuilder(jokesCountFromServer).Build();
			_jokesHandlerMock.Setup(x => x.GetJokes(It.IsAny<string>(), It.IsAny<int>())).Returns(jokes);
		}
		private void WhenTheJokesAreCuratedWithALimit(int limit)
		{
			var jokesResult = _jokesController.GetJokes("anything", true, limit, false).Result as OkObjectResult;
			_thenCuratedJokes = (ICuratedJokes)jokesResult?.Value;
		}
		private int ThenTheTotalCuratedJokes()
		{
			return _thenCuratedJokes.Long.Count + _thenCuratedJokes.Medium.Count + _thenCuratedJokes.Short.Count;
		}
	}

}

