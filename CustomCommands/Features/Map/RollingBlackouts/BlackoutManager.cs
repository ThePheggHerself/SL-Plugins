using Interactables.Interobjects.DoorUtils;
using MEC;
using PluginAPI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCommands.Features.Map.RollingBlackouts
{
	public class BlackoutManager
	{
		public static int MinTime = 180, MaxTime = 360, DelayThisRound;
		public static bool Pause, TriggeredThisRound;
		public static CoroutineHandle Coroutine;

		public void StartBlackoutManager()
		{
			Coroutine = Timing.CallContinuously(0f, () => LightFailure(), Segment.FixedUpdate);
		}

		public IEnumerator<float> LightFailure()
		{
			if (Pause || !Round.IsRoundStarted || Round.IsRoundEnded || Round.Duration.TotalSeconds < DelayThisRound || TriggeredThisRound)
				yield break;

			TriggeredThisRound = true;
			Cassie.Message("Attention all personnel . Power malfunction detected . Repair protocol delta 12 activated . Heavy containment zone power termination in 3 . 2 . 1", false, true, true);
			yield return Timing.WaitForSeconds(18f);

			if (!Round.IsRoundStarted || Round.IsRoundEnded)
				yield break;

			foreach (RoomLightController instance in RoomLightController.Instances)			
				if (instance.Room.Zone == MapGeneration.FacilityZone.HeavyContainment)
					instance.ServerFlickerLights(Plugin.Config.BlackoutDuration);
				
			foreach (var door in DoorVariant.AllDoors.Where(r => r.IsInZone(MapGeneration.FacilityZone.HeavyContainment)))		
				if (door is IDamageableDoor iDD && door.RequiredPermissions.RequiredPermissions == KeycardPermissions.None && !door.name.Contains("LCZ"))
					door.NetworkTargetState = true;
				
			foreach (var tesla in TeslaGateController.Singleton.TeslaGates)
				tesla.enabled = false;
			

			yield return Timing.WaitForSeconds(Plugin.Config.BlackoutDuration);

			if (!Round.IsRoundStarted || Round.IsRoundEnded)
				yield break;

			Cassie.Message("Power system repair complete . System back online", false, true, true);

			foreach (var tesla in TeslaGateController.Singleton.TeslaGates)
				tesla.enabled = true;
			
			yield return 0f;
		}
	}
}
