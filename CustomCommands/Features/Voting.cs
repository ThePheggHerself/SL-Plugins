using CommandSystem;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using RemoteAdmin;
using System;

namespace CustomCommands.Features
{
	[CommandHandler(typeof(ClientCommandHandler))]
	public class CmdYesVote : ICommand
	{
		public string Command => "yes";

		public string[] Aliases => null;
		public string Description => "Vote yes on the current vote";

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (sender is PlayerCommandSender pSender)
			{
				if (!Plugin.VoteInProgress)
				{
					response = "There is no vote in progress";
					return false;
				}

				var plr = Player.Get(pSender.ReferenceHub);

				if (plr.TemporaryData.Contains("vote_yes") || plr.TemporaryData.Contains("vote_no"))
				{
					response = "You have already voted";
					return false;
				}

				plr.TemporaryData.Override("vote_yes", string.Empty);

				response = "You have voted yes";
				return false;
			}

			response = "You must be a player to run this command";
			return false;
		}
	}

	[CommandHandler(typeof(ClientCommandHandler))]
	public class CmdNoVote : ICommand
	{
		public string Command => "no";
		public string[] Aliases => null;
		public string Description => "Vote no on the current vote";

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (sender is PlayerCommandSender pSender)
			{
				if (!Plugin.VoteInProgress)
				{
					response = "There is no vote in progress";
					return false;
				}

				var plr = Player.Get(pSender.ReferenceHub);

				if (plr.TemporaryData.Contains("vote_yes") || plr.TemporaryData.Contains("vote_no"))
				{
					response = "You have already voted";
					return false;
				}

				plr.TemporaryData.Override("vote_no", string.Empty);

				response = "You have voted no";
				return false;
			}

			response = "You must be a player to run this command";
			return false;
		}
	}

	public class Voting
	{
		[PluginEvent(ServerEventType.PlayerJoined), PluginPriority(LoadPriority.Lowest)]
		public void PlayerJoin(PlayerJoinedEvent args)
		{
			if (Plugin.VoteInProgress)
			{
				args.Player.SendBroadcast($"<b><color=#fa886b>[VOTE]</color></b> <color=#79b7ed>{Plugin.CurrentVoteString}</color>\nUse your console to vote now!", 15);
				args.Player.SendConsoleMessage("A vote has been started. Run the `.yes` command to vote yes, or `.no` command to vote no");
			}
		}

		[PluginEvent(ServerEventType.RoundRestart)]
		public void RoundRestart(RoundRestartEvent args)
		{
			SetVote(VoteType.NONE, string.Empty);
		}

		[PluginEvent(ServerEventType.RoundEnd)]
		public void RoundEnd(RoundEndEvent args)
		{
			EndVote();
		}

		public static void SetVote(VoteType type, string vStr)
		{
			Plugin.CurrentVote = type;
			Plugin.CurrentVoteString = vStr;
		}

		public static void EndVote()
		{
			if (!Plugin.VoteInProgress)
				return;

			int yes = 0;
			int no = 0;
			int nil = 0;

			foreach (var a in Player.GetPlayers())
			{
				if (a.TemporaryData.Contains("vote_yes"))
				{
					yes++;
					a.TemporaryData.Remove("vote_yes");
				}
				else if (a.TemporaryData.Contains("vote_no"))
				{
					no++;
					a.TemporaryData.Remove("vote_no");
				}
				else
					nil++;
			}

			Server.SendBroadcast($"The vote is over!\n<color=green>{yes} voted yes</color>, <color=red>{no} voted no</color>, and {nil} did not vote", 10);
			SetVote(VoteType.NONE, string.Empty);
		}
	}
}
