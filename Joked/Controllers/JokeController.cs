using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Joked.Controllers
{
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
	[ApiController]
    public class JokeController : ControllerBase
    {

	    internal int JokeMax = 30;

		/// <summary>
		/// Accept a search term and display the first 30 jokes containing that term,
		/// with the matching term emphasized in some way (upper, bold, color, etc.)
		/// and the matching jokes grouped by length: short (<10 words), medium (<20 words), long (>= 20 words)
		/// https://icanhazdadjoke.com/api
		/// </summary>
		[HttpGet("")]
	    [ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public IActionResult GetCuratedJokes(string searchTerm)
		{
			
			//todo: Header User-Agent:  https://github.com/rsrobinett
			//todo: build uri: https://icanhazdadjoke.com/search?limit=30&term= some urlencoded value, 
			//todo: call async and await. 
			//todo: curate jokes - 1) add length to the object 2) Group by length 3) order groups
			

			return Ok("return joke object here");
		}

		/// <summary>
		/// Triggers a job that will
		/// Display a random joke every 10 seconds.
		/// </summary>
		/// <returns></returns>
		[HttpPost("trigger")]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public IActionResult KickOffJokeJob()
		{
			
			//todo: quartz.net job and trigger: Is this overkill? Used Timed Background Task.
			//todo: Header User-Agent:  https://github.com/rsrobinett
			//todo: call https://icanhazdadjoke.com/
			//todo: update the ui component
			//todo: stop or repeat
			
			return Ok("joke task triggered");
		}
	}
}