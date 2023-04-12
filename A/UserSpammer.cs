using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A;

/// <summary>
/// A user the bot is currently stalking
/// </summary>
public struct SpammedUser
{
	public DateTime StartTime;
	public SocketUser User;
}


//TODO: make this class work
public class UserSpammer
{

	private List<SpammedUser> _spammingUsers = new List<SpammedUser>();

	public static TimeSpan SpamLength => TimeSpan.FromDays(2);
	public static TimeSpan SpamInterval => TimeSpan.FromMinutes(5);

	public UserSpammer()
	{
		
	}

	public void AddToSpamList(SocketUser user)
	{
		_spammingUsers.Add(new SpammedUser() { StartTime = DateTime.UtcNow, User = user });
	}




	public void Update()
	{
		var count = _spammingUsers.Count;
		for (int i = 0; i < count; i++)
		{
			var user = _spammingUsers[i];

			if((user.StartTime+ SpamLength) < DateTime.UtcNow)
			{
				_spammingUsers.RemoveAt(i);
				i--;
			}

		}

	}

}
