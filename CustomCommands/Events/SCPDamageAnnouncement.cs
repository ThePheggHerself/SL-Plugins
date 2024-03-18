using PlayerStatsSystem;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using System.Collections.Generic;
using System.Linq;

namespace CustomCommands.Events
{
	public class SCPDamageAnnouncement
	{
		public static Dictionary<string, float> ScpDamage = new Dictionary<string, float>();

		[PluginEvent(ServerEventType.PlayerDamage)]
		public void PlayerDamageEvent(PlayerDamageEvent args)
		{
			var plr = args.Player;
			var trgt = args.Target;
			if (trgt == null || plr == null || !(Round.IsRoundStarted && args.DamageHandler is AttackerDamageHandler dmgH && trgt.IsSCP) || dmgH.IsFriendlyFire)
				return;
			if (ScpDamage.ContainsKey(plr.UserId))
				ScpDamage[plr.UserId] += dmgH.Damage;
			else
				ScpDamage.Add(plr.UserId, dmgH.Damage);
		}

		[PluginEvent(ServerEventType.RoundEnd)]
		public void RoundEndEvent(RoundEndEvent args)
		{
			var maxDmg = ScpDamage.Max(x => x.Value);
			var kvp = ScpDamage.FirstOrDefault(x => x.Value == maxDmg);
			if (Player.TryGet(kvp.Key, out var plr))
				Server.SendBroadcast($"{plr.Nickname} dealt the most damage to SCPs with a total of {kvp.Value}", 10);
			ScpDamage.Clear();
		}
	}
}
