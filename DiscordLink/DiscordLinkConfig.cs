using RedRightHandCore.PluginCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordLink
{
	public class DiscordLinkConfig : CustomConfig
	{
		public int BotPort { get; set; } = 7777;
		public string BotAddress { get; set; } = "127.0.0.1";
	}
}
