using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCommands.Features.Dummy
{
	public class DummyEvents
	{
		[PluginEvent]
		public void WaitingForPlayers(WaitingForPlayersEvent args)
		{
			DummyManager.DummyID = 0;

			if (Plugin.Config.EnableSteve)
			{
				DummyManager.CreateDummy("Steve");
				Round.IsLocked = true;
			}
		}
	}
}
