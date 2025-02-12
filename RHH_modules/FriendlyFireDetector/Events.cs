using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.CustomHandlers;
using LabApi.Features.Wrappers;
using PlayerRoles;
using PlayerStatsSystem;
using RedRightHandCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FriendlyFireDetector
{
	public class Events : CustomEventsHandler
	{
		public static bool FFDPaused = false;

		public override void OnPlayerHurting(PlayerHurtingEventArgs args)
		{
			if (FFDPaused || !Extensions.RoundInProgress() || !IsValidPlayer(args.Player) || !IsValidPlayer(args.Target) || args.Player.UserId == args.Target.UserId ||
				args.DamageHandler is not AttackerDamageHandler aDH)
				return;

			if(Extensions.IsFF(args.Target, args.Player))
			{
				int friendlies = 0;
				int hostiles = 0;

				foreach (var plr in GetNearbyPlayers(args.Player))
				{
					if(plr.UserId == args.Target.UserId)
						continue;

					if (Extensions.IsFF(plr, args.Player))
						friendlies++;
					else
						hostiles++;
				}

				if(hostiles < 1)
				{
					args.IsAllowed = false;

					if (FFDPluginCore.Config.ReverseDamage)
					{
						args.Player.Damage(aDH.Damage * FFDPluginCore.Config.ReverseDamageModifier, "FFD Damage Reversal");
					}
				}
			}
		}

		private bool IsValidPlayer(Player plr)
		{
			return plr != null && !plr.IsNpc && !plr.IsServer && plr.IsOnline && !plr.IsTutorial;
		}

		private List<Player> GetNearbyPlayers(Player atkr, bool rangeOnly = false)
		{
			float distanceCheck = atkr.Position.y > 900 ? 70 : 35;
			List<Player> nearbyPlayers = new List<Player>();

			foreach (var plr in Player.List)
			{
				if (plr.IsServer || plr.Role == RoleTypeId.Spectator)
					continue;

				var distance = Vector3.Distance(atkr.Position, plr.Position);

				if (rangeOnly && distance <= distanceCheck)
					nearbyPlayers.Add(plr);
				else
				{
					var angle = Vector3.Angle(atkr.GameObject.transform.forward, atkr.Position - plr.Position);

					if ((distance <= distanceCheck && angle > 130) || distance < 5)
						nearbyPlayers.Add(plr);
				}
			}

			return nearbyPlayers;
		}
	}
}
