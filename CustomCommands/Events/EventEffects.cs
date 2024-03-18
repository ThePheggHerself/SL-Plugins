using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;
using MapGeneration.Distributors;
using PlayerRoles;
using PlayerStatsSystem;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using System.Collections.Generic;
using System.Linq;

namespace CustomCommands.Events
{
	public class EventEffects
	{
		readonly List<Scp079Generator> generators = new List<Scp079Generator>();

		[PluginEvent(ServerEventType.RoundRestart)]
		public void RoundRestart()
		{
			Plugin.CurrentEvent = EventType.NONE;
			generators.Clear();
		}

		[PluginEvent(ServerEventType.TeamRespawn)]

		public bool TeamRespawn(TeamRespawnEvent args)
		{
			if (Plugin.EventInProgress)
				return false;
			else return true;
		}

		[PluginEvent(ServerEventType.PlayerInteractDoor)]
		public bool PlayerInteractDoor(PlayerInteractDoorEvent args)
		{
			if (Plugin.EventInProgress)
			{
				if (args.Door.RequiredPermissions.RequiredPermissions == KeycardPermissions.None)
					return true;
				else return false;
			}
			return args.CanOpen;
		}



		[PluginEvent(ServerEventType.GeneratorActivated)]
		public void GeneratorActivated(GeneratorActivatedEvent args)
		{
			if (Plugin.EventInProgress)
			{
				if (Plugin.CurrentEvent == EventType.Hush)
				{
					generators.Add(args.Generator);

					if (generators.Count == 3)
					{
						var plrs = Player.GetPlayers().Where(r => r.Role == RoleTypeId.Scp939);
						foreach (var plr in plrs)
						{
							plr.Kill();
						}

						Round.End();
					}
				}
			}
		}


		[PluginEvent(ServerEventType.PlayerDeactivatedGenerator)]
		public void GeneratorDeactivated(PlayerDeactivatedGeneratorEvent args)
		{
			if (Plugin.EventInProgress)
			{
				if (Plugin.CurrentEvent == EventType.Hush)
				{
					if (args.Player.IsSCP)
					{
						args.Player.Damage(100, "Generator Disabled");
					}
					else
					{
						args.Player.Damage(50, "Do not disable the generators");
					}
				}
			}
		}

		[PluginEvent(ServerEventType.Scp914Activate)]
		public bool SCP914Activate(Scp914ActivateEvent args)
		{
			if (Plugin.EventInProgress)
				return false;
			else return true;
		}

		[PluginEvent(ServerEventType.PlayerThrowItem)]
		public bool PlayerThrowItem(PlayerThrowItemEvent args)
		{
			if (Plugin.EventInProgress)
			{
				if (Plugin.CurrentEvent == EventType.Hush)
					return false;
				else return true;
			}
			return true;
		}

		[PluginEvent(ServerEventType.PlayerInteractElevator)]
		public bool PlayerInteractElevator(PlayerInteractElevatorEvent args)
		{
			if (Plugin.EventInProgress)
			{
				if (Plugin.CurrentEvent == EventType.SnowballFight)
				{
					return false;
				}

				if (Plugin.CurrentEvent == EventType.Hush)
				{
					if (args.Elevator.AssignedGroup == ElevatorManager.ElevatorGroup.Nuke)
						return true;

					else if (args.Elevator.AssignedGroup == ElevatorManager.ElevatorGroup.Scp049 && args.Player.Role == RoleTypeId.Scp939)
						return true;

					else return false;
				}
				else return false;
			}
			else
			{
				if (args.Player.TemporaryData.Contains("plock"))
				{
					return false;
				}

				return true;
			}
		}

		[PluginEvent(ServerEventType.PlayerDying), PluginPriority(LoadPriority.Highest)]
		public bool PlayerDying(PlayerDyingEvent args)
		{
			if (Plugin.CurrentEvent == EventType.Infection && args.DamageHandler is AttackerDamageHandler)
			{
				args.Player.ReferenceHub.roleManager.ServerSetRole(args.Attacker.Role, RoleChangeReason.RemoteAdmin, RoleSpawnFlags.AssignInventory);

				return false;
			}

			return true;
		}

		[PluginEvent(ServerEventType.PlayerDeath)]
		public void PlayerDeath(PlayerDeathEvent args)
		{
			if (Plugin.CurrentEvent == EventType.SnowballFight)
			{
				args.Attacker.Heal(15);
			}
		}
	}
}
