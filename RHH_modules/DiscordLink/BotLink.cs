using LabApi.Features.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using LabApi.Features.Console;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Extensions = RedRightHandCore.Extensions;

namespace DiscordLink
{
	public class BotLink
	{
		public static BotLink Instance { get; private set; }

		/// <summary>
		/// <see cref="Regex"/> used for cleaning messages (Prevents broken JSON strings and URLs/Discord invites).
		/// </summary>
		public static Regex MsgRgx = new Regex("(.gg/)|(<@)|(http)|({)|(})|(<)|(>)|(\")|(\\[)|(\\])");
		/// <summary>
		/// <see cref="Regex"/> used for cleaning player names (Prevents broken JSON strings and formatting).
		/// </summary>
		public static Regex NameRgx = new Regex("(\\*)|(_)|({)|(})|(@)|(<)|(>)|(\")|(\\[)|(\\])");

		/// <summary>
		/// <see cref="System.Net.Sockets.Socket"/> connection used to communicate between the bot and server.
		/// </summary>
		internal Socket Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		private DateTime _lastMsg;
		/// <summary>
		/// <see cref="IPAddress"/> used for the connection to the bot.
		/// </summary>
		[Obsolete]
		public IPAddress Address;

		Timer StatusTimer, KeepAliveTimer;

		private string _botAddress => $"{DiscordLinkPluginCore.Config.BotAddress}:{DiscordLinkPluginCore.Config.BotPort}";

		public BotLink()
		{
			try
			{
				Instance = this;
				StatusTimer = new Timer(t1 => StatusUpdate(), null, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
				KeepAliveTimer = new Timer(t2 => _keepConnectionAlive(), null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));

				_openConnection();

				new Thread(() =>
				{
					BotListener();
				}).Start();

			}
			catch (Exception ex)
			{
				Logger.Error(ex.ToString());
			}
		}

		private Status _lastStatus = null;
		/// <summary>
		/// Updates the status for the bot.
		/// </summary>
		public void StatusUpdate(bool force = false)
		{
			//Log.Info($"Updating Status!");

			if (force || (_lastStatus == null || _lastStatus.CurrentPlayers != (Player.Count + "/" + Server.MaxPlayers)))
			{
				var msg = new Status();
				SendMessage(msg);
				_lastStatus = msg;
			}
		}


		public void SendMessage(string msgContent) => SendMessage(new Msg(msgContent));

		/// <summary>
		/// Sends a message to the bot.
		/// </summary>
		/// <param name="message"><see cref="MsgBase"/></param>
		public void SendMessage(MsgBase message)
		{
			if (!Socket.Connected && !_openConnection())
				return;

			var msg = JsonConvert.SerializeObject(message);

			try
			{
				Socket.Send(Encoding.UTF8.GetBytes(msg));
				_lastMsg = DateTime.Now;
			}
			catch (Exception e)
			{
				Logger.Error(e.ToString());
			}
		}

		

		private void BotListener()
		{
			while (2 > 1)
			{
				if (Socket.Connected)
				{
					byte[] data = new byte[8192];
					int dataLength = Socket.Receive(data);

					string incomingData = Encoding.UTF8.GetString(data, 0, dataLength);
					List<string> messages = new List<string>(incomingData.Split('\n'));

					foreach (string message in messages)
					{
						if (!string.IsNullOrEmpty(message))
						{
							JObject jObj = JsonConvert.DeserializeObject<JObject>(message);

							if (jObj["Type"].ToString().ToLower() == "plist")
							{
								var msg = new PlayerList(jObj["channel"].ToString());

								SendMessage(msg);

								StatusUpdate(true);
							}
							else if (jObj["Type"].ToString().ToLower() == "cmd")
								Commands(jObj);
						}
					}
				}
				Thread.Sleep(50);
			}
		}

		#region Connection Management

		

		private bool _openConnection()
		{
			try
			{
				_connectSocket();
			}
			catch (Exception)
			{
				try
				{
					Logger.Warn($"Bot connection failed. Resetting conection...");
					Socket.Disconnect(false);

					_connectSocket();
				}
				catch (Exception e)
				{
					Logger.Error($"{DiscordLinkPluginCore.Config.BotAddress}:{DiscordLinkPluginCore.Config.BotPort} Unable to connect with the bot.\n{e}");
					return false;
				}
			}
			return true;
		}
		private void _connectSocket()
		{
			Socket.Connect(DiscordLinkPluginCore.Config.BotAddress, DiscordLinkPluginCore.Config.BotPort);
			Logger.Info($"Bot connection established: {_botAddress}");
		}

		/// <summary>
		/// Prevents the bot's connection from closing.
		/// </summary>
		private void _keepConnectionAlive()
		{
			//Log.Info($"Keep Alive!");
			SendMessage(new KeepAlive());
		}

		#endregion

		#region Commands

		private void Commands(JObject jObj)
		{
			string[] command = jObj["Message"].ToString().Split(' ').Skip(1).ToArray();
			Logger.Info($"Remote command ran by {jObj["StaffID"]}: {command[0].ToLower()}");
			string response;

			try
			{
				switch (command[0].ToLower())
				{
					case "ban":
					case "rban":
					case "remoteban":
						response = BanCommand(command, jObj);
						break;
					case "kick":
					case "rkick":
					case "remotekick":
						response = KickCommand(command, jObj);
						break;
					case "unban":
					case "runban":
					case "remoteunban":
						response = UnbanCommand(command, jObj);
						break;
					default:
						response = "Unknown command";
						//response = ServerConsole.EnterCommand(string.Join(" ", command));
						break;
				}

				SendMessage(new Command(jObj["channel"].ToString(), jObj["StaffID"].ToString(), response));
			}
			catch (Exception e)
			{
				Logger.Error($"Error running remote command: {e}\nCommand Info:{jObj["StaffID"]}: {command[0].ToLower()}");
			}
		}
		private string BanCommand(string[] arg, JObject jObject)
		{
			try
			{
				string command = arg[0];
				string searchvariable = string.Empty;
				TimeSpan duration;
				string durationString = string.Empty;
				string reason = string.Empty;
				Player player = null;

				if (arg.Count() < 4 || arg[1].Length < 1)
					return $"```{arg[0]} [UserID/IP] [Duration] [Reason]```";

				//Forces the search variable to the front of the array
				arg = arg.Skip(1).ToArray();

				if (arg[0].StartsWith("'") || arg[0].StartsWith("\""))
				{
					string result = string.Join(" ", arg).Split(new string[] { "\"" }, 3, StringSplitOptions.None)[1];

					searchvariable = result;

					arg = string.Join(" ", arg).Replace("\"" + result + "\"", string.Empty).Trim(' ').Split(' ');
				}
				else
				{
					searchvariable = arg[0];

					arg = arg.Skip(1).ToArray();
				}

				durationString = arg[0];
				var chars = durationString.Where(Char.IsLetter).ToArray();
				if (chars.Length < 1 || !int.TryParse(new string(durationString.Where(Char.IsDigit).ToArray()), out int amount) || !Extensions.ValidDurationUnits.Contains(chars[0]) || amount < 1)
					return "```diff\n- Invalid duration provided```";

				GetPlayer(searchvariable, out player);

				duration = Extensions.GetBanDuration(chars[0], amount);
				arg = arg.Skip(1).ToArray();
				reason = string.Join(" ", arg);

				if (player != null)
				{
					Server.BanPlayer(player, reason, DateTime.UtcNow.Add(duration).Ticks);
					player.Disconnect($"You have been banned by the server staff\nReason: " + reason);

					return $"`{player.Nickname} ({player.UserId})` was banned for {durationString} with reason: {reason}";
				}
				else
				{
					if (searchvariable.Contains('@'))
						Server.BanUserId(searchvariable, reason, DateTime.UtcNow.Add(duration).Ticks);
					else
						Server.BanIpAddress(searchvariable, reason, DateTime.UtcNow.Add(duration).Ticks);

					return $"`{searchvariable}` was banned for {durationString} with reason: {reason}!";
				}
			}
			catch (Exception e)
			{
				return e.ToString();
			}
		}
		private string KickCommand(string[] arg, JObject jObject)
		{
			if (arg.Count() < 3 || arg[1].Length < 1)
				return $"```{arg[0]} [UserID/IP] [Reason]```";

			string searchvariable = string.Empty;

			//Forces the search variable to the front of the array
			arg = arg.Skip(1).ToArray();

			if (arg[0].StartsWith("'") || arg[0].StartsWith("\""))
			{
				string result = string.Join(" ", arg).Split(new string[] { "\"" }, 3, StringSplitOptions.None)[1];

				searchvariable = result;

				arg = string.Join(" ", arg).Replace("\"" + result + "\"", string.Empty).Trim(' ').Split(' ');
			}
			else
			{
				searchvariable = arg[0];

				arg = arg.Skip(1).ToArray();
			}

			if (!GetPlayer(searchvariable, out Player player))
				return $"Unable to find player `{searchvariable.Replace("`", "\\`")}`";

			string reason = string.Join(" ", arg);

			player.Disconnect($"You have been kicked by the server staff\nReason: " + reason);

			return $"`{player.Nickname} ({player.UserId})` was kicked with reason: {reason}";
		}
		private string UnbanCommand(string[] arg, JObject jObject)
		{
			if (arg.Count() < 2) return $"```{arg[0]} [UserID/Ip]```";

			bool validUID = arg[1].Contains('@');

			bool ban = Server.IsPlayerBanned(arg[1]);

			if (ban)
			{
				if (validUID)
					Server.UnbanUserId(arg[1]);
				else
					Server.UnbanIpAddress(arg[1]);

				return $"`{arg[1]}` has been unbanned.";
			}
			return $"No ban found for `{arg[1]}`.\nMake sure you have typed it correctly, and that it has the @domain prefix if it's a UserID";
		}

		#endregion


		private bool GetPlayer(string SearchParameter, out Player Plr)
		{
			Plr = null;

			var plrs = Player.List;
			IEnumerable<Player> posPlrs;

			if (SearchParameter.Contains('@'))
			{
				posPlrs = plrs.Where(p => p.UserId == SearchParameter).ToArray();

				if (posPlrs.Any())
				{
					Plr = posPlrs.First();
					return true;
				}
			}
			else if (IPAddress.TryParse(SearchParameter, out IPAddress IP))
			{
				posPlrs = plrs.Where(p => p.IpAddress == SearchParameter).ToArray();

				if (posPlrs.Any())
				{
					Plr = posPlrs.First();
					return true;
				}
			}

			posPlrs = plrs.Where(p => p.Nickname.ToLowerInvariant() == SearchParameter.ToLowerInvariant()).ToArray();

			if (posPlrs.Any())
			{
				Plr = posPlrs.First();
				return true;
			}

			return false;
		}
	}
}
