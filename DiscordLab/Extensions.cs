using PlayerRoles;
using PlayerRoles.PlayableScps.Scp3114;
using PlayerRoles.PlayableScps.Scp939;
using PlayerStatsSystem;
using PluginAPI.Core;
using PluginAPI.Core.Interfaces;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordLab
{
	public static class Extensions
	{
		public static string GetDamageSource(this AttackerDamageHandler aDH)
		{
            if (aDH is FirearmDamageHandler fDH)
                return fDH.WeaponType.ToString();
            else if (aDH is ExplosionDamageHandler eDH)
                return "Grenade";
            else if (aDH is MicroHidDamageHandler mhidDH)
                return "Micro HID";
            else if (aDH is RecontainmentDamageHandler reconDH)
                return "Recontainment";
            else if (aDH is Scp018DamageHandler scp018DH)
                return "SCP 018";
            else if (aDH is Scp096DamageHandler scp096DH)
                return "SCP 096";
            else if (aDH is Scp049DamageHandler scp049DH)
                return "SCP 049";
            else if (aDH is Scp939DamageHandler scp939DH)
                return "SCP 939";
            else if (aDH is Scp3114DamageHandler scp3114DH)
                return "SCP 3114";
            else if (aDH is ScpDamageHandler scpDH)
                return scpDH.Attacker.Role.ToString();
            else if (aDH is DisruptorDamageHandler dDH)
                return "Particle Disruptor";
            else if (aDH is JailbirdDamageHandler jDH)
                return "Jailbird";

            else return $"{aDH.GetType().Name}";
		}

		public static string ToLogString(this IPlayer plr) => $"{plr.Nickname} ({plr.UserId})";

        public static bool IsChaos(Player player)
        {
            switch (player.Role)
            {
                case RoleTypeId.ChaosConscript:
                case RoleTypeId.ChaosRifleman:
                case RoleTypeId.ChaosRepressor:
                case RoleTypeId.ChaosMarauder:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsMtf(Player player)
        {
            switch (player.Role)
            {
                case RoleTypeId.FacilityGuard:
                case RoleTypeId.NtfCaptain:
                case RoleTypeId.NtfSpecialist:
                case RoleTypeId.NtfPrivate:
                case RoleTypeId.NtfSergeant:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsSCP(Player player)
        {
            switch (player.Role)
            {
                case RoleTypeId.Scp173:
                case RoleTypeId.Scp106:
                case RoleTypeId.Scp049:
                case RoleTypeId.Scp079:
                case RoleTypeId.Scp096:
                case RoleTypeId.Scp0492:
                case RoleTypeId.Scp939:
                case RoleTypeId.Scp3114:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsFF(Player victim, Player Attacker)
        {
            var victimRole = victim.ReferenceHub.roleManager.CurrentRole;
            var AttackerRole = Attacker.ReferenceHub.roleManager.CurrentRole;

            if (victimRole.Team == Team.SCPs || AttackerRole.Team == Team.SCPs)
                return false;

            if ((victimRole.RoleTypeId == RoleTypeId.ClassD || IsChaos(victim)) && (AttackerRole.Team == Team.ClassD || IsChaos(Attacker)))
            {
                if (victim.Role == RoleTypeId.ClassD && Attacker.Role == RoleTypeId.ClassD)
                    return false;
                return true;
            }
            else if ((victimRole.RoleTypeId == RoleTypeId.Scientist || IsMtf(victim)) && (Attacker.Role == RoleTypeId.Scientist || IsMtf(Attacker)))
                return true;

            return false;
        }
    }
}
