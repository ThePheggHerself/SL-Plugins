using LabApi.Events.Arguments.WarheadEvents;
using LabApi.Events.Arguments;
using LabApi.Events.CustomHandlers;
using LabApi.Features.Wrappers;
using LabApi.Features.Interfaces;
using RedRightHandCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.Arguments.PlayerEvents;
using GameCore;
using PlayerRoles;
using PlayerStatsSystem;
using LabApi.Features.Console;
using RemoteAdmin;
using LabApi.Features.Enums;
using InventorySystem.Items.ThrowableProjectiles;

namespace DiscordLink
{
	public class Events : CustomEventsHandler
	{
		#region Warhead Events

		public override void OnWarheadDetonated(WarheadDetonatedEventArgs args) => BotLink.Instance.SendMessage("Warhead Detonated!");
		public override void OnWarheadStopped(WarheadStoppedEventArgs args) => BotLink.Instance.SendMessage($"{args.Player.LogName} stopped warhead");
		public override void OnWarheadStarted(WarheadStartedEventArgs args)
		{
			switch (args.WarheadState.ScenarioType)
			{
				default:
				case WarheadScenarioType.Start:
					{
						if (!args.IsAutomatic)
							BotLink.Instance.SendMessage($"{args.Player.LogName} started warhead");
						else
							BotLink.Instance.SendMessage($"Warhead started! {args.WarheadState.ScenarioId}");
						break;
					}
				case WarheadScenarioType.Resume:
					{
						if (!args.IsAutomatic)
							BotLink.Instance.SendMessage($"{args.Player.LogName} resumed warhead");
						else
							BotLink.Instance.SendMessage($"Warhead resumed! {args.WarheadState.ScenarioId}");
						break;
					}
				case WarheadScenarioType.DeadmanSwitch:
					{
						BotLink.Instance.SendMessage($"Warhead triggered by DMS!");
						break;
					}
			}
		}

		#endregion

		#region Round

		public override void OnServerWaitingForPlayers() => BotLink.Instance.SendMessage("**Waiting for players...**");

		public static DateTime RoundEndTime = DateTime.Now;
		public override void OnServerRoundEnded(RoundEndedEventArgs args)
		{
			RoundEndTime = DateTime.Now;
			BotLink.Instance.SendMessage($"**Round Ended**" +
				$"\n```Round Time: {new DateTime(TimeSpan.FromSeconds(RoundSummary.roundTime).Ticks):HH:mm:ss}" +
				$"\nEscaped D-class: {RoundSummary.EscapedClassD}" +
				$"\nRescued Scientists: {RoundSummary.EscapedScientists}" +
				$"\nSurviving SCPs: {RoundSummary.SurvivingSCPs}" +
				$"\nWarhead Status: {(Warhead.IsDetonated ? "Detonated" : "Not detonated")}" +
				$"\nDeaths: {RoundSummary.Kills} ({RoundSummary.KilledBySCPs} by SCPs)");
		}

		public override void OnServerRoundStarting(RoundStartingEventArgs args) => BotLink.Instance.SendMessage($"**Round started ({Server.PlayerCount}/{Server.MaxPlayers})**");

		#endregion

		#region Players

		public override void OnPlayerCuffed(PlayerCuffedEventArgs args) => BotLink.Instance.SendMessage($"**{args.Player.Nickname} disarmed {args.Target.Nickname}**");
		public override void OnPlayerUncuffed(PlayerUncuffedEventArgs args) => BotLink.Instance.SendMessage($"**{args.Player.Nickname} freed {args.Target.Nickname}**");
		public override void OnPlayerEscaped(PlayerEscapedEventArgs args) => BotLink.Instance.SendMessage($"{args.Player.Nickname} escaped ({args.Player.Role} -> {args.NewRole})");
		public override void OnPlayerThrewProjectile(PlayerThrewProjectileEventArgs args) => BotLink.Instance.SendMessage($"{args.Player.Nickname} threw {args.Item.ItemTypeId}");
		public override void OnPlayerJoined(PlayerJoinedEventArgs args) => BotLink.Instance.SendMessage($"**{args.Player.Nickname} Joined ({args.Player.UserId} \\| ||~~{args.Player.IpAddress}~~|| \\| ID: {args.Player.PlayerId})**");
		public override void OnPlayerLeft(PlayerLeftEventArgs args)
		{
			if (args.Player.IsServer || !string.IsNullOrEmpty(args.Player.UserId))
				BotLink.Instance.SendMessage(new Msg($"{args.Player.Nickname} Left ({args.Player.UserId} \\| ||~~{args.Player.IpAddress}~~|| \\| ID: {args.Player.PlayerId})"));
		}
		public override void OnPlayerDying(PlayerDyingEventArgs args)
		{
			try
			{

				if (!args.IsAllowed || args.Player == null || !Extensions.RoundInProgress())
					return;
				var uDH = args.DamageHandler as UniversalDamageHandler;

				if (args.DamageHandler is AttackerDamageHandler aDH)
				{
					if (aDH.IsSuicide)
						BotLink.Instance.SendMessage(new Msg($"{args.Player.Nickname} died to {aDH.GetDamageSource()} (SELF)"));
					else if (aDH.IsFriendlyFire || Extensions.IsFF(args.Player, args.Attacker))
					{
						BotLink.Instance.SendMessage(new Msg($"**Teamkill** " +
							$"\n```autohotkey" +
							$"\nPlayer: [{args.Attacker.PlayerId}] {args.Attacker.Role} {args.Attacker.ToLogString()}" +
							$"\nKilled: [{args.Player.PlayerId}] {args.Player.Role} {args.Player.ToLogString()}" +
							$"\nUsing: {aDH.GetDamageSource()}```"));
					}
					else if (args.Player.IsDisarmed && !args.Attacker.ReferenceHub.IsSCP())
					{
						BotLink.Instance.SendMessage(new Msg($"__Disarmed Kill__" +
							$"\n```autohotkey" +
							$"\nPlayer: [{args.Attacker.PlayerId}] {args.Attacker.Role} {args.Attacker.ToLogString()}" +
							$"\nKilled: [{args.Player.PlayerId}] {args.Player.Role} {args.Player.ToLogString()}" +
							$"\nUsing: {aDH.GetDamageSource()}```"));
					}
					else
						BotLink.Instance.SendMessage(new Msg($"{args.Attacker.Role} {args.Attacker.Nickname} killed {args.Player.Role} {args.Player.Nickname} ({aDH.GetDamageSource()})"));
				}
				else if (args.DamageHandler is WarheadDamageHandler wDH)
					BotLink.Instance.SendMessage(new Msg($"Warhead killed {args.Player.ToLogString()}"));
				else
					BotLink.Instance.SendMessage(new Msg($"{args.Player.ToLogString()} died ({DeathTranslations.TranslationsById[uDH.TranslationId].LogLabel})"));
			}
			catch (Exception e)
			{
				Logger.Info(args.DamageHandler.ServerLogsText);
			}
		}
		public override void OnPlayerHurting(PlayerHurtingEventArgs args)
		{
			if (!args.IsAllowed || args.Player == null || args.Target == null || !Extensions.RoundInProgress())
				return;

			if (args.DamageHandler is AttackerDamageHandler aDH)
			{
				if (aDH.IsSuicide || args.Player.UserId == args.Target.UserId)
				{
					BotLink.Instance.SendMessage(new Msg($"{args.Target.Nickname} -> (SELF) {Math.Round(aDH.Damage, 1)} ({aDH.GetDamageSource()})"));
				}
				else if (aDH.IsFriendlyFire || Extensions.IsFF(args.Target, args.Player))
				{
					BotLink.Instance.SendMessage(new Msg($"**{args.Player.Role} {args.Player.ToLogString()} -> {args.Target.Role} {args.Target.ToLogString()} -> {Math.Round(aDH.Damage, 1)} ({aDH.GetDamageSource()})**"));
				}
				else if (args.Target.IsDisarmed && !args.Player.ReferenceHub.IsSCP())
				{
					BotLink.Instance.SendMessage(new Msg($"__{args.Player.Role} {args.Player.ToLogString()} -> {args.Target.Role} {args.Target.ToLogString()} -> {Math.Round(aDH.Damage, 1)} ({aDH.GetDamageSource()})__"));
				}
				else
				{
					if (aDH.Damage >= 1)
						BotLink.Instance.SendMessage(new Msg($"{args.Player.Nickname} -> {args.Target.Nickname} -> {Math.Round(aDH.Damage, 1)} ({aDH.GetDamageSource()} {aDH.Hitbox})"));
				}
			}
			else if (args.DamageHandler is WarheadDamageHandler wDH)
			{
				BotLink.Instance.SendMessage(new Msg($"Warhead damaged {args.Target.ToLogString()} {Math.Round(wDH.Damage, 1)}"));
			}
			else if (args.DamageHandler is UniversalDamageHandler uDH)
			{
				//BotLink.Instance.SendMessage(new msgMessage($"{args.Target.ToLogString()} was damaged by {DeathTranslations.TranslationsById[uDH.TranslationId].LogLabel}"));
			}
		}
		public override void OnPlayerChangedRole(PlayerChangedRoleEventArgs args)
		{
			if (args.NewRole == RoleTypeId.Spectator || args.NewRole == RoleTypeId.None || args.OldRole.RoleTypeId == RoleTypeId.Spectator || args.OldRole.RoleTypeId == RoleTypeId.None)
				return;

			BotLink.Instance.SendMessage(new Msg($"{args.Player.ToLogString()} changed from {args.OldRole.RoleTypeId} to {args.NewRole} ({args.ChangeReason})"));
		}
		public override void OnPlayerSpawned(PlayerSpawnedEventArgs args)
		{
			if (args.Role.RoleTypeId == RoleTypeId.None || args.Role.RoleTypeId == RoleTypeId.Spectator || args.Role.RoleTypeId == RoleTypeId.Overwatch || !Extensions.RoundInProgress())
				return;
			BotLink.Instance.SendMessage(new Msg($"{args.Player.ToLogString()} spawned as {args.Role.RoleTypeId}"));
		}

		#endregion

		#region Admin

		public override void OnPlayerMuted(PlayerMutedEventArgs args) => BotLink.Instance.SendMessage($"{args.Issuer.ToLogString()} has {(args.IsIntercom ? "icom-" : "")}muted {args.Player.ToLogString()}");
		public override void OnPlayerUnmuted(PlayerUnmutedEventArgs args) => BotLink.Instance.SendMessage($"{args.Issuer.ToLogString()} has {(args.IsIntercom ? "icom-" : "")}unmuted {args.Player.ToLogString()}");
		public override void OnPlayerBanned(PlayerBannedEventArgs args) => BotLink.Instance.SendMessage($"**New Ban!**```autohotkey" +
			$"\nUser: {args.Player.ToLogString()}" +
			$"\nAdmin: {args.Issuer.ToLogString()}" +
			$"\nDuration: {args.Duration / 60} {(args.Duration / 60 > 1 ? "minutes" : "minute")}" +
			$"\nReason: {(string.IsNullOrEmpty(args.Reason) ? "No reason provided" : args.Reason)}```");
		public override void OnPlayerKicked(PlayerKickedEventArgs args)
		{
			if (args.Issuer.IsServer)
			{
				BotLink.Instance.SendMessage(($"**Player Kicked!**```autohotkey" +
					$"\nUser: {args.Player.ToLogString()}" +
					$"\nAdmin: SERVER" +
					$"\nReason: {(string.IsNullOrEmpty(args.Reason) ? "No reason provided" : args.Reason)}```"));
			}
			else
			{
				BotLink.Instance.SendMessage(($"**Player Kicked!**```autohotkey" +
					$"\nUser: {args.Player.ToLogString()}" +
					$"\nAdmin: {args.Issuer}" +
					$"\nReason: {(string.IsNullOrEmpty(args.Reason) ? "No reason provided" : args.Reason)}```"));
			}
		}
		public override void OnServerCommandExecuted(CommandExecutedEventArgs args)
		{
			string prefix = "(RA)";
			string player = "Console";


			if(args.CommandType == CommandType.Console)
			{
				prefix = "(CONSOLE)";
			}
			else if(args.CommandType == CommandType.Client)
			{
				prefix = "(CLIENT)";
				player = Player.Get(args.Sender.SenderId).ToLogString();
			}
			else
			{
				if (args.Sender is ServerConsoleSender SCS)
				{
					prefix = "(CONSOLE)";
				}

				if (args.Sender is PlayerCommandSender pCS)
				{
					player = Player.Get(pCS.PlayerId).ToLogString();
				}
			}

			BotLink.Instance.SendMessage($"{prefix} {player} ran: **{commandToString(args)}**");
		}

		private string commandToString(CommandExecutedEventArgs args)
		{
			return $"{(args.Arguments.Count > 0 ? $"{args.Command.Command} {string.Join(" ", args.Arguments)}" : $"{args.Command.Command}")}";
		}

		#endregion

		#region World

		public override void OnServerGrenadeExploded(GrenadeExplodedEventArgs args) => BotLink.Instance.SendMessage(($"Frag grenade ({args.Player.Nickname}) exploded"));
		

		#endregion
	}
}
