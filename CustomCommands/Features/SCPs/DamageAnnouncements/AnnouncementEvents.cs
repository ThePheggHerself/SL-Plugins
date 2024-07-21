using PlayerStatsSystem;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using System.Collections.Generic;
using System.Linq;

namespace CustomCommands.Features.SCPs.DamageAnnouncements
{
	public class AnnouncementEvents
	{
		[PluginEvent(ServerEventType.PlayerDamage)]
		public void PlayerDamageEvent(PlayerDamageEvent args)
		{
			if (!Plugin.Config.EnableDamageAnnouncements)
				return;

			var plr = args.Player;
			var trgt = args.Target;
			if (trgt == null || plr == null || !(Round.IsRoundStarted && args.DamageHandler is AttackerDamageHandler dmgH && trgt.IsSCP) || dmgH.IsFriendlyFire)
				return;
			if (AnnouncementManager.ScpDamage.ContainsKey(plr.UserId))
				AnnouncementManager.ScpDamage[plr.UserId] += dmgH.Damage;
			else
				AnnouncementManager.ScpDamage.Add(plr.UserId, dmgH.Damage);
		}

		[PluginEvent(ServerEventType.RoundEnd)]
		public void RoundEndEvent(RoundEndEvent args)
		{
			if (!Plugin.Config.EnableDamageAnnouncements || !AnnouncementManager.ScpDamage.Any())
				return;

			var maxDmg = AnnouncementManager.ScpDamage.Max(x => x.Value);
			var kvp = AnnouncementManager.ScpDamage.FirstOrDefault(x => x.Value == maxDmg);
			if (Player.TryGet(kvp.Key, out var plr))
				Server.SendBroadcast($"{plr.Nickname} dealt the most damage to SCPs with a total of {kvp.Value}", 10);
			AnnouncementManager.ScpDamage.Clear();
		}
	}
}
