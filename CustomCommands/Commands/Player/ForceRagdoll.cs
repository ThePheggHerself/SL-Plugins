using CommandSystem;
using CustomCommands.Events;
using CustomPlayerEffects;
using Mirror;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.Ragdolls;
using PlayerStatsSystem;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;


namespace CustomCommands.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class TripCommand : ICustomCommand
    {
        public string Command => "ragdoll";

        public string[] Aliases { get; } = { "trip" };

        public string Description => "Ragdolls a specified player";

        public string[] Usage { get; } = { "%player%", "force multiplyer", "ragdoll time (s)" };

        public PlayerPermissions? Permission => PlayerPermissions.Effects;
        public string PermissionString => string.Empty;

        public bool RequirePlayerSender => false;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CanRun(this, arguments, out response, out var players, out _))
                return false;

            float forceMultiplyer = 0;
            float time = 3;

            if (arguments.Count >= 2)
                float.TryParse(arguments.ElementAt(1), out forceMultiplyer);

            if (arguments.Count >= 3)
                float.TryParse(arguments.ElementAt(2), out time);

            foreach (PluginAPI.Core.Player plr in players)
                plr.RagdollPlayer(time, forceMultiplyer);

            response = $"{players.Count} player{(players.Count == 1 ? "s" : "")} ragdolled";

            return true;
        }
    }

    public static class ForceRagdoll
    {
        public static BasicRagdoll SpawnRagdoll(RagdollData ragdollData, StandardDamageHandler dh)
        {
            PlayerRoleLoader.TryGetRoleTemplate(ragdollData.RoleType, out FpcStandardRoleBase ragdollRole);

            BasicRagdoll basicRagdoll = UnityEngine.Object.Instantiate<BasicRagdoll>(ragdollRole.Ragdoll);
            basicRagdoll.NetworkInfo = ragdollData;
            basicRagdoll.gameObject.AddComponent<FakeRagdoll>();
            NetworkServer.Spawn(basicRagdoll.gameObject, (NetworkConnection)null);
            return basicRagdoll;
        }
        public static BasicRagdoll SpawnRagdoll(string nickname, RoleTypeId role, Vector3 position, Quaternion rotation, Vector3 velocity, string deathReason)
        {
            PlayerRoleLoader.TryGetRoleTemplate(role, out FpcStandardRoleBase ragdollRole);

            var dh = new CustomReasonDamageHandler(deathReason);

            typeof(StandardDamageHandler).GetField("StartVelocity", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
    .SetValue(dh, velocity);

            return SpawnRagdoll(new RagdollData(null, dh, role, position, rotation, nickname, NetworkTime.time), dh);
        }


        public static void RagdollPlayer(this PluginAPI.Core.Player plr, float time = 3, float forceMultiplyer = 1)
        {

            Vector3 velocity = plr.Velocity;
            velocity += plr.Camera.transform.forward * UnityEngine.Random.Range(1, 1.5f) * forceMultiplyer;

            velocity += plr.Camera.transform.up * UnityEngine.Random.Range(0.75f, 1.25f) * forceMultiplyer;
            var basicRagdoll = SpawnRagdoll(plr.Nickname, plr.Role, plr.Position, plr.GameObject.transform.rotation, velocity, "guh");

            var items = plr.ReferenceHub.inventory.UserInventory.Items;
            plr.CurrentItem = null;
            plr.ReferenceHub.inventory.UserInventory.Items = new System.Collections.Generic.Dictionary<ushort, InventorySystem.Items.ItemBase>();
            plr.EffectsManager.EnableEffect<Invisible>(time);
            plr.EffectsManager.EnableEffect<Ensnared>(time);

            MEC.Timing.CallDelayed(time, () => {

                plr.ReferenceHub.inventory.UserInventory.Items = items;
                plr.Position = basicRagdoll.CenterPoint.position + Vector3.up;
                NetworkServer.Destroy(basicRagdoll.gameObject);

            });
        }
    }
}