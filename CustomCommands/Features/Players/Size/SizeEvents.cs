using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PluginAPI.Core.Attributes;
using PluginAPI.Core;


namespace CustomCommands.Features.Players.Size
{
	public class SizeEvents
	{
		[PluginEvent]
		public void OnPlayerDie(PlayerDeathEvent ev)
		{
			ev.Player.ResetSize();
		}

		[PluginEvent]
		public void OnRoundEnd(RoundEndEvent ev)
		{
			foreach (var plr in Server.GetPlayers())
				plr.ResetSize();
		}
	}
}
