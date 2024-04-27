using PlayerRoles;
using PluginAPI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCommands.Features.SCPs.Swap
{
	public static class SwapManager
	{
		public static int SCPsToReplace = 0;
		public static void ReplaceBroadcast() => Server.SendBroadcast($"There {(SCPsToReplace == 1 ? "is" : "are")} now {SCPsToReplace} SCP spot{(SCPsToReplace == 1 ? "" : "s")} available. Run \".scp\" to queue for an SCP", 5);
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
	}
}
