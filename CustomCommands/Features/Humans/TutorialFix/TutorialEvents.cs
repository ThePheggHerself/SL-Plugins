using PlayerRoles;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace CustomCommands.Features.Humans.TutorialFix
{
	public class TutorialEvents
	{
		[PluginEvent]
		public bool OnDisarm(PlayerHandcuffEvent args)
		{
			if (args.Target.Role == RoleTypeId.Tutorial)
				return false;
			else
				return true;
		}

		[PluginEvent]
		public void CoinFlip(PlayerCoinFlipEvent args)
		{
			if (args.Player.Role == RoleTypeId.Tutorial && Plugin.Config.TutorialCoinExplosion)
			{
				MEC.Timing.CallDelayed(2, () =>
				{
					if (!args.IsTails)
					{
						ExplosionUtils.ServerExplode(args.Player.ReferenceHub);
					}
				});
			}
		}
	}
}
