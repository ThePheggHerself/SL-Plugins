using CommandSystem;
using CustomCommands.Features.SCPs.Swap;
using NWAPIPermissionSystem;
using PlayerRoles;
using PluginAPI.Core;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;

namespace CustomCommands
{
	public static class Extensions
	{
		public static bool CanRun(this ICommandSender sender, ICustomCommand cmd, ArraySegment<string> args, out string Response, out List<Player> Players, out PlayerCommandSender PlrCmdSender)
		{
			Players = new List<Player>();
			PlrCmdSender = null;

			if (cmd.RequirePlayerSender)
			{
				if (!(sender is PlayerCommandSender pSender))
				{
					Response = "You must be a player to run this command";
					return false;
				}
				PlrCmdSender = pSender;
			}

			if (cmd.Permission != null && !sender.CheckPermission((PlayerPermissions)cmd.Permission))
			{
				Response = $"You do not have the required permission to execute this command: {cmd.Permission}";
				return false;
			}
			else if (!string.IsNullOrEmpty(cmd.PermissionString) && !sender.CheckPermission(cmd.PermissionString))
			{
				Response = $"You do not have the required permission to execute this command: {cmd.PermissionString}";
				return false;
			}

			if (args.Count < cmd.Usage.Length)
			{
				Response = $"Missing argument: {cmd.Usage[args.Count]}";
				return false;
			}

			if (cmd.Usage.Contains("%player%"))
			{
				var index = cmd.Usage.IndexOf("%player%");

				var hubs = RAUtils.ProcessPlayerIdOrNamesList(args, index, out _, false);

				if (hubs.Count < 1)
				{
					Response = $"No player(s) found for: {args.ElementAt(index)}";
					return false;
				}
				else
				{
					foreach (var plr in hubs)
					{
						Players.Add(Player.Get(plr));
					}
				}
			}

			Response = string.Empty;
			return true;
		}

		public static RoleTypeId[] ValidSwapSCP = new RoleTypeId[]
		{
			RoleTypeId.Scp173, RoleTypeId.Scp049, RoleTypeId.Scp079,RoleTypeId.Scp096, RoleTypeId.Scp106,RoleTypeId.Scp939, RoleTypeId.Scp3114
		};

		public static RoleTypeId GetRoleFromString(string role)
		{
			if (Enum.TryParse(role, true, out RoleTypeId roleType))
			{
				if (!IsValidSCP(roleType))
					return RoleTypeId.None;

				return roleType;
			}
			else return RoleTypeId.None;
		}

		public static bool IsValidSCP(this RoleTypeId role)
		{
			return ValidSwapSCP.Contains(role);
		}

		public static string SCPNumbersFromRole(this RoleTypeId role)
		{
			if (IsValidSCP(role))
			{
				return role.ToString().ToLower().Replace("scp", "");
			}
			else return string.Empty;
		}

		public static void AddToOrReplaceValue<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
		{
			if (dict.ContainsKey(key))
				dict[key] = value;
			else
				dict.Add(key, value);
		}
		public static void AddToOrUpdateValue<TKey>(this Dictionary<TKey, int> dict, TKey key, int value)
		{
			if (dict.ContainsKey(key))
				dict[key] += value;
			else
				dict.Add(key, value);
		}
	}
}
