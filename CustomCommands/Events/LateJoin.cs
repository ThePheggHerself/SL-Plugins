using PlayerRoles.RoleAssign;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;

namespace CustomCommands.Events
{
	public class LateJoin
	{
		[PluginEvent(ServerEventType.PlayerJoined), PluginPriority(LoadPriority.Low)]
		public void PlayerJoin(PlayerJoinedEvent args)
		{
			if (!Plugin.EventInProgress && Round.IsRoundStarted && Round.Duration.TotalSeconds < 30)
				HumanSpawner.SpawnLate(args.Player.ReferenceHub);
		}
	}
}
