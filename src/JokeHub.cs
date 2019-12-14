using System.Threading.Tasks;
using Joked.Model;
using Microsoft.AspNetCore.SignalR;
using Microsoft.VisualBasic;

namespace Joked
{
	public class JokeHub : Hub
	{
		//public async Task BroadcastJokeData(Joke data) => await Clients.All.SendAsync("transferjokedata", data);
		public async Task SendMessage(Joke joke)
		{
			await Clients.All.SendAsync("Send", joke);
		}
	}
}
