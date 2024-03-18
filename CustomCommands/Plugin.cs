using CustomCommands.Events;
using HarmonyLib;
using PlayerRoles.Ragdolls;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Events;
using System;

namespace CustomCommands
{
	public enum EventType
	{
		NONE = 0,

		Infection = 1,
		Battle = 2,
		Hush = 3,
		SnowballFight = 4 // This event is christmas-exclusive.
	}
	public enum VoteType
	{
		NONE = 0,
		AdminVote = 1,
		AutoEventVote = 2,
	}


	public class Plugin
	{
		public static bool EventInProgress => CurrentEvent != EventType.NONE;
		public static bool VoteInProgress => CurrentVote != VoteType.NONE;

		public static EventType CurrentEvent = EventType.NONE;
		public static VoteType CurrentVote = VoteType.NONE;
		public static string CurrentVoteString = string.Empty;



		[PluginEntryPoint("Custom Commands", "1.0.0", "Simple plugin for custom commands", "ThePheggHerself")]
		public void OnPluginStart()
		{
			Harmony harmony = new Harmony("CC-Patching-Phegg");
			harmony.PatchAll();

			Log.Info($"Plugin is loading...");

			//EventManager.RegisterEvents<DebugTests>(this);
			EventManager.RegisterEvents<CustomCommands.Events.DoorLocking>(this);
			EventManager.RegisterEvents<EventEffects>(this);
			EventManager.RegisterEvents<LateJoin>(this);
			EventManager.RegisterEvents<NameFix>(this);
			EventManager.RegisterEvents<SCPDamageAnnouncement>(this);		
			EventManager.RegisterEvents<SurfaceLightingFix>(this);
			EventManager.RegisterEvents<TutorialFixes>(this);

			EventManager.RegisterEvents<Features.DummyEvents>(this);
			EventManager.RegisterEvents<Features.Voting>(this);
			EventManager.RegisterEvents<Features.SCPSwap>(this);
			
			EventManager.RegisterEvents<Features._079Removal>(this);
			EventManager.RegisterEvents<SCP3114Overhaul>(this);

			//EventManager.RegisterEvents<Features.SpecialWeapons>(this);

			RagdollManager.OnRagdollSpawned += MiscEvents.RagdollManager_OnRagdollSpawned;
		}


	}
}
