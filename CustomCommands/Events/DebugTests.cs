using InventorySystem.Items;
using InventorySystem.Items.ThrowableProjectiles;
using Mirror;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.Ragdolls;
using PlayerStatsSystem;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using System.Reflection;
using UnityEngine;

namespace CustomCommands.Events
{
	public class DebugTests
	{
		//[PluginEvent(ServerEventType.PlayerShotWeapon)]
		public void RagdollGun(PlayerShotWeaponEvent args)
		{
			var plr = args.Player;

			//This gets the FpcStandardRoleBase for the specified Role.
			//This is how we tell the server which role to imitate for the ragdoll.
			PlayerRoleLoader.TryGetRoleTemplate(RoleTypeId.ClassD, out FpcStandardRoleBase pRB);

			//Creates a new FirearmDamageHandler. This is required for ragdolls to properly show cause of death.
			//Velocity for ragdolls are also handled entirely by the damagehandler (Which is beyond moronic but nothing we can't fix :P)
			//It doesn't really matter what type of DamageHandler is used, as long as it is some form of AttackerDamageHandler (Such as FirearmDamageHandler)
			//In this case, as we are already shooting a gun, it just makes sense.
			var dh = new FirearmDamageHandler(args.Firearm, 10);

			//This create a new Vector3 with all 3 values of zero (0, 0, 0)
			Vector3 velocity = Vector3.zero;

			//The below adds to the velocity in the specified direction (Forwards, Up or Right)
			//Subtracting on the right velocity will make it go left instead of right.
			//This is mostly just to add randomness to the direction when firing the ragdoll, but static values can be used.
			//In this instance, the transform of the Camera is used to make sure it properly rotates based on the direction of the camera (Both vertically and horizontally)
			
			velocity += args.Player.Camera.transform.forward * Random.Range(5, 10);
			velocity += args.Player.Camera.transform.up * Random.Range(0.75f, 4.5f);

			if (Random.Range(1, 2) % 2 == 0)
				velocity += args.Player.Camera.transform.right * Random.Range(1, 2.5f);
			else
				velocity -= args.Player.Camera.transform.right * Random.Range(1, 2.5f);

			
			//As we are unable to access the 'StartVelocity' field inside of our FirearmDamageHandler, we need to use reflection in order to access and set it.
			//So what is happening is it is getting the type of StandardDamageHandler (Which is the base for all AttackerDamageHandlers) which contains the 'StartVelocity' field, and getting the field.
			//The binding flags are ways to specify which type of field to search for. In this instance, it is a non-static and non-public field.
			//So we are telling it to search for non-static and non-public fields with the name of 'StartVelocity'.
			//Once we have it, we are setting the value of it in the specified instance. So we are giving it the FirearmDamageHandler created previously, and the velocity Vector3 value we just created.
			typeof(StandardDamageHandler).GetField("StartVelocity", BindingFlags.NonPublic | BindingFlags.Instance)
				.SetValue(dh, velocity);

			//This is the data of the ragdoll that is synced to clients.
			//We are providing it the FirearmDamageHandler created earlier (So it indicates the cause of death), the role in which the Ragdoll is (Should be the same as what you got earlier)
			//The starting position of the ragdoll, the rotation of the ragdoll (In this instance, it should match the direction we are looking), the nickname to display on the ragdoll
			//And the time that the ragdoll is created (Ragdolls get frozen on the client after a few seconds, so this needs to be accurate)
			RagdollData data = new RagdollData(null, dh, RoleTypeId.ClassD, plr.Position, plr.GameObject.transform.rotation, plr.Nickname, NetworkTime.time);

			//This creates the ragdoll object itself, and setting the Ragdoll data using FpcStandardRoleBase that we got earlier.
			//It also sets the NetworkInfo to the RagdollData created above, so it gets synced with clients properly.
			BasicRagdoll basicRagdoll = Object.Instantiate(pRB.Ragdoll);
			basicRagdoll.NetworkInfo = data;

			//This finally creates the ragdoll itself for all clients on the server.
			//If this wasn't done, it would be created on the server, but would not be sent through to the clients at all.
			//Any time something gets spawned/created on the server, this needs to be run.
			NetworkServer.Spawn(basicRagdoll.gameObject, (NetworkConnection)null);
		}

		//[PluginEvent(ServerEventType.PlayerShotWeapon)]
		public void GrenadeLauncher(PlayerShotWeaponEvent args)
		{
			var plr = args.Player;

			ThrowableItem item = (ThrowableItem)plr.ReferenceHub.inventory.CreateItemInstance(new ItemIdentifier(ItemType.GrenadeHE, ItemSerialGenerator.GenerateNext()), false);
			Vector3 Pos = plr.Position;

			TimeGrenade grenade = (TimeGrenade)UnityEngine.Object.Instantiate(item.Projectile, Pos, Quaternion.identity);
			grenade.NetworkInfo = new InventorySystem.Items.Pickups.PickupSyncInfo(item.ItemTypeId, item.Weight, item.ItemSerial);
			grenade.Position = Pos;
			grenade.Rotation = plr.GameObject.transform.rotation;

			grenade.GetComponent<Rigidbody>().velocity = new Vector3(UnityEngine.Random.Range(5, 10), UnityEngine.Random.Range(0, 0.75f), UnityEngine.Random.Range(-2, 2));

			grenade.PreviousOwner = new Footprinting.Footprint(plr.ReferenceHub);
			NetworkServer.Spawn(grenade.gameObject);
			grenade.ServerActivate();
		}
	}
}
