# ExternalQuery
Allows you to execute commands on your SL server remotely.

# Config
| Config  | Default Value | Description |
| ------------- | ------------- | ------------- |
| enabled  | false  | Enables the plguin |
| port  | 8290  | Query port for the plugin |
| password | NEVERGONNAGIVEYOUUP | Password used for queries |

# Commands
| Command | Alias | Permission | Description |
| ------------- | ------------- | ------------- | ------------- |
| list | plist / players | NONE | Get a list of all players on the server |

# Events
This plugin uses no events

# Developer notes
Queries are to be sent as a json string with a "password" object and a "command" opbject. <br />
Due to SL magic, anything that changes a player's role **WILL** crash the server, so the "forceclass" command is not available for remote execution. <br />
Kicking, Banning and Unbanning have also custom overrides to simplify usage, so instead of fighting with player ids or the oban command, you can simply provide the command with a userid/ip (or name if the player is still online) and it will handle the kick/ban/unban
