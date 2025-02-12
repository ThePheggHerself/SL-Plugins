using LabApi.Events.Handlers;
using LabApi.Features;
using LabApi.Features.Console;
using LabApi.Loader.Features.Plugins;
using LabApi.Loader.Features.Plugins.Enums;
using RedRightHandCore;
using RedRightHandCore.PluginCore;
using System;

namespace Choas
{
	public class ChoasCore : CustomPluginCore
	{
		public override LoadPriority Priority { get; } = LoadPriority.High;
		public override string Name => "Choas";
		public override string Description => "Random plugin for testing and causing chaos";
		public override Version Version => new(0,0,0,1);

		public override void Disable()
		{
			ServerEvents.WaitingForPlayers -= ServerEvents_WaitingForPlayers;
		}

		public override void Enable()
		{
			ServerEvents.WaitingForPlayers += ServerEvents_WaitingForPlayers;
		}

		public void ServerEvents_WaitingForPlayers()
		{
			Logger.Info("Testing custom event");
		}
	}
}
