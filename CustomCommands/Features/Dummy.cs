using CommandSystem;
using MEC;
using Mirror;
using PluginAPI.Core;
using PluginAPI.Events;
using System;
using System.Linq;
using UnityEngine;

namespace CustomCommands.Features
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class DummyCreate : ICustomCommand
	{
		public string Command => "dummycreate";

		public string[] Aliases => null;

		public string Description => "Creates a dummy player";

		public string[] Usage { get; } = { "name" };

		public PlayerPermissions? Permission => null;
		public string PermissionString => "cuscom.dummyc";

		public bool RequirePlayerSender => false;

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CanRun(this, arguments, out response, out _, out _))
				return false;

			var dummy = DummyManager.CreateDummy(arguments.ElementAt(0));

			response = $"Dummy '{dummy.authManager.UserId}' created";

			return true;
		}
	}

	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class DummyDestroy : ICustomCommand
	{
		public string Command => "dummydestroy";

		public string[] Aliases => null;

		public string Description => "Destroys a dummy player";

		public string[] Usage { get; } = { "User ID" };

		public PlayerPermissions? Permission => null;
		public string PermissionString => "cuscom.dummyd";

		public bool RequirePlayerSender => false;

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CanRun(this, arguments, out response, out _, out _))
				return false;

			if (DummyManager.DestroyDummy(arguments.ElementAt(0)))
			{
				response = $"Dummy '{arguments.ElementAt(0)}' destroyed";
				return true;
			}
			else
			{
				response = $"No dummy located";
				return false;
			}
		}
	}

	public class DummyEvents
	{
		[PluginAPI.Core.Attributes.PluginEvent]
		public void WaitingForPlayers(WaitingForPlayersEvent args)
		{
			DummyManager.DummyID = 0;

			//DummyManager.CreateDummy("Steve");
			//Round.IsLocked = true;
		}
	}

	public static class DummyManager
	{
		public static int DummyID = 0;

		public static ReferenceHub CreateDummy(string Name)
		{
			var dummy = GameObject.Instantiate(NetworkManager.singleton.playerPrefab);
			DummyID++;
			int id = DummyID;

			var dcon = new DummyConnection(id);
			var hub = dummy.GetComponent<ReferenceHub>();

			NetworkServer.AddPlayerForConnection(dcon, dummy);

			try
			{
				hub.authManager.NetworkSyncedUserId = $"dummy{id}@server";
				hub.nicknameSync.Network_myNickSync = Name;
				hub.authManager.UserId = $"dummy{id}@server";
			}
			catch (Exception)
			{

			}

			return hub;
		}

		public static bool DestroyDummy(string ID)
		{
			foreach (var plr in Player.GetPlayers())
			{
				if (plr.UserId == ID)
				{
					plr.Kill();

					Timing.CallDelayed(0.2f, () =>
					{
						NetworkServer.RemovePlayerForConnection(plr.ReferenceHub.connectionToClient, true);
					});

					return true;
				}
			}

			return false;
		}
	}

	public class DummyConnection : NetworkConnectionToClient
	{
		public DummyConnection(int networkConnectionId) : base(networkConnectionId)
		{
		}

		public override string address => "AtTheInn";
		public override void Send(ArraySegment<byte> segment, int channelId = 0)
		{

		}
		public override void Disconnect()
		{
			NetworkServer.RemovePlayerForConnection(identity.gameObject.GetComponent<ReferenceHub>().connectionToServer, true);
		}
	}
}
