using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using PluginAPI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;

namespace DynamicTags.Systems
{
	public class Reporting
	{
		[PluginEvent(ServerEventType.PlayerReport), PluginPriority(LoadPriority.Highest)]
		public bool OnPlayerReport(PlayerReportEvent args)
		{
			if(args.Player.TemporaryData.Contains("report") && (DateTime.Now - new DateTime(long.Parse(args.Player.TemporaryData.Get<string>("report")))).TotalMinutes < 5)
			{
				return false;
			}
			var reportDetails = new PlayerReportDetails
			{
				PlayerName = args.Target.Nickname,
				PlayerID = args.Target.UserId,
				PlayerRole = args.Target.Role.ToString(),
				PlayerAddress = args.Target.IpAddress,
				ReporterName = args.Player.Nickname,
				ReporterID = args.Player.UserId,
				ReporterRole = args.Player.Role.ToString(),
				Reason = args.Reason,
				ServerAddress = Server.ServerIpAddress,
				ServerPort = Server.Port.ToString(),
			};

			Extensions.Post(Plugin.Config.ApiEndpoint + "scpsl/report", new StringContent(JsonConvert.SerializeObject(reportDetails), Encoding.UTF8, "application/json"));

			args.Player.TemporaryData.Add("report", DateTime.Now.Ticks.ToString());

			return true;
		}
	}
}
