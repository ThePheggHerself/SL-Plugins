using CommandSystem;

namespace CustomCommands
{
	public interface ICustomCommand : ICommand, IUsageProvider
	{
		PlayerPermissions? Permission { get; }
		string PermissionString { get; }
		bool RequirePlayerSender { get; }
		bool SanitizeResponse { get; }
	}
}
