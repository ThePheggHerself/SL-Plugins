using MEC;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Core.Factories;
using PluginAPI.Events;
using System;
using System.Net;

namespace DiscordLab
{
	public class DiscordLab
	{
		[PluginConfig]
		public static DiscordLabConfig Config;
		public static BotConnector Bot;

		[PluginEntryPoint("DiscordLab", "1.0.0", "Bridge between SCP:SL servers, and Discord", "ThePheggHerself"), PluginPriority(PluginAPI.Enums.LoadPriority.Highest)]
		public void OnPluginStart()
		{
			Log.Info($"Plugin is loading...");

			Bot = new BotConnector();

			EventManager.RegisterEvents<Events>(this);


			if (string.IsNullOrEmpty(Config.Address))
				Config.Address = Server.ServerIpAddress;

			Log.Info($"Plugin has fully loaded");

			WaitingForPlayersEvent.Event += WaitingForPlayersEvent_Event;
		}

		public void WaitingForPlayersEvent_Event(WaitingForPlayersEvent obj)
		{
			DiscordLab.Bot.SendMessage(new Msg("**Waiting for players...**"));
		}
	}
}
