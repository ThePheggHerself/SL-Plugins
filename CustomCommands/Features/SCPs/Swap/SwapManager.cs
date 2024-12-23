using PlayerRoles;
using PlayerRoles.RoleAssign;
using PluginAPI.Core;
using RoundRestarting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CustomCommands.Features.SCPs.Swap
{
	public static class SwapManager
	{
		public static int SCPsToReplace = 0;
		public static int ReplaceBaseCooldownRounds = 4;
		public static int SwapSeconds = 60;
		public static Dictionary<string, int> triggers = new Dictionary<string, int>();
        public static List<KeyValuePair<string, uint>> RaffleParticipants = new List<KeyValuePair<string, uint>>();

        public static void ReplaceBroadcast()
		{
			Server.ClearBroadcasts();
			Server.SendBroadcast($"There {(SCPsToReplace == 1 ? "is" : "are")} now {SCPsToReplace} SCP spot{(SCPsToReplace == 1 ? "" : "s")} available. Run \".scp\" to queue for an SCP", 5);
		}
		public static bool LateTimer = false;

		public static RoleTypeId[] AvailableSCPs
		{
			get
			{
				var Roles = new List<RoleTypeId>() { RoleTypeId.Scp049, /*RoleTypeId.Scp079, */RoleTypeId.Scp106, RoleTypeId.Scp173, RoleTypeId.Scp939, RoleTypeId.Scp096 };

				var scpRoles = Player.GetPlayers().Where(r => r.ReferenceHub.IsSCP()).Select(r => r.Role);
				//if (scpRoles.Any())
				foreach (var r in scpRoles)
				{
					if (Roles.Contains(r))
						Roles.Remove(r);
				}
				//else
				//    Roles.Remove(RoleTypeId.Scp079);

				return Roles.ToArray();
			}
		}

		public static Dictionary<string, int> Cooldown = new Dictionary<string, int>();

		public static bool CanScpSwapToHuman(ReferenceHub plr, out string reason) => CanScpSwapToHuman(Player.Get(plr), out reason);
		public static bool CanScpSwapToHuman(Player plr, out string reason)
		{
            if (!plr.IsSCP || plr.Role == RoleTypeId.Scp0492)
            {
				reason = "You must be an SCP to run this command";
				return false;
			}

            if (plr.Health != plr.MaxHealth)
			{
				reason = "You cannot swap as you have taken damage";
				return false;
			}
			if (plr.TemporaryData.Contains("replacedscp"))
			{
				reason = "You cannot swap back to human";
				return false;
			}
			if (Round.Duration > TimeSpan.FromSeconds(SwapSeconds))
			{
				reason = $"You can only swap from SCP within the first {SwapSeconds} seconds of a round";
				return false;
			}

			reason = string.Empty;
			return true;
		}

		public static bool CanHumanSwapToScp(ReferenceHub plr, out string reason) => CanHumanSwapToScp(Player.Get(plr), out reason);
		public static bool CanHumanSwapToScp(Player plr, out string reason)
		{
			if (SCPsToReplace < 1)
			{
				reason = "There are no SCPs to replace";
				return false;
			}
			if (plr.TemporaryData.Contains("startedasscp"))
			{
				reason = "You were already an SCP this round";
				return false;
			}
			if (Round.Duration > TimeSpan.FromSeconds(SwapSeconds * 1.5) && !SwapManager.LateTimer || Round.Duration > TimeSpan.FromSeconds(SwapSeconds * 2))
			{
				reason = $"You can only replace an SCP within the first {SwapSeconds * 1.5} seconds of the round";
				return false;
			}
			if (SwapManager.Cooldown.TryGetValue(plr.UserId, out int roundCount))
			{
				if (roundCount > RoundRestart.UptimeRounds)
				{
					if (SwapManager.triggers.TryGetValue(plr.UserId, out int count))
					{
						if (count > 2)
						{
							SwapManager.Cooldown[plr.UserId]++;
							SwapManager.triggers[plr.UserId] = 0;
						}
						else SwapManager.triggers[plr.UserId]++;
					}
					else
						SwapManager.triggers.Add(plr.UserId, 1);


					reason = $"You are on cooldown for another {SwapManager.Cooldown[plr.UserId] - RoundRestart.UptimeRounds} round(s).";
					return false;
				}
			}

			reason = string.Empty;
			return true;
		}

        public static void SwapScpToHuman(ReferenceHub plr) => SwapScpToHuman(Player.Get(plr));
		public static void SwapScpToHuman(Player plr)
		{
			SwapManager.SCPsToReplace++;
			HumanSpawner.SpawnLate(plr.ReferenceHub);
			plr.TemporaryData.Add("startedasscp", true.ToString());
			SwapManager.ReplaceBroadcast();
		}

        public static void QueueSwapHumanToScp(ReferenceHub plr) => QueueSwapHumanToScp(Player.Get(plr));
        public static void QueueSwapHumanToScp(Player plr)
        {
            bool first = RaffleParticipants.Count == 0;

            ScpTicketsLoader tix = new ScpTicketsLoader();
            int numTix = tix.GetTickets(plr.ReferenceHub, 10);
            tix.Dispose();
            uint rGroup = 1;
            if (numTix >= 13) rGroup = (uint)numTix;
            if (plr.Role == RoleTypeId.Spectator) rGroup <<= 8;

            RaffleParticipants.Add(new KeyValuePair<string, uint>(plr.UserId, rGroup));

            if (first)
            {
                MEC.Timing.CallDelayed(5f, () =>
                {
                    string draw = "";
                yoinkus: //Makes sure the person didn't leave in the 5 second draw time
                    if (RaffleParticipants.Count > 0)
                    {
                        List<string> DrawGroup = new List<string>();
                        if (RaffleParticipants.Count >= 6)
                        {
                            RaffleParticipants.Sort((x, y) => -x.Value.CompareTo(y.Value)); //I think this is descending order?
                            DrawGroup = RaffleParticipants.Take(RaffleParticipants.Count / 2).Select(x => x.Key).ToList();
                        }
                        else DrawGroup = RaffleParticipants.Select(x => x.Key).ToList();

                        draw = DrawGroup.PullRandomItem();
                    }
                    else
                    {
                        return;
                    }

                    if (!Player.TryGet(draw, out var drawPlr)) goto yoinkus;
                    else
                        SwapHumanToScp(drawPlr);

                    RaffleParticipants.Clear();
                });
            }
        }

		public static void SwapHumanToScp(ReferenceHub plr) => SwapHumanToScp(Player.Get(plr));
		public static void SwapHumanToScp(Player plr)
		{
			var scps = SwapManager.AvailableSCPs;

			plr.SetRole(scps[new Random().Next(0, scps.Length)], RoleChangeReason.LateJoin);
            ScpTicketsLoader tix = new ScpTicketsLoader();
            tix.ModifyTickets(plr.ReferenceHub, 10);
			tix.Dispose();
            plr.TemporaryData.Add("replacedscp", plr.Role.ToString());

			SwapManager.SCPsToReplace--;

			if (SwapManager.Cooldown.ContainsKey(plr.UserId))
				SwapManager.Cooldown[plr.UserId] = RoundRestart.UptimeRounds + SwapManager.ReplaceBaseCooldownRounds;
			else
				SwapManager.Cooldown.Add(plr.UserId, RoundRestart.UptimeRounds + SwapManager.ReplaceBaseCooldownRounds);
		}
	}
}
