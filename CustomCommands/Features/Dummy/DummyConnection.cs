using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCommands.Features.Dummy
{
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
