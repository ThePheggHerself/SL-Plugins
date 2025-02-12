using CommandSystem;
using LabApi.Features.Permissions;
using LabApi.Features.Wrappers;
using Newtonsoft.Json.Linq;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp3114;
using PlayerRoles.PlayableScps.Scp939;
using PlayerStatsSystem;
using RedRightHandCore.Commands;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace RedRightHandCore
{
	public static class Extensions
	{
		public static bool RoundInProgress() => Round.IsRoundStarted && !Round.IsRoundEnded;

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
			else if (!string.IsNullOrEmpty(cmd.PermissionString) && CheckPermission(sender, cmd.PermissionString) )
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

		public static bool CheckPermission(ICommandSender sender, string permission) {
			if (sender is PlayerCommandSender pCS)
			{
				var plr = Player.Get(pCS);

				if (plr != null)
				{
					return plr.HasPermissions(permission);
				}
				//Return false if player cannot be found. How was the command run in the first place?
				else return false;
			}
			else return true;
		}

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

		private static RoleTypeId[] _isCoreSCP = new RoleTypeId[]
		{
			RoleTypeId.Scp173, RoleTypeId.Scp049, RoleTypeId.Scp079,RoleTypeId.Scp096, RoleTypeId.Scp106,RoleTypeId.Scp939, RoleTypeId.Scp3114
		};

		public static bool IsValidSCP(this RoleTypeId role)
		{
			return _isCoreSCP.Contains(role);
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

		public static void UpdatePrivateProperty(this object obj, string propName, object propValue, BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
		{
			var propinfo = obj.GetType().GetProperty(propName, flags);
			propinfo.SetValue(obj, propValue);
		}

		public static string GetDamageSource(this AttackerDamageHandler aDH)
		{
			if (aDH is FirearmDamageHandler fDH)
				return fDH.WeaponType.ToString();
			else if (aDH is ExplosionDamageHandler eDH)
				return "Grenade";
			else if (aDH is MicroHidDamageHandler mhidDH)
				return "Micro HID";
			else if (aDH is RecontainmentDamageHandler reconDH)
				return "Recontainment";
			else if (aDH is Scp018DamageHandler scp018DH)
				return "SCP 018";
			else if (aDH is Scp096DamageHandler scp096DH)
				return "SCP 096";
			else if (aDH is Scp049DamageHandler scp049DH)
				return "SCP 049";
			else if (aDH is Scp939DamageHandler scp939DH)
				return "SCP 939";
			else if (aDH is Scp3114DamageHandler scp3114DH)
				return "SCP 3114";
			else if (aDH is ScpDamageHandler scpDH)
				return scpDH.Attacker.Role.ToString();
			else if (aDH is DisruptorDamageHandler dDH)
				return "Particle Disruptor";
			else if (aDH is JailbirdDamageHandler jDH)
				return "Jailbird";
			else return $"{aDH.GetType().Name}";
		}

		public static string ToLogString(this Player plr) => $"{plr.Nickname} ({plr.UserId})";

		public static bool IsChaos(Player player, bool includeCivs = false)
		{
			switch (player.Role)
			{
				case RoleTypeId.ChaosConscript:
				case RoleTypeId.ChaosRifleman:
				case RoleTypeId.ChaosRepressor:
				case RoleTypeId.ChaosMarauder:
					return true;
				case RoleTypeId.ClassD:
					if (includeCivs)
						return true;
					else return false;
				default:
					return false;
			}
		}

		public static bool IsMtf(Player player, bool includeCivs = false)
		{
			switch (player.Role)
			{
				case RoleTypeId.FacilityGuard:
				case RoleTypeId.NtfCaptain:
				case RoleTypeId.NtfSpecialist:
				case RoleTypeId.NtfPrivate:
				case RoleTypeId.NtfSergeant:
					return true;
				case RoleTypeId.Scientist:
					if (includeCivs)
						return true;
					else return false;
				default:
					return false;
			}
		}

		public static bool IsSCP(Player player)
		{
			switch (player.Role)
			{
				case RoleTypeId.Scp173:
				case RoleTypeId.Scp106:
				case RoleTypeId.Scp049:
				case RoleTypeId.Scp079:
				case RoleTypeId.Scp096:
				case RoleTypeId.Scp0492:
				case RoleTypeId.Scp939:
				case RoleTypeId.Scp3114:
					return true;
				default:
					return false;
			}
		}

		public static bool IsFF(Player victim, Player Attacker)
		{
			var victimRole = victim.ReferenceHub.roleManager.CurrentRole;
			var AttackerRole = Attacker.ReferenceHub.roleManager.CurrentRole;

			if (victimRole.Team == Team.SCPs || AttackerRole.Team == Team.SCPs)
				return false;

			if (IsChaos(victim, true) && IsChaos(Attacker, true))
			{
				if (victim.Role == RoleTypeId.ClassD && Attacker.Role == RoleTypeId.ClassD)
					return false;
				return true;
			}
			else if (IsMtf(victim, true) && IsMtf(Attacker, true))
				return true;

			return false;
		}

		public async static Task<HttpResponseMessage> Post(string Url, StringContent Content)
		{
			using (HttpClient client = new HttpClient())
			{
				client.BaseAddress = new Uri(Url);

				return await client.PostAsync(client.BaseAddress, Content);
			}
		}
		public async static Task<HttpResponseMessage> Get(string Url)
		{
			using (HttpClient client = new HttpClient())
			{
				client.BaseAddress = new Uri(Url);

				return await client.GetAsync(client.BaseAddress);
			}
		}

		public static bool TryParseJSON(string json, out JObject jObject)
		{
			try
			{
				jObject = JObject.Parse(json);
				return true;
			}
			catch
			{
				jObject = null;
				return false;
			}
		}

		public static char[] ValidDurationUnits = { 'm', 'h', 'd', 'w', 'M', 'y' };

		public static TimeSpan GetBanDuration(char unit, int amount)
		{
			switch (unit)
			{
				default:
					return new TimeSpan(0, 0, amount, 0);
				case 'h':
					return new TimeSpan(0, amount, 0, 0);
				case 'd':
					return new TimeSpan(amount, 0, 0, 0);
				case 'w':
					return new TimeSpan(7 * amount, 0, 0, 0);
				case 'M':
					return new TimeSpan(30 * amount, 0, 0, 0);
				case 'y':
					return new TimeSpan(365 * amount, 0, 0, 0);
			}
		}
	}
}
