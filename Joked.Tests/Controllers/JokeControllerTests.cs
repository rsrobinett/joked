using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Joked.Tests.Controllers.JokeControllerTests
{
	[TestFixture]
    internal class JokeControllerTests
    {
		//these are actually integration tests
	    [Test]
		[TestCase(3)]
	    public void ShouldGetJokes(int count)
	    {
		    WhenJokesAreCurated();
		    ThenReturnedJokes();
	    }

	    [SetUp]
	    public void Setup()
	    {

	    }
		
	    private void GivenJokes()
	    {
		    throw new NotImplementedException();
	    }

		private void WhenJokesAreCurated()
	    {
		    throw new NotImplementedException();
	    }
		
	    private void ThenReturnedJokes()
	    {
		    
	    }
    }
}
