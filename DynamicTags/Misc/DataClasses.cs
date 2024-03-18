using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicTags
{
	public class TagData
	{
		public bool ReservedSlot { get; set; }

		public string UserID { get; set; }

		public string Tag { get; set; }

		public string Colour { get; set; }

		public ulong Perms { get; set; }

		public string Group { get; set; }
	}
	public class PlayerDetails
	{
		public string UserId;
		public string UserName;
		public string Address;

		public string ServerAddress;
		public string ServerPort;
	}
	public class PlayerBanDetails
	{
		public string PlayerName { get; set; }
		public string PlayerID { get; set; }
		public string PlayerAddress { get; set; }
		public string AdminName { get; set; }
		public string AdminID { get; set; }
		public string Duration { get; set; }
		public string Reason { get; set; }
	}

	public class PlayerReportDetails
	{
		public string PlayerName { get; set; }
		public string PlayerID { get; set; }
		public string PlayerRole { get; set; }
		public string PlayerAddress { get; set; }

		public string ReporterName { get; set; }
		public string ReporterID { get; set; }
		public string ReporterRole { get; set; }

		public string Reason { get; set; }

		public string ServerAddress { get; set; }
		public string ServerPort { get; set; }
	}
}
