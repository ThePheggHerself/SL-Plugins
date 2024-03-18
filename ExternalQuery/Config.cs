using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExternalQuery
{
	public class Config
	{
		public bool Enabled { get; set; } = false;
		public int Port { get; set; } = 8290;
		public string Password { get; set; } = "NEVERGONNAGIVEYOUUP";
	}
}
