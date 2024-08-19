﻿using PlayerRoles;
using PlayerRoles.RoleAssign;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Respawning;
using MEC;
using PluginAPI.Core;

namespace CustomCommands.Features.Humans.LateSpawn
{
	public class LateSpawnEvents
	{
		public DateTime LastRespawn = new DateTime();
		public SpawnableTeamType LastTeam = SpawnableTeamType.None;

		[PluginEvent, PluginPriority(LoadPriority.Low)]
		public void RespawnEvent(TeamRespawnEvent ev)
		{
			LastRespawn = DateTime.Now;
			LastTeam = ev.Team;
		}


		[PluginEvent, PluginPriority(LoadPriority.Low)]
		public void PlayerDeath(PlayerDeathEvent ev)
		{
			//Log.Info((DateTime.Now - LastRespawn).TotalSeconds.ToString());

			if ((DateTime.Now - LastRespawn).TotalSeconds < Plugin.Config.LateSpawnTime && (ev.Attacker != null && ev.Attacker.Team != Team.SCPs))
			{
				if (LastTeam != SpawnableTeamType.None)
				{
					Timing.CallDelayed(1f, () =>
					{
						if (LastTeam == SpawnableTeamType.NineTailedFox)
							ev.Player.SetRole(RoleTypeId.NtfPrivate);
						else if (LastTeam == SpawnableTeamType.ChaosInsurgency)
							ev.Player.SetRole(RoleTypeId.ChaosRifleman);
					});
				}
			}
		}
	}
}