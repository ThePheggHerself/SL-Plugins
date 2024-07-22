using System;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Events;

namespace StatTracker
{
    public class Plugin
    {
		[PluginConfig]
		public static Config config;

		[PluginEntryPoint("Stat Tracker", "1.0.0", "Tracks player stats", "PheWitch")]
		public void OnPluginStart()
		{
			EventManager.RegisterEvents<Events>(this);

			Log.Info($"Plugin Loaded! Endpoint: {config.ApiEndpoint}");
		}


		public class Stats
		{
			public Stats(Player plr)
			{
				UserID = plr.UserId;
				DNT = plr.DoNotTrack;
				Jointime = DateTime.UtcNow;
			}

			public string UserID; 
			public bool DNT = true;
			public int SCPsKilled = 0; //How many SCPs the player has killed
			public int SCPKills = 0; //How many kills the player has as SCP
			public int HumansKilled = 0; //How many human kills the player has
			public int HumanKills = 0; //How many kills the player has as a human
			public int MedicalItems = 0; 
			public bool Escaped = false; //Has the player escaped
			public bool RoundWon = false; //Is the player alive as the winning team when the round ends
			public int SecondsPlayed = 0; //How many seconds the player has played for
			public int PlayersDisarmed = 0; //How many people has the player disarmed
			public int DamageDealt = 0; //How much damage the player has dealt
			public int DamageTaken = 0; //How much damage the player has taken
			public int Deaths = 0; //How many times the player has died
			public int SCP = 0;//This is rounds where you spawned as any SCP
			public DateTime Jointime;
		}
	}
}
