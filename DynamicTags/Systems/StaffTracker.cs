using LiteNetLib.Utils;
using LiteNetLib;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using PluginAPI.Core.Attributes;
using PluginAPI.Core;
using PluginAPI.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using RemoteAdmin;
using CommandSystem;
using PluginAPI.Events;

namespace DynamicTags.Systems
{
	public class StaffTracker
	{
		[PluginEvent(ServerEventType.PlayerJoined)]
		public void OnPlayerJoin(PlayerJoinedEvent args)
		{
			var details = new PlayerDetails
			{
				UserId = args.Player.UserId,
				UserName = args.Player.Nickname,
				Address = args.Player.IpAddress,
				ServerAddress = Server.ServerIpAddress,
				ServerPort = Server.Port.ToString()
			};

			Extensions.Post(Plugin.Config.ApiEndpoint + "scpsl/playerjoin", new StringContent(JsonConvert.SerializeObject(details), Encoding.UTF8, "application/json"));
		}

		[PluginEvent(ServerEventType.PlayerLeft)]
		public void OnPlayerLeave(PlayerLeftEvent args)
        {
            var details = new PlayerDetails
            {
                UserId = args.Player.UserId,
                UserName = args.Player.Nickname,
                Address = args.Player.IpAddress,
                ServerAddress = Server.ServerIpAddress,
                ServerPort = Server.Port.ToString()
            };

            Extensions.Post(Plugin.Config.ApiEndpoint + "scpsl/playerleave", new StringContent(JsonConvert.SerializeObject(details), Encoding.UTF8, "application/json"));
		}


		[PluginEvent(ServerEventType.PlayerBanned)]
		public void OnPlayerBanned(PlayerBannedEvent args)
		{
			var details = new PlayerBanDetails
			{
				PlayerName = args.Player.Nickname.Replace(':', ' '),
				PlayerID = args.Player.UserId,
				PlayerAddress = args.Player.IpAddress,
				AdminName = args.Issuer.Nickname.Replace(':', ' '),
				AdminID = args.Issuer.UserId,
				Duration = (args.Duration / 60).ToString(),
				Reason = string.IsNullOrEmpty(args.Reason) ? "No reason provided" : args.Reason
			};

			Extensions.Post(Plugin.Config.ApiEndpoint + "scpsl/playerban", new StringContent(JsonConvert.SerializeObject(details), Encoding.UTF8, "application/json"));
		}

		[PluginEvent(ServerEventType.PlayerKicked)]
		public void OnPlayerKicked(PlayerKickedEvent args)
		{
			PlayerBanDetails details;

			if (args.Issuer is PlayerCommandSender pCS)
			{
				var admin = Player.Get(pCS.PlayerId);

				details = new PlayerBanDetails
				{
					PlayerName = args.Player.Nickname.Replace(':', ' '),
					PlayerID = args.Player.UserId,
                    PlayerAddress = args.Player.IpAddress,
                    AdminName = admin.Nickname.Replace(':', ' '),
					AdminID = admin.UserId,
					Duration = "0",
					Reason = string.IsNullOrEmpty(args.Reason) ? "No reason provided" : args.Reason
				};				
			}
			else
			{
				details = new PlayerBanDetails
				{
                    PlayerName = args.Player.Nickname.Replace(':', ' '),
                    PlayerID = args.Player.UserId,
                    PlayerAddress = args.Player.IpAddress,
                    AdminName = "SERVER",
					AdminID = "server",
					Duration = "0",
                    Reason = string.IsNullOrEmpty(args.Reason) ? "No reason provided" : args.Reason
                };

			}
			Extensions.Post(Plugin.Config.ApiEndpoint + "scpsl/playerkick", new StringContent(JsonConvert.SerializeObject(details), Encoding.UTF8, "application/json"));
		}

		[PluginEvent(ServerEventType.PlayerMuted)]
		public void PlayerMuteEvent(PlayerMutedEvent args)
		{
			var details = new PlayerBanDetails
			{
				PlayerName = args.Player.Nickname.Replace(':', ' '),
				PlayerID = args.Player.UserId,
                PlayerAddress = args.Player.IpAddress,
                AdminName = args.Issuer.Nickname.Replace(':', ' '),
				AdminID = args.Issuer.UserId,
				Duration = args.IsIntercom.ToString(),
				Reason = "No reason provided"
			};

			Extensions.Post(Plugin.Config.ApiEndpoint + "scpsl/playermute", new StringContent(JsonConvert.SerializeObject(details), Encoding.UTF8, "application/json"));
		}

		[PluginEvent(ServerEventType.PlayerUnmuted)]
		public void PlayerUnmuteEvent(PlayerUnmutedEvent args)
        {
            var details = new PlayerBanDetails
            {
                PlayerName = args.Player.Nickname.Replace(':', ' '),
                PlayerID = args.Player.UserId,
                PlayerAddress = args.Player.IpAddress,
                AdminName = args.Issuer.Nickname.Replace(':', ' '),
                AdminID = args.Issuer.UserId,
                Duration = args.IsIntercom.ToString(),
                Reason = "No reason provided"
            };

            Extensions.Post(Plugin.Config.ApiEndpoint + "scpsl/playerunmute", new StringContent(JsonConvert.SerializeObject(details), Encoding.UTF8, "application/json"));
		}
	}
}
