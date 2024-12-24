using CommandSystem;
using GameCore;
using Mirror;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Events;
using RelativePositioning;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace CustomCommands.Features.Testing
{
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class Fill : ICustomCommand
	{
		public string Command => "dummyfill";

		public string[] Aliases => null;

		public string Description => "Fills the server with dummy players";

		public string[] Usage { get; } = { "name" };

		public PlayerPermissions? Permission => null;
		public string PermissionString => "cuscom.dummyf";

		public bool RequirePlayerSender => false;

		public bool SanitizeResponse => false;

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CanRun(this, arguments, out response, out _, out _))
				return false;

			var dumsToMake = Server.MaxPlayers - Server.PlayerCount;

			for (int i = 0; i < dumsToMake; i++)
			{
				DummyUtils.SpawnDummy(arguments.ElementAt(0) + i);
			}

			response = $"Server filled with {dumsToMake} dummies";

			return true;
		}
	}

	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class AI : ICustomCommand
	{
		public string Command => "dummyai";

		public string[] Aliases => null;

		public string Description => "Basic AI for dummies";

		public string[] Usage { get; } = { "ID" };

		public PlayerPermissions? Permission => null;
		public string PermissionString => "cuscom.dummyf";

		public bool RequirePlayerSender => false;

		public bool SanitizeResponse => false;

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CanRun(this, arguments, out response, out _, out _))
				return false;

			if (sender is PlayerCommandSender pSender)
			{
				foreach (var a in ReferenceHub.AllHubs)
				{
					if (a.IsDummy)
					{
						if (!a.gameObject.TryGetComponent<NavMeshAgent>(out var agent))
						{
							agent = a.gameObject.AddComponent<NavMeshAgent>();

							agent.baseOffset = 0.98f;
							agent.updateRotation = true;
							agent.angularSpeed = 360;
							agent.acceleration = 600;
							agent.speed = 1;
							agent.height = 1f;

							agent.radius = 0.1f;
							agent.areaMask = 1;
							agent.obstacleAvoidanceType = ObstacleAvoidanceType.MedQualityObstacleAvoidance;
						}

						if (!a.gameObject.TryGetComponent<DummyAI>(out var ai))
							a.gameObject.AddComponent<DummyAI>().Init(a, agent);

						agent.SetDestination(pSender.ReferenceHub.transform.position);

						foreach(var b in agent.path.corners)
						{
							PluginAPI.Core.Log.Info($"{b} + {pSender.ReferenceHub.transform.position}");
						}

						response = $"Path set to {agent.pathEndPosition} | {agent.nextPosition}";
					}
				}
			}

			//response = $"Path set to";
			return true;
		}
	}

	public class DummyAI : MonoBehaviour
	{
		private ReferenceHub _hub;
		private NavMeshAgent _agent;

		public void Init(ReferenceHub hub, NavMeshAgent agent)
		{
			_hub = hub;
			_agent = agent;
		}

		private void Update()
		{
			if (NetworkServer.active)
			{
				IFpcRole fpcRole = _hub.roleManager.CurrentRole as IFpcRole;
				if (fpcRole != null)
				{
					FirstPersonMovementModule fpcModule = fpcRole.FpcModule;

					Vector3 position = _hub.transform.position;

					//PluginAPI.Core.Log.Info("EEEEEEEE");

					

					
					Vector3 dir = _agent.nextPosition - position;
					Vector3 b = Time.deltaTime * 1 * dir.normalized;
					fpcModule.Motor.ReceivedPosition = new RelativePosition(position + b);
					fpcModule.MouseLook.LookAtDirection(dir, 1f);
					return;
				}
			}
			Destroy(this);
		}
	}

	public class TestingDummies
	{

	}
}
